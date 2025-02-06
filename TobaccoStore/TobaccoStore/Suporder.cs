using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TobaccoStore
{
    public partial class Suporder : Form
    {
        public Suporder()
        {
            InitializeComponent();
           
            numericUpDownQuantity.ValueChanged += UpdateTotalAmount;
            txtCostPrice.TextChanged += UpdateTotalAmount;
        }

        private DataTable orderDataTable;


        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=Tabacoostore;Trusted_Connection=True;";
        private void Suporder_Load(object sender, EventArgs e)
        {
            LoadProducts();
            LoadSuppliers();

            // Initialize DataTable with separate columns
            orderDataTable = new DataTable();
            orderDataTable.Columns.Add("Product Info");  // Product name + ID
            orderDataTable.Columns.Add("Supplier Info"); // Supplier name + ID
            orderDataTable.Columns.Add("Quantity");
            orderDataTable.Columns.Add("Cost Price");
            orderDataTable.Columns.Add("Total Amount");
            orderDataTable.Columns.Add("Date"); // New column for date

            // Bind DataTable to DataGridView
            dataGridViewOrder.DataSource = orderDataTable;
        }


        private void LoadProducts()
        {
            lstProducts.Items.Clear(); // Clear the list before loading
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT product_id, product_name FROM Product";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // Add products to the ListBox
                        lstProducts.Items.Add(new { Text = reader["product_name"].ToString(), Value = reader["product_id"] });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading products: " + ex.Message);
                }
            }
        }

        private void LoadSuppliers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT supplier_id, supplier_name FROM Supplier";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // Add suppliers to the ListBox
                        lstSuppliers.Items.Add(new { Text = reader["supplier_name"].ToString(), Value = reader["supplier_id"] });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading suppliers: " + ex.Message);
                }
            }
        }
        private void lstProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected product ID
            if (lstProducts.SelectedItem != null)
            {
                var selectedProduct = (dynamic)lstProducts.SelectedItem;
                int productId = selectedProduct.Value;

                // Get cost price and stock quantity of the selected product and update the TextBox
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT cost_price, stock_quantity FROM Product WHERE product_id = @productId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@productId", productId);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            // Set the cost price and stock quantity
                            txtCostPrice.Text = reader["cost_price"].ToString();
                            int stockQuantity = Convert.ToInt32(reader["stock_quantity"]);
                            lblStockQuantity.Text = $"Stock Quantity: {stockQuantity}"; // Update the label with stock quantity
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error fetching product details: " + ex.Message);
                    }
                }

                // Now filter suppliers based on the selected product
                LoadSuppliersForProduct(productId);
            }
        }
        private void LoadSuppliersForProduct(int productId)
        {
            // Load suppliers for the selected product
            lstSuppliers.Items.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT supplier_id, supplier_name FROM Supplier WHERE supplier_id IN (SELECT supplier_id FROM Product WHERE product_id = @productId)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@productId", productId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // Add filtered suppliers to the ListBox
                        lstSuppliers.Items.Add(new { Text = reader["supplier_name"], Value = reader["supplier_id"] });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching suppliers: " + ex.Message);
                }
            }
        }
        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void btnSubmitOrder_Click(object sender, EventArgs e)
        {
            // Check if the orderDataTable is empty
            if (orderDataTable.Rows.Count == 0)
            {
                MessageBox.Show("No items have been added to the order. Please add items before submitting.", "Empty Order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Stop further execution
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Start a transaction
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert data into SupplierOrder table
                        string insertOrderQuery = "INSERT INTO SupplierOrder (supplier_id, order_date, total_amount) " +
                                                  "VALUES (@supplierId, @orderDate, @totalAmount); SELECT SCOPE_IDENTITY();";

                        int supplierId = 0;
                        DateTime orderDate = dateTimePickerOrderDate.Value.Date; // Ensure we get the date part

                        // Assuming that we have selected a supplier and product
                        if (lstSuppliers.SelectedItem != null)
                        {
                            var selectedSupplier = (dynamic)lstSuppliers.SelectedItem;
                            supplierId = selectedSupplier.Value;
                        }

                        // Calculate the total amount
                        decimal totalAmount = 0;
                        foreach (DataRow row in orderDataTable.Rows)
                        {
                            totalAmount += Convert.ToDecimal(row["Total Amount"]);
                        }

                        using (SqlCommand cmd = new SqlCommand(insertOrderQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@supplierId", supplierId);
                            cmd.Parameters.AddWithValue("@orderDate", orderDate);
                            cmd.Parameters.AddWithValue("@totalAmount", totalAmount);

                            // Get the supplier_order_id (identity value)
                            int supplierOrderId = Convert.ToInt32(cmd.ExecuteScalar());

                            // Insert data into SupplierOrderDetails table for each row
                            string insertOrderDetailsQuery = "INSERT INTO SupplierOrderDetails (supplier_order_id, product_id, quantity, cost_price_at_order) " +
                                                             "VALUES (@supplierOrderId, @productId, @quantity, @costPriceAtOrder)";

                            foreach (DataRow row in orderDataTable.Rows)
                            {
                                var productInfo = row["Product Info"].ToString();
                                var supplierInfo = row["Supplier Info"].ToString();
                                int productId = Convert.ToInt32(productInfo.Split(new string[] { "(ID:" }, StringSplitOptions.None)[1].Replace(")", ""));
                                int quantity = Convert.ToInt32(row["Quantity"]);
                                decimal costPriceAtOrder = Convert.ToDecimal(row["Cost Price"]);

                                // Check if the combination already exists
                                string checkExistenceQuery = "SELECT COUNT(*) FROM SupplierOrderDetails WHERE supplier_order_id = @supplierOrderId AND product_id = @productId AND cost_price_at_order = @costPriceAtOrder";
                                using (SqlCommand checkCmd = new SqlCommand(checkExistenceQuery, connection, transaction))
                                {
                                    checkCmd.Parameters.AddWithValue("@supplierOrderId", supplierOrderId);
                                    checkCmd.Parameters.AddWithValue("@productId", productId);
                                    checkCmd.Parameters.AddWithValue("@costPriceAtOrder", costPriceAtOrder);
                                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                                    if (count > 0)
                                    {
                                        // Duplicate entry detected, rollback the transaction
                                        transaction.Rollback();
                                        MessageBox.Show($"Duplicate entry detected for Product ID {productId} in Order ID {supplierOrderId}. No data has been saved. Please edit your order and try again.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return; // Exit the method and return to the form
                                    }
                                }

                                // Insert the order detail
                                using (SqlCommand cmdDetails = new SqlCommand(insertOrderDetailsQuery, connection, transaction))
                                {
                                    cmdDetails.Parameters.AddWithValue("@supplierOrderId", supplierOrderId);
                                    cmdDetails.Parameters.AddWithValue("@productId", productId);
                                    cmdDetails.Parameters.AddWithValue("@quantity", quantity);
                                    cmdDetails.Parameters.AddWithValue("@costPriceAtOrder", costPriceAtOrder);

                                    cmdDetails.ExecuteNonQuery();
                                }

                                // Update stock quantity in the Product table
                                string updateStockQuery = "UPDATE Product SET stock_quantity = stock_quantity + @quantity WHERE product_id = @productId";
                                using (SqlCommand updateCmd = new SqlCommand(updateStockQuery, connection, transaction))
                                {
                                    updateCmd.Parameters.AddWithValue("@quantity", quantity);
                                    updateCmd.Parameters.AddWithValue("@productId", productId);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        // Commit the transaction if no duplicates are found
                        transaction.Commit();
                        MessageBox.Show("Order saved successfully and stock updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of any error
                        transaction.Rollback();
                        MessageBox.Show($"Database Error\n\n{ex.Message}\n\nNo data has been saved. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            clearing();
        }



        private int GetLastSupplierOrderId(SqlConnection connection)
        {
            string query = "SELECT MAX(supplier_order_id) FROM SupplierOrder";
            SqlCommand command = new SqlCommand(query, connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();

            form1.Show();

            this.Hide();
        }

        private void UpdateTotalAmount(object sender, EventArgs e)
        {
            if (orderDataTable.Rows.Count > 0)
            {
                decimal totalAmount = 0;

                foreach (DataRow row in orderDataTable.Rows)
                {
                    // Check if the value is DBNull and then safely cast to decimal
                    if (row["Total Amount"] != DBNull.Value)
                    {
                        totalAmount += Convert.ToDecimal(row["Total Amount"]);
                    }
                }

                lblTotalAmount.Text = $"Total Amount: {totalAmount:C}"; // Formats as currency
            }
            else
            {
                lblTotalAmount.Text = "Total Amount: $0.00";
            }
        }
        private void BtnClear_Click(object sender, EventArgs e)
        {
            clearing();
        }

        private void clearing()
        {
            LoadProducts();
            lstSuppliers.Items.Clear();  // Clear suppliers as well
            txtCostPrice.Clear();
            numericUpDownQuantity.Value = 0;
            lblStockQuantity.Text = string.Empty;
            lblTotalAmount.Text = string.Empty;
            orderDataTable.Clear();  // Clears the order DataGridView as well
        }

        private void btnAddToOrder_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (lstProducts.SelectedItem == null || lstSuppliers.SelectedItem == null)
            {
                MessageBox.Show("Please select a product and a supplier.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (numericUpDownQuantity.Value <= 0)
            {
                MessageBox.Show("Quantity must be greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the selected product and supplier
            var selectedProduct = (dynamic)lstProducts.SelectedItem;
            var selectedSupplier = (dynamic)lstSuppliers.SelectedItem;

            int productId = selectedProduct.Value;   // Product ID
            string productName = selectedProduct.Text;  // Product Name
            int supplierId = selectedSupplier.Value; // Supplier ID
            string supplierName = selectedSupplier.Text; // Supplier Name

            decimal costPrice = decimal.Parse(txtCostPrice.Text);
            int quantity = (int)numericUpDownQuantity.Value;

            // Calculate total amount
            decimal totalAmount = costPrice * quantity;

            // Get selected date from DateTimePicker
            string orderDate = dateTimePickerOrderDate.Value.ToString("yyyy-MM-dd"); // Format: YYYY-MM-DD

            // Format product and supplier info
            string productInfo = $"{productName} (ID: {productId})";
            string supplierInfo = $"{supplierName} (ID: {supplierId})";

            // Add data to the DataTable with separate columns
            orderDataTable.Rows.Add(productInfo, supplierInfo, quantity, costPrice, totalAmount, orderDate);

            // Update the total amount label
            UpdateTotalAmount(sender, e);
        }

        private void btnRemoveCustomer_Click(object sender, EventArgs e)
        {
            if (dataGridViewOrder.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridViewOrder.SelectedRows)
                {
                    dataGridViewOrder.Rows.Remove(row);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to remove.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            UpdateTotalAmount(sender, e);
        }
    }
}     
    


