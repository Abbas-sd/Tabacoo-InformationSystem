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
    public partial class Customer_Order_With_barcode : Form
    {

        

        public Customer_Order_With_barcode()
        {
            InitializeComponent();
            // Wire up events
            this.Shown += new EventHandler(SalesForm_Shown);
            txtBarcode.KeyPress += new KeyPressEventHandler(txtBarcode_KeyPress); // Use KeyPress event
            dataGridViewSale.CellEndEdit += new DataGridViewCellEventHandler(dataGridViewSale_CellEndEdit);
            dataGridViewSale.UserDeletingRow += new DataGridViewRowCancelEventHandler(dataGridViewSale_UserDeletingRow);
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=Tabacoostore;Trusted_Connection=True;";
        private List<SaleItem> saleItems = new List<SaleItem>(); // List to store sale items
        private decimal totalAmount = 0; // Total amount for the sale
        private string barcodeBuffer = ""; // Buffer to store barcode characters

        private void SalesForm_Shown(object sender, EventArgs e)
        {
            // Set focus to the barcode text box when the form is shown
            txtBarcode.Focus();
        }

        private void Customer_Order_With_barcode_Load(object sender, EventArgs e)
        {
            dateTimePickerOrderDate.Value = DateTime.Now;
            LoadCustomers();
        }

        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the Enter key (carriage return) is pressed
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Process the barcode
                AddProductToSale(barcodeBuffer);

                // Clear the buffer and the text box
                barcodeBuffer = "";
                txtBarcode.Clear();

                // Suppress the beep sound
                e.Handled = true;
            }
            else
            {
                // Append the character to the buffer
                barcodeBuffer += e.KeyChar;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Validate that there are items in the sale
            if (saleItems.Count == 0)
            {
                MessageBox.Show("No items in the sale.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate that a customer is selected
            if (comboBoxCustomers.SelectedItem == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the selected customer ID
            int customerId = ((CustomerItem)comboBoxCustomers.SelectedItem).CustomerId;

            // Get the selected date from the DateTimePicker
            DateTime orderDate = dateTimePickerOrderDate.Value;

            // Save the sale to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Insert into CustomerOrder table
                    string orderQuery = "INSERT INTO CustomerOrder (customer_id, order_date, total_amount) VALUES (@customerId, @orderDate, @totalAmount); SELECT SCOPE_IDENTITY();";
                    SqlCommand orderCommand = new SqlCommand(orderQuery, connection);
                    orderCommand.Parameters.AddWithValue("@customerId", customerId); // Use the selected customer ID
                    orderCommand.Parameters.AddWithValue("@orderDate", orderDate); // Use the selected date
                    orderCommand.Parameters.AddWithValue("@totalAmount", totalAmount);

                    // Get the generated customer_order_id
                    int customerOrderId = Convert.ToInt32(orderCommand.ExecuteScalar());

                    // Insert into CustomerOrderDetails table
                    foreach (var item in saleItems)
                    {
                        string detailsQuery = "INSERT INTO CustomerOrderDetails (customer_order_id, product_id, quantity, selling_price_at_order) VALUES (@customerOrderId, @productId, @quantity, @sellingPrice)";
                        SqlCommand detailsCommand = new SqlCommand(detailsQuery, connection);
                        detailsCommand.Parameters.AddWithValue("@customerOrderId", customerOrderId);
                        detailsCommand.Parameters.AddWithValue("@productId", item.ProductId);
                        detailsCommand.Parameters.AddWithValue("@quantity", item.Quantity);
                        detailsCommand.Parameters.AddWithValue("@sellingPrice", item.SellingPrice);
                        detailsCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Sale completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear the form
                    Btnclear_Click(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AddProductToSale(string barcode)
        {
            // Trim the barcode input
            barcode = barcode.Trim();

            // Debug: Print the scanned barcode
            Console.WriteLine("Scanned Barcode: " + barcode);

            // Fetch product details from the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT product_id, product_name, selling_price FROM Product WHERE barcode = @barcode";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@barcode", barcode);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        // Get product details
                        int productId = reader.GetInt32(0);
                        string productName = reader.GetString(1);
                        decimal sellingPrice = reader.GetDecimal(2);

                        // Add the item to the sale list (always add a new row)
                        saleItems.Add(new SaleItem
                        {
                            ProductId = productId,
                            ProductName = productName,
                            Quantity = 1, // Always set quantity to 1 for new rows
                            SellingPrice = sellingPrice,
                            TotalPrice = sellingPrice // Total price for this row
                        });

                        // Refresh the DataGridView
                        RefreshDataGridView();
                    }
                    else
                    {
                        // Product not found
                        MessageBox.Show("Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtBarcode.Clear(); // Clear the barcode field
                        txtBarcode.Focus(); // Set focus back to the barcode field
                        return; // Exit the method without adding the product
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void dataGridViewSale_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Update the sale item when a cell is edited
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var row = dataGridViewSale.Rows[e.RowIndex];
                int productId = (int)row.Cells["ProductId"].Value;

                // Find the sale item
                var saleItem = saleItems.Find(item => item.ProductId == productId);
                if (saleItem != null)
                {
                    // Update quantity or price
                    if (dataGridViewSale.Columns[e.ColumnIndex].Name == "Quantity")
                    {
                        int newQuantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                        saleItem.Quantity = newQuantity;
                    }
                    else if (dataGridViewSale.Columns[e.ColumnIndex].Name == "SellingPrice")
                    {
                        decimal newPrice = Convert.ToDecimal(row.Cells["SellingPrice"].Value);
                        saleItem.SellingPrice = newPrice;
                    }

                    // Recalculate the total price
                    saleItem.TotalPrice = saleItem.Quantity * saleItem.SellingPrice;

                    // Refresh the DataGridView
                    RefreshDataGridView();
                }
            }
        }
        private void dataGridViewSale_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            // Remove the item from the sale list when a row is deleted
            int productId = (int)e.Row.Cells["ProductId"].Value;
            var saleItem = saleItems.Find(item => item.ProductId == productId);
            if (saleItem != null)
            {
                saleItems.Remove(saleItem);
                RefreshDataGridView();
            }
        }

        
        private void RefreshDataGridView()
        {
            // Bind the sale items to the DataGridView
            dataGridViewSale.DataSource = null;
            dataGridViewSale.DataSource = saleItems;
            dataGridViewSale.Refresh();

            // Set read-only properties for specific columns
            if (dataGridViewSale.Columns.Count > 0)
            {
                dataGridViewSale.Columns["ProductId"].ReadOnly = true;
                dataGridViewSale.Columns["ProductName"].ReadOnly = true;
            }

            // Update the total amount label
            totalAmount = saleItems.Sum(item => item.TotalPrice);
            lblTotalAmount.Text = $"Total Amount: {totalAmount:C}";
        }

        private class SaleItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal SellingPrice { get; set; }
            public decimal TotalPrice { get; set; }
        }

        private void Btnclear_Click(object sender, EventArgs e)
        {
            // Clear the sale items and reset the form
            saleItems.Clear();
            totalAmount = 0;
            RefreshDataGridView();
            txtBarcode.Clear();
            txtBarcode.Focus();
        }

        private void listBoxCustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private async void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            await Task.Delay(150); // Add a 1-second delay

            // Rest of your code
            if (!string.IsNullOrEmpty(txtBarcode.Text))
            {
                AddProductToSale(txtBarcode.Text.Trim());
                txtBarcode.Clear();
            }
        }

        private void LoadCustomers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT customer_id, fname, lname FROM Customer";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Clear the ComboBox
                    comboBoxCustomers.Items.Clear();

                    // Add customers to the ComboBox
                    while (reader.Read())
                    {
                        int customerId = reader.GetInt32(0);
                        string customerName = reader.GetString(1) + " " + reader.GetString(2);
                        comboBoxCustomers.Items.Add(new CustomerItem { CustomerId = customerId, CustomerName = customerName });
                    }

                    // Display the customer name in the ComboBox
                    comboBoxCustomers.DisplayMember = "CustomerName";
                    comboBoxCustomers.ValueMember = "CustomerId";

                    // Select the first customer by default (if any)
                    if (comboBoxCustomers.Items.Count > 0)
                    {
                        comboBoxCustomers.SelectedIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Class to represent a customer item in the ComboBox
        private class CustomerItem
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
        }
    }
}
