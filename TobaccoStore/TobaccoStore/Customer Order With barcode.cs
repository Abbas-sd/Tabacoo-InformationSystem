using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TobaccoStore
{
    public partial class Customer_Order_With_barcode : Form
    {
        private bool errorShown = false;

        private System.Windows.Forms.Timer barcodeTimer;
        private string scannedBarcode = "";

        public Customer_Order_With_barcode()
        {
            InitializeComponent();
            // Wire up events
            this.Shown += new EventHandler(SalesForm_Shown);
            txtBarcode.KeyPress += new KeyPressEventHandler(txtBarcode_KeyPress);
            dataGridViewSale.CellEndEdit += new DataGridViewCellEventHandler(dataGridViewSale_CellEndEdit);
            dataGridViewSale.UserDeletingRow += new DataGridViewRowCancelEventHandler(dataGridViewSale_UserDeletingRow);
            txtSearchCustomer.TextChanged += new EventHandler(txtSearchCustomer_TextChanged); // Add this line
                                                                                              // Timer Setup
            barcodeTimer = new System.Windows.Forms.Timer();
            barcodeTimer.Interval = 300; // Adjust timing if needed
            barcodeTimer.Tick += BarcodeTimer_Tick;

            txtBarcode.KeyPress += new KeyPressEventHandler(txtBarcode_KeyPress);
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
                // Process the full barcode after scan
                string barcode = txtBarcode.Text.Trim();

                // Proceed if barcode is not empty
                if (!string.IsNullOrEmpty(barcode))
                {
                    AddProductToSale(barcode);
                }

                // Clear the barcode field after processing
                txtBarcode.Clear();

                // Suppress the beep sound (optional depending on your scanner)
                e.Handled = true;
            }
        }



        private void BarcodeTimer_Tick(object sender, EventArgs e)
        {
            barcodeTimer.Stop(); // Stop timer after delay

            if (!string.IsNullOrEmpty(scannedBarcode))
            {
                ProcessScannedBarcode(scannedBarcode.Trim());
                scannedBarcode = ""; // Reset buffer
                txtBarcode.Clear();
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
            if (listBoxCustomers.SelectedItem == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the selected customer ID
            int customerId = ((CustomerItem)listBoxCustomers.SelectedItem).CustomerId;

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
            errorShown = false;  // Reset error flag for each scan attempt

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

                        // Check if the product already exists in the sale list
                        var existingSaleItem = saleItems.FirstOrDefault(item => item.ProductId == productId);

                        if (existingSaleItem != null)
                        {
                            // If the product exists, update the quantity
                            existingSaleItem.Quantity += 1;

                            // Recalculate the total price
                            existingSaleItem.TotalPrice = existingSaleItem.Quantity * existingSaleItem.SellingPrice;
                        }
                        else
                        {
                            // If the product doesn't exist, add it as a new item
                            saleItems.Add(new SaleItem
                            {
                                ProductId = productId,
                                ProductName = productName,
                                Quantity = 1, // Start with quantity 1 for the new item
                                SellingPrice = sellingPrice,
                                TotalPrice = sellingPrice // Total price for this row
                            });
                        }

                        // Refresh the DataGridView
                        RefreshDataGridView();
                    }
                    else
                    {
                        // Product not found
                        if (!errorShown) // Prevent multiple errors
                        {
                            MessageBox.Show("Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            errorShown = true; // Mark that the error has been shown
                        }
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
            barcodeTimer.Stop(); // Reset the timer
            scannedBarcode = txtBarcode.Text.Trim(); // Store current barcode
            barcodeTimer.Start(); // Restart timer
        }

        private void ProcessScannedBarcode(string barcode)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT product_id, product_name, selling_price, stock_quantity FROM Product WHERE barcode = @barcode";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@barcode", barcode);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int productId = reader.GetInt32(0);
                    string productName = reader.GetString(1);
                    decimal sellingPrice = reader.GetDecimal(2);
                    int stockQuantity = reader.GetInt32(3);

                    var existingItem = saleItems.FirstOrDefault(item => item.ProductId == productId);
                    if (existingItem != null)
                    {
                        if (existingItem.Quantity < stockQuantity)
                        {
                            existingItem.Quantity += 1;
                            existingItem.TotalPrice = existingItem.Quantity * existingItem.SellingPrice;
                        }
                        else
                        {
                            MessageBox.Show("Not enough stock!", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        if (stockQuantity > 0)
                        {
                            saleItems.Add(new SaleItem
                            {
                                ProductId = productId,
                                ProductName = productName,
                                Quantity = 1,
                                SellingPrice = sellingPrice,
                                TotalPrice = sellingPrice
                            });
                        }
                        else
                        {
                            MessageBox.Show("This product is out of stock!", "Stock Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    RefreshDataGridView();
                }
                else
                {
                    if (!errorShown) // Prevent multiple errors
                    {
                        MessageBox.Show("Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        errorShown = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
        private void LoadCustomers(string searchText = "")
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT customer_id, fname, lname FROM Customer WHERE fname LIKE @searchText OR lname LIKE @searchText";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@searchText", $"%{searchText}%");

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Clear the ListBox
                    listBoxCustomers.Items.Clear();

                    // Add customers to the ListBox
                    while (reader.Read())
                    {
                        int customerId = reader.GetInt32(0);
                        string customerName = reader.GetString(1) + " " + reader.GetString(2);
                        listBoxCustomers.Items.Add(new CustomerItem { CustomerId = customerId, CustomerName = customerName });
                    }

                    // Display the customer name in the ListBox
                    listBoxCustomers.DisplayMember = "CustomerName";
                    listBoxCustomers.ValueMember = "CustomerId";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Class to represent a customer item in the ListBox
        private class CustomerItem
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
        }

        private void txtSearchCustomer_TextChanged(object sender, EventArgs e)
        {
            // Reload customers based on the search text
            LoadCustomers(txtSearchCustomer.Text);
        }

        private void btnremove_Click(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (dataGridViewSale.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataGridViewSale.SelectedRows[0];

                // Get the product ID from the selected row
                int productId = (int)selectedRow.Cells["ProductId"].Value;

                // Find the sale item in the saleItems list
                var saleItem = saleItems.FirstOrDefault(item => item.ProductId == productId);

                if (saleItem != null)
                {
                    // Remove the sale item from the list
                    saleItems.Remove(saleItem);

                    // Refresh the DataGridView
                    RefreshDataGridView();
                }
            }
            else
            {
                // If no row is selected, show a warning
                MessageBox.Show("Please select a row to remove.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnManualEntry_Click(object sender, EventArgs e)
        {
            string manualBarcode = txtManualBarcode.Text.Trim();

            if (!string.IsNullOrEmpty(manualBarcode))
            {
                AddProductToSale(manualBarcode); // Process the manually entered barcode
                txtManualBarcode.Clear(); // Clear after entry
            }
            else
            {
                MessageBox.Show("Please enter a valid barcode.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void btnManualEntry_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
        private void txtManualBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // If Enter key is pressed
            {
                string manualBarcode = txtManualBarcode.Text.Trim(); // Get barcode text

                if (!string.IsNullOrEmpty(manualBarcode))
                {
                    AddProductToSale(manualBarcode); // Process barcode directly
                    txtManualBarcode.Clear(); // Clear textbox after processing
                    txtManualBarcode.Focus(); // Refocus for next input
                }

                e.SuppressKeyPress = true; // Prevent the "ding" sound
            }
        }

        private void btnback_Click(object sender, EventArgs e)
        {
            Main form4 = new Main();

            form4.Show();

            this.Hide();
        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void PrintInvoice()
        {
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();

            // Handle the PrintPage event to define how the content is printed
            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);

            // Set the printer settings (optional, use default printer if not set)
            printDialog.Document = printDocument;

            // Show the print dialog to the user
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (ValidateOrder())  // Check validation before proceeding
            {
                PrintInvoice();
            }
        }
        private bool ValidateOrder()
        {
            // Check if a customer is selected
            if (listBoxCustomers.SelectedItem == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Check if there are items in the sale
            if (saleItems.Count == 0)
            {
                MessageBox.Show("Please add items to the order.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            if (ValidateOrder())  // Check validation before proceeding
            {
                ShowPrintPreview();  // Only show preview if validation passed
            }
        }
        private void ShowPrintPreview()
        {
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            PrintDocument printDocument = new PrintDocument();

            // Handle the PrintPage event to define how the content is displayed
            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);

            // Set the Document property of PrintPreviewDialog to the printDocument
            printPreviewDialog.Document = printDocument;

            // Show the print preview dialog
            printPreviewDialog.ShowDialog();
        }
        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font headerFont = new Font("Arial", 14, FontStyle.Bold);
            Font bodyFont = new Font("Arial", 12);
            Brush brush = Brushes.Black;

            /*
            // Load the icon image (replace with your own image path)
            Image iconImage = Image.FromFile("C:\\Users\\abbas\\OneDrive\\Desktop\\images\\Tabacoo-Icon2.jpeg");
            int iconWidth = 150; // Adjust size of the icon if needed
            int iconHeight = 150; // Adjust size of the icon if needed
            int iconX = e.PageBounds.Width - iconWidth - 20; // Right margin
            int iconY = 50; // Top margin

            // Draw the icon at the top-right corner
            graphics.DrawImage(iconImage, iconX, iconY, iconWidth, iconHeight);
            */

            // Header
            graphics.DrawString("Customer Order Invoice", headerFont, brush, 100, 100);

            // Customer Info
            int yPos = 150; // Starting position for customer info
            if (listBoxCustomers.SelectedItem != null)
            {
                var selectedCustomer = listBoxCustomers.SelectedItem as CustomerItem;
                graphics.DrawString($"Customer: {selectedCustomer.CustomerName}", bodyFont, brush, 100, yPos);
            }

            else
            {
                graphics.DrawString("Customer: Not Selected", bodyFont, brush, 100, yPos);
            }

            // Order Date
            yPos += 50;
            graphics.DrawString($"Order Date & Time: {dateTimePickerOrderDate.Value.ToString("f")}", bodyFont, brush, 100, yPos);


            // Item List Header
            yPos += 40;
            graphics.DrawString("Product Name", bodyFont, brush, 100, yPos);
            graphics.DrawString("Quantity", bodyFont, brush, 300, yPos);
            graphics.DrawString("Price", bodyFont, brush, 400, yPos);
            graphics.DrawString("Total", bodyFont, brush, 500, yPos);

            // Item List
            yPos += 30;
            foreach (var item in saleItems)
            {
                graphics.DrawString(item.ProductName, bodyFont, brush, 100, yPos);
                graphics.DrawString(item.Quantity.ToString(), bodyFont, brush, 300, yPos);
                graphics.DrawString(item.SellingPrice.ToString("C"), bodyFont, brush, 400, yPos);
                graphics.DrawString(item.TotalPrice.ToString("C"), bodyFont, brush, 500, yPos);
                yPos += 30;
            }

            // Total Amount
            yPos += 20;
            graphics.DrawString($"Total Amount: {totalAmount:C}", headerFont, brush, 100, yPos);

            // End of Page
            e.HasMorePages = false; // Only one page
        }
    }
}
