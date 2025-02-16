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

            // Wire up the CellValidating event
            dataGridViewSale.CellValidating += dataGridViewSale_CellValidating;
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
        private void dataGridViewSale_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridViewSale.Columns[e.ColumnIndex].Name == "SellingPrice")
            {
                string input = e.FormattedValue.ToString();

                // Check if the input is a valid positive number
                if (!decimal.TryParse(input, out decimal sellingPrice) || sellingPrice < 0)
                {
                    MessageBox.Show("Please enter a valid positive number for the selling price.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // Cancel the edit
                }
            }
        }
        private void Customer_Order_With_barcode_Load(object sender, EventArgs e)
        {
            dateTimePickerOrderDate.Value = DateTime.Now;
            LoadCustomers();
        }
        private void UpdateProductStockQuantity(int productId, int quantitySold)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Product SET stock_quantity = CASE " +
                               "WHEN stock_quantity - @quantity >= 0 THEN stock_quantity - @quantity " +
                               "ELSE 0 END WHERE product_id = @productId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@quantity", quantitySold);
                command.Parameters.AddWithValue("@productId", productId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating stock quantity: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private int GetProductStockQuantity(int productId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT stock_quantity FROM Product WHERE product_id = @productId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@productId", productId);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving stock quantity: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
            }
        }

        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string barcode = txtBarcode.Text.Trim();
                if (!string.IsNullOrEmpty(barcode))
                {
                    AddOrUpdateProductInSale(barcode);
                }
                txtBarcode.Clear();
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

            // Check if any item has a quantity greater than available stock
            foreach (var item in saleItems)
            {
                int availableStock = GetProductStockQuantity(item.ProductId); // Function to get stock from DB
                if (item.Quantity > availableStock)
                {
                    MessageBox.Show($"The quantity for {item.ProductName} exceeds available stock ({availableStock} available).",
                                    "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Stop execution if any item exceeds stock
                }
            }

            // Get the selected customer ID
            int customerId = ((CustomerItem)listBoxCustomers.SelectedItem).CustomerId;

            // Get the selected date from the DateTimePicker
            DateTime orderDate = dateTimePickerOrderDate.Value;

            // Calculate Totals (Applying VAT First, Then Discount)
            decimal totalBeforeVAT = saleItems.Sum(item => item.SellingPrice * item.Quantity);
            decimal vatAmount = totalBeforeVAT * 0.11m;  // VAT is 11%
            decimal totalAfterVAT = totalBeforeVAT + vatAmount;
            decimal totalDiscount = totalAfterVAT * (saleItems.Any() ? saleItems.Average(item => item.Discount) / 100 : 0);
            decimal totalAmount = totalAfterVAT - totalDiscount; // Final Total

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Insert into CustomerOrder table
                    string orderQuery = @"
                INSERT INTO CustomerOrder (customer_id, order_date, total_amount, vat_amount, discount_amount)
                VALUES (@customerId, @orderDate, @totalAmount, @vat, @discount);
                SELECT SCOPE_IDENTITY();";

                    SqlCommand orderCommand = new SqlCommand(orderQuery, connection, transaction);
                    orderCommand.Parameters.AddWithValue("@customerId", customerId);
                    orderCommand.Parameters.AddWithValue("@orderDate", orderDate);
                    orderCommand.Parameters.AddWithValue("@totalAmount", totalAmount);
                    orderCommand.Parameters.AddWithValue("@vat", vatAmount);
                    orderCommand.Parameters.AddWithValue("@discount", totalDiscount);

                    int customerOrderId = Convert.ToInt32(orderCommand.ExecuteScalar());

                    // Insert into CustomerOrderDetails table
                    foreach (var item in saleItems)
                    {
                        string detailsQuery = @"
                    INSERT INTO CustomerOrderDetails 
                    (customer_order_id, product_id, quantity, selling_price_at_order, discount) 
                    VALUES (@customerOrderId, @productId, @quantity, @sellingPrice, @discount)";

                        SqlCommand detailsCommand = new SqlCommand(detailsQuery, connection, transaction);
                        detailsCommand.Parameters.AddWithValue("@customerOrderId", customerOrderId);
                        detailsCommand.Parameters.AddWithValue("@productId", item.ProductId);
                        detailsCommand.Parameters.AddWithValue("@quantity", item.Quantity);
                        detailsCommand.Parameters.AddWithValue("@sellingPrice", item.SellingPrice);
                        detailsCommand.Parameters.AddWithValue("@discount", item.Discount);

                        detailsCommand.ExecuteNonQuery();
                    }

                    // Commit the transaction (save changes)
                    transaction.Commit();

                    MessageBox.Show("Sale completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear the form
                    Btnclear_Click(sender, e);
                }
                catch (Exception ex)
                {
                    // Rollback transaction in case of error
                    transaction.Rollback();
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void AddProductToSale(string barcode)
        {
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
                        int productId = reader.GetInt32(0);
                        string productName = reader.GetString(1);
                        decimal sellingPrice = reader.GetDecimal(2);
                        

                        var existingSaleItem = saleItems.FirstOrDefault(item => item.ProductId == productId);
                        if (existingSaleItem != null)
                        {
                            existingSaleItem.Quantity += 1;
                        }
                        else
                        {
                            saleItems.Add(new SaleItem
                            {
                                ProductId = productId,
                                ProductName = productName,
                                Quantity = 1,
                                SellingPrice = sellingPrice,
                                
                            });
                        }

                        RefreshDataGridView();
                    }
                    else
                    {
                        MessageBox.Show("Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var row = dataGridViewSale.Rows[e.RowIndex];
                int productId = (int)row.Cells["ProductId"].Value;

                var saleItem = saleItems.Find(item => item.ProductId == productId);
                if (saleItem != null)
                {
                    // Handle editing the Discount column
                    if (dataGridViewSale.Columns[e.ColumnIndex].Name == "Discount")
                    {
                        string discountValue = row.Cells["Discount"].Value.ToString();

                        // Check if the discount value is a valid decimal number
                        if (!decimal.TryParse(discountValue, out decimal discount) || discount < 0m || discount > 100m)
                        {
                            MessageBox.Show("Please enter a valid number for the discount (0-100).", "Invalid Discount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            row.Cells["Discount"].Value = saleItem.Discount; // Revert to the old value
                            return;
                        }

                        saleItem.Discount = discount; // Update discount in saleItems
                    }

                    // Handle editing the Quantity column
                    if (dataGridViewSale.Columns[e.ColumnIndex].Name == "Quantity")
                    {
                        string quantityValue = row.Cells["Quantity"].Value.ToString();

                        // Ensure the quantity is a valid integer
                        if (!int.TryParse(quantityValue, out int newQuantity) || newQuantity < 0)
                        {
                            MessageBox.Show("Quantity must be a non-negative integer.", "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            row.Cells["Quantity"].Value = saleItem.Quantity; // Revert to the old value
                            return;
                        }

                        // Check if the new quantity exceeds the available stock
                        int availableStock = GetProductStockQuantity(productId);
                        if (newQuantity > availableStock)
                        {
                            MessageBox.Show("Quantity exceeds available stock!", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            row.Cells["Quantity"].Value = saleItem.Quantity; // Revert to the old value
                            return;
                        }

                        saleItem.Quantity = newQuantity;
                        UpdateProductStockQuantity(productId, newQuantity);
                    }

                    // Handle editing the SellingPrice column
                    if (dataGridViewSale.Columns[e.ColumnIndex].Name == "SellingPrice")
                    {
                        string sellingPriceValue = row.Cells["SellingPrice"].Value.ToString();

                        // Ensure the selling price is a valid decimal number
                        if (!decimal.TryParse(sellingPriceValue, out decimal sellingPrice) || sellingPrice < 0m)
                        {
                            MessageBox.Show("Please enter a valid number for the selling price.", "Invalid Price", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            row.Cells["SellingPrice"].Value = saleItem.SellingPrice; // Revert to the old value
                            return;
                        }

                        saleItem.SellingPrice = sellingPrice;
                    }

                    // Refresh the DataGridView after validation
                    BeginInvoke((MethodInvoker)delegate
                    {
                        RefreshDataGridView();
                    });
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
            BeginInvoke((MethodInvoker)delegate
            {
                dataGridViewSale.DataSource = null;
                dataGridViewSale.DataSource = saleItems;
                dataGridViewSale.Refresh();

                if (dataGridViewSale.Columns.Count > 0)
                {
                    dataGridViewSale.Columns["ProductId"].ReadOnly = true;
                    dataGridViewSale.Columns["ProductName"].ReadOnly = true;
                    dataGridViewSale.Columns["TotalPrice"].ReadOnly = true;
                    dataGridViewSale.Columns["VAT"].ReadOnly = true;
                }
            });
            // Calculate the subtotal (total before VAT)
            decimal totalBeforeVAT = saleItems.Sum(item => item.SellingPrice * item.Quantity);

            // Calculate the VAT (11% of the subtotal before discount)
            decimal totalVAT = totalBeforeVAT * 0.11m;

            // Calculate the price after VAT
            decimal totalAfterVAT = totalBeforeVAT + totalVAT;

            // Calculate the discount on the price after VAT
            decimal totalDiscount = totalAfterVAT * (saleItems.Any() ? saleItems.Average(item => item.Discount) / 100 : 0);

            // Calculate the total after applying discount to the price after VAT
            decimal totalAfterDiscount = totalAfterVAT - totalDiscount;

            // Final total amount after applying VAT and discount
            totalAmount = totalAfterDiscount;

            // Update the label with the formatted amounts
            lblTotalAmount.Text = $"Subtotal: {totalBeforeVAT:C}\nVAT (11%): +{totalVAT:C}\nDiscount: -{totalDiscount:C}\nTotal: {totalAmount:C}";

        }


        private class SaleItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal SellingPrice { get; set; }
            public decimal Discount { get; set; }  // Discount in percentage
            public decimal VAT { get; set; } = 11; // Default VAT is 11%
                                                   // Read-Only Property (No Setter)
            public decimal TotalPrice
            {
                get
                {
                    // Calculate the price before VAT and discount
                    decimal priceBeforeVAT = Quantity * SellingPrice;

                    // Calculate the VAT (11% by default)
                    decimal vatAmount = priceBeforeVAT * (VAT / 100);

                    // Calculate the total price including VAT
                    decimal priceAfterVAT = priceBeforeVAT + vatAmount;

                    // Apply the discount (assuming Discount is in percentage)
                    decimal discountAmount = priceAfterVAT * (Discount / 100);

                    // Calculate the final total price after VAT and discount
                    return priceAfterVAT - discountAmount;
                }
            }

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
        private void AddOrUpdateProductInSale(string barcode)
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
                                    SellingPrice = sellingPrice
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
        private void ProcessScannedBarcode(string barcode)
        {
            AddOrUpdateProductInSale(barcode);
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
