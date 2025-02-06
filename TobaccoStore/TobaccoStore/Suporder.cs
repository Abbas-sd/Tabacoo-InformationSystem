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
            try
            {
                foreach (DataRow row in orderDataTable.Rows)
                {
                    // Extract data safely
                    string productInfo = row["Product Info"].ToString();
                    string supplierInfo = row["Supplier Info"].ToString();
                    string quantityStr = row["Quantity"].ToString();
                    string costPriceStr = row["Cost Price"].ToString();
                    string totalAmountStr = row["Total Amount"].ToString();
                    string dateStr = row["Date"].ToString();

                    // Ensure numeric fields are valid
                    if (!int.TryParse(quantityStr, out int quantity))
                    {
                        MessageBox.Show("Invalid quantity value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!decimal.TryParse(costPriceStr, out decimal costPrice))
                    {
                        MessageBox.Show("Invalid cost price value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!decimal.TryParse(totalAmountStr, out decimal totalAmount))
                    {
                        MessageBox.Show("Invalid total amount value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Convert date
                    DateTime orderDate;
                    if (!DateTime.TryParse(dateStr, out orderDate))
                    {
                        MessageBox.Show("Invalid date format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Insert data into the database (replace this with your actual query)
                    string query = "INSERT INTO Orders (ProductInfo, SupplierInfo, Quantity, CostPrice, TotalAmount, OrderDate) " +
                                   "VALUES (@ProductInfo, @SupplierInfo, @Quantity, @CostPrice, @TotalAmount, @OrderDate)";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProductInfo", productInfo);
                            cmd.Parameters.AddWithValue("@SupplierInfo", supplierInfo);
                            cmd.Parameters.AddWithValue("@Quantity", quantity);
                            cmd.Parameters.AddWithValue("@CostPrice", costPrice);
                            cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                            cmd.Parameters.AddWithValue("@OrderDate", orderDate);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Order saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            LoadProducts();
            lstSuppliers.Items.Clear();  // Clear suppliers as well
            txtCostPrice.Clear();
            numericUpDownQuantity.Value = 0;
            lblStockQuantity.Text = string.Empty;
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
    }
}     
    


