using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TobaccoStore
{
    public partial class Customer_Order : Form
    {
        public Customer_Order()
        {

            InitializeComponent();
        }

        private Dictionary<int, int> availableStockDictionary = new Dictionary<int, int>();

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=Tabacoostore;Trusted_Connection=True;";
        private void Customer_Order_Load(object sender, EventArgs e)
        {
            LoadCustomerData();
            LoadProductData();
            LoadCustomers();
            LoadStockQuantities();

            dgvOrderDetails.Columns.Add("CustomerInfo", "Customer Info");
            dgvOrderDetails.Columns.Add("ProductName", "ProductName ");
            dgvOrderDetails.Columns.Add("Quantity", "Quantity");
            dgvOrderDetails.Columns.Add("SellingPrice", "Selling Price");
            dgvOrderDetails.Columns.Add("TotalPrice", "Total Price");

            dgvOrderDetails.Columns.Add("OrderDate", "Order Date");

        }

        private void LoadStockQuantities()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT product_id, stock_quantity FROM Product";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int productId = reader.GetInt32(0);
                    int stockQuantity = reader.GetInt32(1);
                    availableStockDictionary[productId] = stockQuantity;
                }

                reader.Close();
            }
        }
        // Declare a list to store all customers
        List<KeyValuePair<int, string>> allCustomers = new List<KeyValuePair<int, string>>();

        // Method to load customers from the database or source
        private void LoadCustomers()
        {
            allCustomers.Clear(); // Clear previous data

            DataTable dt = GetCustomersFromDatabase(); // Fetch customers from DB

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["customer_id"]);
                string fullName = row["fname"].ToString() + " " + row["lname"].ToString(); // Combine first & last name

                allCustomers.Add(new KeyValuePair<int, string>(id, fullName));
            }

            // Bind the full list to the ListBox
            lstCustomer.DataSource = new BindingSource(allCustomers, null);
            lstCustomer.DisplayMember = "Value"; // Show full name
            lstCustomer.ValueMember = "Key"; // Store customer ID
        }

        private void ClearForm()
        {
            // Reset product selection
            lstProduct.ClearSelected();

            // Reset customer selection
            lstCustomer.ClearSelected();

            // Clear price and quantity fields
            txtPrice.Text = "";
            numQuantity.Value = 0; // Reset to default

            txtSearchCustomer.Text = "";

            // Clear total price
            lblTotalAmount.Text = "$0.00";

            // Clear the DataGridView
            dgvOrderDetails.Rows.Clear();

            lblStockQuantity.Text = "";
        }

        private int GetProductIdByName(string productName)
        {
            // Extract only the product name (remove "(ID: x)")
            int index = productName.IndexOf(" (ID:");
            if (index != -1)
            {
                productName = productName.Substring(0, index).Trim();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT product_id FROM Product WHERE LOWER(product_name) = LOWER(@productName)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@productName", productName);

                    object result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int productId))
                    {
                        return productId;
                    }
                    else
                    {
                        MessageBox.Show($"Product '{productName}' not found in the database.");
                        return -1;
                    }
                }
            }
        }

        private void CalculateTotalAmount()
        {
            decimal totalAmount = 0;

            // Loop through all rows in the DataGridView to calculate the total
            foreach (DataGridViewRow row in dgvOrderDetails.Rows)
            {
                if (row.Cells["TotalPrice"].Value != null)
                {
                    string value = row.Cells["TotalPrice"].Value.ToString();

                    if (decimal.TryParse(value, System.Globalization.NumberStyles.Currency,
                                         System.Globalization.CultureInfo.CurrentCulture,
                                         out decimal parsedValue))
                    {
                        totalAmount += parsedValue;
                    }
                }
            }

            // Display the total amount in a Label
            lblTotalAmount.Text = totalAmount.ToString("C2"); // Format as currency
        }

        private void LoadCustomerData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT customer_id, CONCAT(fname, ' ', lname) AS customer_name FROM Customer", connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    lstCustomer.Items.Add(new KeyValuePair<int, string>(
                        Convert.ToInt32(reader["customer_id"]),
                        reader["customer_name"].ToString()
                    ));
                }
            }
        }

        private void LoadProductData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT product_id, product_name, selling_price FROM Product", connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    lstProduct.Items.Add(new KeyValuePair<int, string>(
                        Convert.ToInt32(reader["product_id"]),
                        reader["product_name"].ToString()
                    ));
                }

                
            }
        }

        private decimal GetProductPriceById(int productId)
        {
            decimal price = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT selling_price FROM Product WHERE product_id = @product_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@product_id", productId);

                var result = command.ExecuteScalar();
                if (result != null)
                {   
                    price = Convert.ToDecimal(result);
                }
            }
            return price;
        }


        private void lstProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstProduct.SelectedItem is KeyValuePair<int, string> selectedProduct)
            {
                int productId = selectedProduct.Key;
                string productName = selectedProduct.Value;

                // Fetch the price from the database or a cached list
                decimal price = GetProductPriceById(productId);
                txtPrice.Text = price.ToString("C2");

                int stockQuantity = GetStockQuantityById(productId);
                lblStockQuantity.Text = $"Stock: {stockQuantity}";
            }
        }

        private int GetStockQuantityById(int productId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT stock_quantity FROM Product WHERE product_id = @product_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@product_id", productId);
                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();

            form1.Show();

            this.Hide();
        }

        private void btnAddToOrder_Click(object sender, EventArgs e)
        {
            if (lstProduct.SelectedItem is KeyValuePair<int, string> selectedProduct)
            {
                int productId = selectedProduct.Key;
                int quantity = (int)numQuantity.Value;

                // Check if the quantity is 0
                if (quantity == 0)
                {
                    MessageBox.Show("Quantity cannot be 0. Please enter a valid quantity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get available stock from memory
                if (!availableStockDictionary.TryGetValue(productId, out int availableStock))
                {
                    MessageBox.Show("Product stock not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validate stock availability
                if (quantity > availableStock)
                {
                    MessageBox.Show($"Not enough stock available!\nAvailable: {availableStock}, Requested: {quantity}", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get price
                if (!decimal.TryParse(txtPrice.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal price))
                {
                    MessageBox.Show("Invalid price! Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                decimal total = price * quantity;
                string productInfo = $"{selectedProduct.Value} (ID: {productId})";

                if (lstCustomer.SelectedItem is KeyValuePair<int, string> selectedCustomer)
                {
                    string customerInfo = $"{selectedCustomer.Value} (ID: {selectedCustomer.Key})";
                    string orderDate = dateTimePickerOrderDate.Value.ToString("yyyy-MM-dd");

                    // Add order to DataGridView
                    dgvOrderDetails.Rows.Add(customerInfo, productInfo, quantity, price.ToString("C2"), total.ToString("C2"), orderDate);

                    // Update stock quantity in memory (not in database)
                    availableStockDictionary[productId] -= quantity;

                    // Update stock label
                    lblStockQuantity.Text = $"Stock: {availableStockDictionary[productId]}";
                }

                CalculateTotalAmount();
            }
        }
        private void btnSaveOrder_Click(object sender, EventArgs e)
        {
            // Check if a customer is selected
            if (lstCustomer.SelectedItem == null)
            {
                MessageBox.Show("Please select a customer before saving the order.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if there are valid products in the order
            bool hasProducts = false;
            foreach (DataGridViewRow row in dgvOrderDetails.Rows)
            {
                if (row.Cells["ProductName"].Value != null && !string.IsNullOrWhiteSpace(row.Cells["ProductName"].Value.ToString()))
                {
                    hasProducts = true;
                    break;
                }
            }

            if (!hasProducts)
            {
                MessageBox.Show("Please add at least one product to the order before saving.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if there are any products in the order
            if (dgvOrderDetails.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one product to the order before saving.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Your existing save logic here...
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Insert into CustomerOrder Table
                    string insertOrderQuery = "INSERT INTO CustomerOrder (customer_id, order_date, total_amount) OUTPUT INSERTED.customer_order_id VALUES (@customer_id, @order_date, @total_amount)";
                    SqlCommand insertOrderCommand = new SqlCommand(insertOrderQuery, connection);
                    insertOrderCommand.Parameters.AddWithValue("@customer_id", ((KeyValuePair<int, string>)lstCustomer.SelectedItem).Key);
                    insertOrderCommand.Parameters.AddWithValue("@order_date", dateTimePickerOrderDate.Value);
                    insertOrderCommand.Parameters.AddWithValue("@total_amount", decimal.Parse(lblTotalAmount.Text.Replace("$", "")));

                    int orderId = (int)insertOrderCommand.ExecuteScalar();

                    // Insert into CustomerOrderDetails Table
                    foreach (DataGridViewRow row in dgvOrderDetails.Rows)
                    {
                        if (row.Cells["ProductName"].Value != null)
                        {
                            string productName = row.Cells["ProductName"].Value.ToString();
                            int productId = GetProductIdByName(productName);

                            if (productId == -1)
                            {
                                MessageBox.Show($"Product '{productName}' not found in the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                continue;
                            }

                            int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                            decimal priceAtOrder = Convert.ToDecimal(row.Cells["SellingPrice"].Value.ToString().Replace("$", ""));

                            string insertOrderDetailsQuery = "INSERT INTO CustomerOrderDetails (customer_order_id, product_id, quantity, selling_price_at_order) VALUES (@customer_order_id, @product_id, @quantity, @selling_price_at_order)";
                            SqlCommand insertOrderDetailsCommand = new SqlCommand(insertOrderDetailsQuery, connection);
                            insertOrderDetailsCommand.Parameters.AddWithValue("@customer_order_id", orderId);
                            insertOrderDetailsCommand.Parameters.AddWithValue("@product_id", productId);
                            insertOrderDetailsCommand.Parameters.AddWithValue("@quantity", quantity);
                            insertOrderDetailsCommand.Parameters.AddWithValue("@selling_price_at_order", priceAtOrder);

                            insertOrderDetailsCommand.ExecuteNonQuery();

                            // Update stock quantity in Product Table
                            string updateProductQuery = "UPDATE Product SET stock_quantity = stock_quantity - @quantity WHERE product_id = @product_id";
                            SqlCommand updateProductCommand = new SqlCommand(updateProductQuery, connection);
                            updateProductCommand.Parameters.AddWithValue("@quantity", quantity);
                            updateProductCommand.Parameters.AddWithValue("@product_id", productId);

                            updateProductCommand.ExecuteNonQuery();
                        }
                    }

                    // Now update the stock quantities using availableStockDictionary
                    foreach (var entry in availableStockDictionary)
                    {
                        int productId = entry.Key;
                        int newStock = entry.Value;

                        string query = "UPDATE Product SET stock_quantity = @stock_quantity WHERE product_id = @product_id";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@stock_quantity", newStock);
                        command.Parameters.AddWithValue("@product_id", productId);
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Order saved and stock updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm(); // Clear the form after saving
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only numbers, backspace, and one decimal point
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true; // Ignore the key press
            }

            // Ensure only one decimal point is entered
            if (e.KeyChar == '.' && txtPrice.Text.Contains("."))
            {
                e.Handled = true; // Ignore the key press
            }
        }

        private void btnRemoveCustomer_Click(object sender, EventArgs e)
        {
            if (dgvOrderDetails.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvOrderDetails.SelectedRows)
                {
                    dgvOrderDetails.Rows.Remove(row);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to remove.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Update total amount after removal
            CalculateTotalAmount();
        }

        private void txtSearchCustomer_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearchCustomer.Text.ToLower();

            var filteredCustomers = allCustomers
                .Where(c => c.Value.ToLower().Contains(searchText))
                .ToList();

            lstCustomer.DataSource = new BindingSource(filteredCustomers, null);
            lstCustomer.DisplayMember = "Value";
            lstCustomer.ValueMember = "Key";
        }


        private DataTable GetCustomersFromDatabase()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT customer_id, fname, lname FROM Customer";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
            }

            return dt;
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}
