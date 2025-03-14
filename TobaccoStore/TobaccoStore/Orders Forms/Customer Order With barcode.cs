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

            // Disable the search button for Cashier role
            if (log_in.currentUserRole == Main.UserRole.Stoker)
            {
                button1.Enabled = false;  // Disable button1 (search button) for Cashier
                btnPrintPreview.Enabled = false;
                btnPrint.Enabled = false;
            }

            if (log_in.currentUserRole == Main.UserRole.User)
            {
                button1.Enabled = false;  // Disable button1 (search button) for Cashier
                btnPrintPreview.Enabled = false;
                btnPrint.Enabled = false;
            }
        }
        private void Customer_Order_With_barcode_Load(object sender, EventArgs e)
        {
            dateTimePickerOrderDate.Value = DateTime.Now;
            LoadCustomers();

            // Populate the payment status dropdown
            
            comboBoxPaymentStatus.Items.Add("Paid");
            comboBoxPaymentStatus.Items.Add("Partial");

            
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
        // Validate input before the user leaves the cell
        private void dataGridViewSale_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;

            var columnName = dataGridViewSale.Columns[e.ColumnIndex].Name;
            string input = e.FormattedValue.ToString();

            // Selling Price Validation
            if (columnName == "SellingPrice")
            {
                if (!decimal.TryParse(input, out decimal sellingPrice) || sellingPrice < 0)
                {
                    MessageBox.Show("Please enter a valid positive number for the selling price.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // ❌ Prevents moving to another cell
                }
            }

            // Quantity Validation
            if (columnName == "Quantity")
            {
                if (!int.TryParse(input, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Quantity must be a positive integer.", "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
        }
        private void dataGridViewSale_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var row = dataGridViewSale.Rows[e.RowIndex];
            int productId = Convert.ToInt32(row.Cells["ProductId"].Value);

            var saleItem = saleItems.Find(item => item.ProductId == productId);
            if (saleItem == null) return;

            string columnName = dataGridViewSale.Columns[e.ColumnIndex].Name;

            // Handle Quantity column changes
            if (columnName == "Quantity")
            {
                if (!int.TryParse(row.Cells["Quantity"].Value.ToString(), out int newQuantity) || newQuantity <= 0)
                {
                    MessageBox.Show("Quantity must be a positive integer.", "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row.Cells["Quantity"].Value = saleItem.Quantity; // ❌ Revert to old value
                    return;
                }

                // Check available stock
                int availableStock = GetProductStockQuantity(productId);
                if (newQuantity > availableStock)
                {
                    MessageBox.Show("Quantity exceeds available stock!", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row.Cells["Quantity"].Value = saleItem.Quantity; // ❌ Revert to old value
                    return;
                }

                saleItem.Quantity = newQuantity;
                UpdateStockQuantity(productId, newQuantity);
            }

            // Handle Discount column changes
            if (columnName == "Discount")
            {
                if (!decimal.TryParse(row.Cells["Discount"].Value.ToString(), out decimal discount) || discount < 0m || discount > 100m)
                {
                    MessageBox.Show("Please enter a valid discount (0-100).", "Invalid Discount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row.Cells["Discount"].Value = saleItem.Discount; // ❌ Revert to old value
                    return;
                }

                saleItem.Discount = discount;
            }

            // Handle Selling Price column changes
            if (columnName == "SellingPrice")
            {
                if (!decimal.TryParse(row.Cells["SellingPrice"].Value.ToString(), out decimal sellingPrice) || sellingPrice < 0m)
                {
                    MessageBox.Show("Please enter a valid selling price.", "Invalid Price", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row.Cells["SellingPrice"].Value = saleItem.SellingPrice; // ❌ Revert to old value
                    return;
                }

                saleItem.SellingPrice = sellingPrice;
            }

            // Refresh DataGridView to show updated totals
            BeginInvoke((MethodInvoker)delegate
            {
                RefreshDataGridView();
            });
        }

        private void UpdateStockQuantity(int productId, int quantitySold)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Product SET stock_quantity = stock_quantity - @quantitySold WHERE product_id = @productId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@quantitySold", quantitySold);
                command.Parameters.AddWithValue("@productId", productId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery(); // Execute the query to update the stock quantity
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

            // Validate that a payment status is selected
            if (comboBoxPaymentStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                // Ensure discount is between 0 and 100
                if (item.Discount < 0 || item.Discount > 100)
                {
                    MessageBox.Show($"Invalid discount for {item.ProductName}. Discount must be between 0 and 100.",
                                    "Discount Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Get the selected customer ID
            int customerId = ((CustomerItem)listBoxCustomers.SelectedItem).CustomerId;

            // Get the selected date from the DateTimePicker
            DateTime orderDate = dateTimePickerOrderDate.Value;

            // Step 1: Calculate the total price before VAT
            decimal totalBeforeVAT = saleItems.Sum(item => item.SellingPrice * item.Quantity);

            // Step 2: Apply VAT (11%)
            decimal vatAmount = totalBeforeVAT * 0.11m;
            decimal totalAfterVAT = totalBeforeVAT + vatAmount;

            // Step 3: Apply Discount (AFTER VAT)
            decimal totalDiscount = totalAfterVAT * (saleItems.Any() ? saleItems.Average(item => item.Discount) / 100 : 0);
            decimal totalAmount = totalAfterVAT - totalDiscount; // Final Total

            // Get the selected payment status from the ComboBox
            string paymentStatus = comboBoxPaymentStatus.SelectedItem.ToString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Insert into CustomerOrder table with Payment Status
                    string orderQuery = @"
            INSERT INTO CustomerOrder (customer_id, order_date, total_amount, vat_amount, discount_amount, payment_status)
            VALUES (@customerId, @orderDate, @totalAmount, @vat, @discount, @paymentStatus);
            SELECT SCOPE_IDENTITY();";

                    SqlCommand orderCommand = new SqlCommand(orderQuery, connection, transaction);
                    orderCommand.Parameters.AddWithValue("@customerId", customerId);
                    orderCommand.Parameters.AddWithValue("@orderDate", orderDate);
                    orderCommand.Parameters.AddWithValue("@totalAmount", totalAmount);
                    orderCommand.Parameters.AddWithValue("@vat", vatAmount);
                    orderCommand.Parameters.AddWithValue("@discount", totalDiscount);
                    orderCommand.Parameters.AddWithValue("@paymentStatus", paymentStatus);

                    int customerOrderId = Convert.ToInt32(orderCommand.ExecuteScalar());

                    // Insert into CustomerOrderDetails table and update stock
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

                        // ✅ Reduce stock after saving the sale details
                        string updateStockQuery = "UPDATE Product SET stock_quantity = stock_quantity - @quantity WHERE product_id = @productId";
                        SqlCommand updateStockCommand = new SqlCommand(updateStockQuery, connection, transaction);
                        updateStockCommand.Parameters.AddWithValue("@quantity", item.Quantity);
                        updateStockCommand.Parameters.AddWithValue("@productId", item.ProductId);
                        updateStockCommand.ExecuteNonQuery();
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
                        int stockQuantity = reader.GetInt32(3);  // Fetch stock quantity

                        var existingSaleItem = saleItems.FirstOrDefault(item => item.ProductId == productId);
                        if (existingSaleItem != null)
                        {
                            // Check if the new quantity exceeds available stock
                            if (existingSaleItem.Quantity + 1 > stockQuantity)
                            {
                                MessageBox.Show("Quantity exceeds available stock!", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                existingSaleItem.Quantity += 1;
                            }
                        }
                        else
                        {
                            // Check if the initial quantity exceeds available stock
                            if (1 > stockQuantity)
                            {
                                MessageBox.Show("Quantity exceeds available stock!", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                saleItems.Add(new SaleItem
                                {
                                    ProductId = productId,
                                    ProductName = productName,
                                    Quantity = 1,
                                    SellingPrice = sellingPrice,
                                    StockQuantity = stockQuantity // Add the stock quantity
                                });
                            }
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
                    dataGridViewSale.Columns["StockQuantity"].ReadOnly = true; 
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
            public int StockQuantity { get; set; } // Add stock quantity  

            // Read-Only Property (No Setter)
            public decimal TotalPrice
            {
                get
                {
                    decimal priceBeforeVAT = Quantity * SellingPrice;
                    decimal vatAmount = priceBeforeVAT * (VAT / 100);
                    decimal priceAfterVAT = priceBeforeVAT + vatAmount;
                    decimal discountAmount = priceAfterVAT * (Discount / 100);
                    return priceAfterVAT - discountAmount;
                }
            }
        }



        private void Btnclear_Click(object sender, EventArgs e)
        {
            // Clear the sale items and reset the form
            listBoxCustomers.Items.Clear();
            LoadCustomers();
            saleItems.Clear();
            totalAmount = 0;
            RefreshDataGridView();
            txtBarcode.Clear();
            txtBarcode.Focus();
            comboBoxPaymentStatus.SelectedIndex = -1;
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
                                existingItem.Quantity += 1;  // Just increase quantity in the cart (not in stock)
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
                        MessageBox.Show("Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            listBoxCustomers.Items.Clear(); // Clear existing items

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query;

                // Check if the searchText is a number (for ID search)
                if (int.TryParse(searchText, out int customerId))
                {
                    query = "SELECT customer_id, fname, lname FROM Customer WHERE customer_id = @searchText";
                }
                else
                {
                    query = "SELECT customer_id, fname, lname FROM Customer WHERE fname LIKE @searchText OR lname LIKE @searchText";
                    searchText = $"%{searchText}%"; // Add wildcards for name search
                }

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@searchText", searchText);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string fullName = reader.GetString(1) + " " + reader.GetString(2);

                        listBoxCustomers.Items.Add(new CustomerItem { CustomerId = id, CustomerName = fullName });
                    }

                    reader.Close();
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

            // Override ToString() so that ListBox displays both ID and Name
            public override string ToString()
            {
                return $"{CustomerId} - {CustomerName}"; // Example: "101 - John Doe"
            }
        }



        private void txtSearchCustomer_TextChanged(object sender, EventArgs e)
        {
            LoadCustomers(txtSearchCustomer.Text.Trim()); // Trim removes extra spaces
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
            if (ValidateOrder() && ValidateStockBeforePrint() && ValidateDiscountBeforePrint())  // Check all validations before proceeding
            {
                PrintInvoice();
            }

            if (comboBoxPaymentStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
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
            if (ValidateOrder() && ValidateStockBeforePrint() && ValidateDiscountBeforePrint())  // Check all validations before proceeding
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

            int yPos = 100; // Starting Y position

            // Header
            graphics.DrawString("Customer Order Invoice", headerFont, brush, 100, yPos);
            yPos += 50;

            // Customer Info
            if (listBoxCustomers.SelectedItem != null)
            {
                var selectedCustomer = listBoxCustomers.SelectedItem as CustomerItem;
                graphics.DrawString($"Customer: {selectedCustomer.CustomerName}", bodyFont, brush, 100, yPos);
            }
            else
            {
                graphics.DrawString("Customer: Not Selected", bodyFont, brush, 100, yPos);
            }

            yPos += 30;
            graphics.DrawString($"Order Date & Time: {dateTimePickerOrderDate.Value.ToString("f")}", bodyFont, brush, 100, yPos);
            yPos += 30;

            // **Payment Status**
            string paymentStatus = comboBoxPaymentStatus.SelectedItem?.ToString() ?? "Pending"; // Retrieve payment status
            graphics.DrawString($"Payment Status: {paymentStatus}", bodyFont, brush, 100, yPos);

            yPos += 40;

            // Item List Header (Including Discount Percentage Column)
            int xProduct = 100, xQuantity = 400, xPrice = 500, xDiscount = 600, xTotal = 725;
            graphics.DrawString("Product Name", bodyFont, brush, xProduct, yPos);
            graphics.DrawString("Quantity", bodyFont, brush, xQuantity, yPos);
            graphics.DrawString("Price", bodyFont, brush, xPrice, yPos);
            graphics.DrawString("Discount (%)", bodyFont, brush, xDiscount, yPos);  // New Discount Percentage column
            graphics.DrawString("Total", bodyFont, brush, xTotal, yPos);

            yPos += 30;

            // Item List
            decimal totalItemDiscount = 0; // To accumulate total discount
            foreach (var item in saleItems)
            {
                decimal itemTotalBeforeDiscount = item.SellingPrice * item.Quantity;
                decimal itemVAT = itemTotalBeforeDiscount * 0.11m;
                decimal itemTotalWithVAT = itemTotalBeforeDiscount + itemVAT;

                // Calculate the discount for this item based on the discount percentage
                decimal itemDiscountAmount = itemTotalWithVAT * (item.Discount / 100);
                totalItemDiscount += itemDiscountAmount;

                graphics.DrawString(item.ProductName, bodyFont, brush, xProduct, yPos);
                graphics.DrawString(item.Quantity.ToString(), bodyFont, brush, xQuantity, yPos);
                graphics.DrawString(item.SellingPrice.ToString("C"), bodyFont, brush, xPrice, yPos);
                graphics.DrawString(item.Discount.ToString("F2") + "%", bodyFont, brush, xDiscount, yPos);  // Discount Percentage column
                graphics.DrawString(itemTotalWithVAT.ToString("C"), bodyFont, brush, xTotal, yPos);

                yPos += 30;
            }

            // Discount and VAT calculations
            decimal totalBeforeDiscount = saleItems.Sum(item => item.SellingPrice * item.Quantity);

            // Calculate VAT first, before applying discount
            decimal totalVAT = totalBeforeDiscount * 0.11m; // VAT on the total before discount

            // Apply VAT first
            decimal totalWithVAT = totalBeforeDiscount + totalVAT; // Total with VAT applied

            // Calculate the total discount
            decimal totalDiscount = totalWithVAT * (saleItems.Any() ? saleItems.Average(item => item.Discount) / 100 : 0);

            // Final amount after applying discount
            decimal finalAmount = totalWithVAT - totalDiscount;

            // Display the total discount amount (not the percentage)
            yPos += 20;
            graphics.DrawString($"Total Discount: -{totalItemDiscount:C}", bodyFont, brush, 100, yPos); // Display total discount amount

            // Display VAT
            yPos += 20;
            graphics.DrawString($"VAT (11%): +{totalVAT:C}", bodyFont, brush, 100, yPos);

            // Display the total amount after discount and VAT
            yPos += 20;
            graphics.DrawString($"Total Amount: {finalAmount:C}", headerFont, brush, 100, yPos);

            e.HasMorePages = false; // Only one page
        }






        private bool ValidateStockBeforePrint()
        {
            foreach (var item in saleItems)
            {
                // Get the available stock from the database for each item
                int availableStock = GetProductStockQuantity(item.ProductId);

                // If the quantity exceeds the available stock, return false
                if (item.Quantity > availableStock)
                {
                    MessageBox.Show($"The quantity of {item.ProductName} exceeds the available stock. Please adjust the quantity.", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true; // All quantities are valid
        }
        private bool ValidateDiscountBeforePrint()
        {
            foreach (var item in saleItems)
            {
                // If the discount is less than 0 or greater than 100, return false
                if (item.Discount < 0 || item.Discount > 100)
                {
                    MessageBox.Show($"The discount for {item.ProductName} is invalid. Please enter a value between 0 and 100.", "Discount Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true; // All discounts are valid
        }

        private void Customer_Order_With_barcode_FormClosing(object sender, FormClosingEventArgs e)
        {   /*
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true; // Cancel closing
            }
            */
        }
    }
}
