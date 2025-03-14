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
    public partial class Suporder : Form
    {
        private PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
        private PrintDocument printDocument = new PrintDocument();
        private Bitmap logo; // Store the company logo

        public Suporder()
        {
            InitializeComponent();

            if (log_in.currentUserRole == Main.UserRole.Stoker)
            {
                btnSubmitOrder.Enabled = false;  // Disable button1 (search button) for Cashier
                btnPrintPreview.Enabled = false;
                btnPrintOrder.Enabled = false;
            }

            if (log_in.currentUserRole == Main.UserRole.User)
            {
                btnSubmitOrder.Enabled = false;  // Disable button1 (search button) for Cashier
                btnPrintPreview.Enabled = false;
                btnPrintOrder.Enabled = false;
            }
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
            orderDataTable.Columns.Add("VAT");  // New VAT column
            orderDataTable.Columns.Add("Discount"); // New Discount column
            orderDataTable.Columns.Add("Final Amount"); // New Final Amount column after discount

            // Bind DataTable to DataGridView
            dataGridViewOrder.DataSource = orderDataTable;

            dataGridViewOrder.Columns["VAT"].ReadOnly = true;
            dataGridViewOrder.Columns["Total Amount"].ReadOnly = true;
            dataGridViewOrder.Columns["Final Amount"].ReadOnly = true;
            dataGridViewOrder.Columns["Product Info"].ReadOnly = true;
            dataGridViewOrder.Columns["Supplier Info"].ReadOnly = true;

            
            // Ensure these columns are editable
            dataGridViewOrder.Columns["Quantity"].ReadOnly = false;
            dataGridViewOrder.Columns["Cost Price"].ReadOnly = false;
            dataGridViewOrder.Columns["Discount"].ReadOnly = false;

            // Wire up the CellValueChanged event
            dataGridViewOrder.CellValueChanged += dataGridViewOrder_CellValueChanged;

            comboBoxPaymentStatus.Items.Add("Paid");
            comboBoxPaymentStatus.Items.Add("Partial");

            // Load company logo
            try
            {
                logo = new Bitmap("C:\\Users\\abbas\\OneDrive\\Desktop\\images\\Tabacoo-Icon2.jpeg"); // Update path
            }
            catch
            {
                logo = null; // If the logo fails to load, avoid crashing
            }

            // Assign the print event
            printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
        }

        class ListBoxItem
        {
            public string Text { get; set; }  // Display name (Product/Supplier Name)
            public int Value { get; set; }    // ID (Product/Supplier ID)

            public override string ToString() => $"{Text} (ID: {Value})"; // Shows name and ID in ListBox
        }

        private List<ListBoxItem> allProducts = new List<ListBoxItem>();

        private void LoadProducts()
        {
            lstProducts.Items.Clear(); // Clear the list before loading
            allProducts.Clear(); // Clear the allProducts list before repopulating

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
                        var product = new ListBoxItem
                        {
                            Text = reader["product_name"].ToString(),
                            Value = Convert.ToInt32(reader["product_id"])
                        };

                        // Add product to both the ListBox and allProducts list
                        lstProducts.Items.Add(product);
                        allProducts.Add(product);
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
            lstSuppliers.Items.Clear(); // Clear the list before loading
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
                        // Add suppliers to the ListBox using ListBoxItem class
                        lstSuppliers.Items.Add(new ListBoxItem
                        {
                            Text = reader["supplier_name"].ToString(),
                            Value = Convert.ToInt32(reader["supplier_id"])
                        });
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
            // Ensure a valid selection
            if (lstProducts.SelectedItem is ListBoxItem selectedProduct)
            {
                int productId = selectedProduct.Value; // Get selected product ID
                decimal costPrice = 0;  // Variable to store the fetched cost price

                // Fetch cost price and stock quantity
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
                            costPrice = reader["cost_price"] != DBNull.Value ? Convert.ToDecimal(reader["cost_price"]) : 0;
                            int stockQuantity = Convert.ToInt32(reader["stock_quantity"]);
                            lblStockQuantity.Text = $"Stock Quantity: {stockQuantity}"; // Update stock label
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error fetching product details: " + ex.Message);
                    }
                }

                // You can use the costPrice variable wherever needed, e.g., when adding items to the order grid

                // Load suppliers related to the selected product
                LoadSuppliersForProduct(productId);
            }
        }



        private void LoadSuppliersForProduct(int productId)
        {
            // Clear existing suppliers
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
                        // Add suppliers using ListBoxItem
                        lstSuppliers.Items.Add(new ListBoxItem
                        {
                            Text = reader["supplier_name"].ToString(),
                            Value = Convert.ToInt32(reader["supplier_id"])
                        });
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

            if (comboBoxPaymentStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string paymentStatus = comboBoxPaymentStatus.SelectedItem.ToString(); // Get selected payment status

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Start a transaction
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert data into SupplierOrder table (including payment_status)
                        string insertOrderQuery = "INSERT INTO SupplierOrder (supplier_id, order_date, total_amount, vat_amount, discount_amount, payment_status) " +
                                                  "VALUES (@supplierId, @orderDate, @totalAmount, @vatAmount, @discountAmount, @paymentStatus); SELECT SCOPE_IDENTITY();";

                        int supplierId = 0;
                        DateTime orderDate = dateTimePickerOrderDate.Value.Date; // Ensure we get the date part

                        // Assuming that we have selected a supplier
                        if (lstSuppliers.SelectedItem != null)
                        {
                            var selectedSupplier = (dynamic)lstSuppliers.SelectedItem;
                            supplierId = selectedSupplier.Value;
                        }

                        // Calculate the total amount, VAT, and Discount
                        decimal totalAmount = 0;
                        decimal totalVat = 0;
                        decimal totalDiscount = 0;

                        foreach (DataRow row in orderDataTable.Rows)
                        {
                            decimal rowTotal = Convert.ToDecimal(row["Total Amount"]);
                            totalAmount += rowTotal;

                            // Get VAT from the "VAT" column (assumed to be stored in the grid)
                            decimal rowVat = Convert.ToDecimal(row["VAT"]); // VAT amount for this row
                            totalVat += rowVat;

                            // Get discount from the "Discount" column (assumed to be stored in the grid)
                            decimal rowDiscount = Convert.ToDecimal(row["Discount"]); // Discount percentage for this row
                            decimal amountAfterVat = rowTotal + rowVat; // Amount after VAT
                            decimal discountAmount = amountAfterVat * (rowDiscount / 100); // Apply discount
                            totalDiscount += discountAmount;
                        }

                        // Final total amount after applying VAT and discount
                        decimal finalAmount = totalAmount + totalVat - totalDiscount;

                        // Insert the order data into the SupplierOrder table
                        using (SqlCommand cmd = new SqlCommand(insertOrderQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@supplierId", supplierId);
                            cmd.Parameters.AddWithValue("@orderDate", orderDate);
                            cmd.Parameters.AddWithValue("@totalAmount", finalAmount);
                            cmd.Parameters.AddWithValue("@vatAmount", totalVat);
                            cmd.Parameters.AddWithValue("@discountAmount", totalDiscount);
                            cmd.Parameters.AddWithValue("@paymentStatus", paymentStatus); // Add payment status

                            // Get the supplier_order_id (identity value)
                            int supplierOrderId = Convert.ToInt32(cmd.ExecuteScalar());

                            // Insert data into SupplierOrderDetails table for each row
                            string insertOrderDetailsQuery = "INSERT INTO SupplierOrderDetails (supplier_order_id, product_id, quantity, cost_price_at_order, discount, vat) " +
                                                             "VALUES (@supplierOrderId, @productId, @quantity, @costPriceAtOrder, @discount, @vat)";

                            foreach (DataRow row in orderDataTable.Rows)
                            {
                                var productInfo = row["Product Info"].ToString();
                                int productId = Convert.ToInt32(productInfo.Split(new string[] { "(ID:" }, StringSplitOptions.None)[1].Replace(")", ""));
                                int quantity = Convert.ToInt32(row["Quantity"]);
                                decimal costPriceAtOrder = Convert.ToDecimal(row["Cost Price"]);
                                decimal rowDiscount = Convert.ToDecimal(row["Discount"]); // Discount percentage for this row
                                decimal vatPercentage = 11; // VAT percentage is 11%

                                // Insert the order detail
                                using (SqlCommand cmdDetails = new SqlCommand(insertOrderDetailsQuery, connection, transaction))
                                {
                                    cmdDetails.Parameters.AddWithValue("@supplierOrderId", supplierOrderId);
                                    cmdDetails.Parameters.AddWithValue("@productId", productId);
                                    cmdDetails.Parameters.AddWithValue("@quantity", quantity);
                                    cmdDetails.Parameters.AddWithValue("@costPriceAtOrder", costPriceAtOrder);
                                    cmdDetails.Parameters.AddWithValue("@discount", rowDiscount); // Store the discount as percentage
                                    cmdDetails.Parameters.AddWithValue("@vat", vatPercentage); // Insert VAT percentage (11%)

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

                        // Commit the transaction if no issues
                        transaction.Commit();
                        MessageBox.Show("Order saved successfully and stock updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
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
        private void dataGridViewOrder_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridViewOrder.Columns["Cost Price"].Index) // Check for Cost Price column
            {
                // Check if the entered value is a valid decimal and is not negative
                if (decimal.TryParse(e.FormattedValue.ToString(), out decimal newCostPrice))
                {
                    if (newCostPrice < 0)
                    {
                        MessageBox.Show("Cost Price cannot be negative.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true; // Cancel the edit if value is negative
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid numeric value for Cost Price.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // Cancel the edit if the value is not numeric
                }
            }
        }

        private bool isUpdating = false;

        private void dataGridViewOrder_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isUpdating)
                return; // Prevent recursion if we're already updating the grid programmatically

            // Only handle changes in the Cost Price, Quantity, or Discount columns
            if (e.RowIndex >= 0)
            {
                var row = dataGridViewOrder.Rows[e.RowIndex];

                // Handle changes in the Cost Price, Quantity, or Discount columns to recalculate total amounts
                if (e.ColumnIndex == dataGridViewOrder.Columns["Cost Price"].Index ||
                    e.ColumnIndex == dataGridViewOrder.Columns["Quantity"].Index ||
                    e.ColumnIndex == dataGridViewOrder.Columns["Discount"].Index)
                {
                    try
                    {
                        isUpdating = true;  // Set flag to prevent recursion

                        decimal costPrice = Convert.ToDecimal(row.Cells["Cost Price"].Value);
                        int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                        decimal discount = Convert.ToDecimal(row.Cells["Discount"].Value);

                        // Calculate total amount
                        decimal totalAmount = costPrice * quantity;

                        // VAT Calculation (11%)
                        decimal vat = totalAmount * 0.11m;

                        // Discount Calculation
                        decimal discountAmount = totalAmount * (discount / 100.0m);

                        // Final Amount after VAT and Discount
                        decimal finalAmount = totalAmount + vat - discountAmount;

                        // Update the calculated columns in the grid
                        row.Cells["Total Amount"].Value = totalAmount;
                        row.Cells["VAT"].Value = vat;
                        row.Cells["Final Amount"].Value = finalAmount;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error calculating totals: {ex.Message}", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        isUpdating = false; // Reset the flag
                    }

                    // After updating the row, update the total amount label
                    UpdateTotalAmount(sender, e);
                }
            }
        }

        private void UpdateTotalAmount(object sender, EventArgs e)  
        {
            if (orderDataTable.Rows.Count > 0)
            {
                decimal totalAmount = 0;
                decimal totalVat = 0;
                decimal totalDiscount = 0;

                foreach (DataRow row in orderDataTable.Rows)
                {
                    // Get the total amount for each row (before VAT and discount)
                    if (row["Total Amount"] != DBNull.Value)
                    {
                        decimal rowTotal = Convert.ToDecimal(row["Total Amount"]);
                        totalAmount += rowTotal;

                        // Get VAT from the grid (assuming a column called "VAT" in the grid)
                        decimal rowVat = Convert.ToDecimal(row["VAT"]); // This should be 11% or whatever VAT rate per row
                        totalVat += rowVat;

                        // Calculate the amount after VAT
                        decimal amountAfterVat = rowTotal + rowVat;

                        // Get discount from the grid (assuming a column called "Discount")
                        decimal rowDiscount = Convert.ToDecimal(row["Discount"]); // This should be the discount percentage for the row
                        decimal discountAmount = amountAfterVat * (rowDiscount / 100);
                        totalDiscount += discountAmount;
                    }
                }

                // Calculate final amount after applying VAT and discount
                decimal finalAmount = totalAmount + totalVat - totalDiscount;

                // Display the amounts
                lblTotalAmount.Text = $"Total Amount: {finalAmount:C}\nVAT: {totalVat:C}\nDiscount: {totalDiscount:C}";
            }
            else
            {
                lblTotalAmount.Text = "Total Amount: $0.00\nVAT: $0.00\nDiscount: $0.00";
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
            lblStockQuantity.Text = string.Empty;
            lblTotalAmount.Text = string.Empty;
            txtDiscount.Text = "0";
            txtSearchProduct.Text = string.Empty;
            orderDataTable.Clear();  // Clears the order DataGridView as well
            comboBoxPaymentStatus.SelectedIndex = -1;
        }

        private void btnAddToOrder_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (lstProducts.SelectedItem == null || lstSuppliers.SelectedItem == null)
            {
                MessageBox.Show("Please select a product and a supplier.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate discount input (store it as a percentage)
            if (!int.TryParse(txtDiscount.Text, out int discount) || discount < 0 || discount > 100)
            {
                MessageBox.Show("Discount must be a number between 0 and 100.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the selected product and supplier
            var selectedProduct = (dynamic)lstProducts.SelectedItem;
            var selectedSupplier = (dynamic)lstSuppliers.SelectedItem;

            int productId = selectedProduct.Value;   // Product ID
            string productName = selectedProduct.Text;  // Product Name
            int supplierId = selectedSupplier.Value; // Supplier ID
            string supplierName = selectedSupplier.Text; // Supplier Name

            decimal costPrice = 0;

            // Fetch the cost price from the database based on the selected product
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT cost_price FROM Product WHERE product_id = @productId";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@productId", productId);
                    costPrice = (decimal)cmd.ExecuteScalar();
                }
            }

            // Calculate total amount
            decimal totalAmount = costPrice;

            // VAT Calculation (11%)
            decimal vat = totalAmount * 0.11m;

            // Discount Calculation (using discount percentage)
            decimal discountAmount = totalAmount * (discount / 100.0m);

            // Final Amount after VAT and Discount
            decimal finalAmount = totalAmount + vat - discountAmount;

            // Format product and supplier info
            string productInfo = $"{productName} (ID: {productId})";
            string supplierInfo = $"{supplierName} (ID: {supplierId})";

            // Check if the product and supplier combination already exists in the DataTable
            bool exists = false;
            foreach (DataRow row in orderDataTable.Rows)
            {
                if (row["Product Info"].ToString() == productInfo && row["Supplier Info"].ToString() == supplierInfo)
                {
                    // If product and supplier match, update the quantity and amount
                    int newQuantity = Convert.ToInt32(row["Quantity"]) + 1; // Increase quantity by 1
                    row["Quantity"] = newQuantity;
                    row["Total Amount"] = newQuantity * costPrice;
                    row["VAT"] = (newQuantity * costPrice) * 0.11m;
                    row["Discount"] = discount; // Store discount as percentage
                    row["Final Amount"] = newQuantity * costPrice + (newQuantity * costPrice) * 0.11m - (newQuantity * costPrice) * (discount / 100.0m);

                    exists = true;
                    break;
                }
            }

            // If product and supplier combination doesn't exist, add a new row
            if (!exists)
            {
                orderDataTable.Rows.Add(productInfo, supplierInfo, 1, costPrice, totalAmount, vat, discount, finalAmount);
            }

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

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            if (orderDataTable.Rows.Count == 0)
            {
                MessageBox.Show("No orders to preview!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            printPreviewDialog.Document = printDocument;
            printPreviewDialog.ShowDialog();
        }
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font titleFont = new Font("Arial", 14, FontStyle.Bold);
            Font bodyFont = new Font("Arial", 10);
            Brush brush = Brushes.Black;

            int yPos = 50; // Initial Y position

            // Draw logo in the top-right corner (optional)
            if (logo != null)
            {
                graphics.DrawImage(logo, e.MarginBounds.Right - 100, yPos, 40, 40);
            }

            // Print Order Header
            graphics.DrawString("Supplier Order Invoice", titleFont, brush, 100, yPos);
            yPos += 40;
            graphics.DrawString($"Order Date & Time: {DateTime.Now.ToString("f")}", bodyFont, brush, 100, yPos);
            yPos += 20;

            // Retrieve Payment Status
            string paymentStatus = comboBoxPaymentStatus.SelectedItem?.ToString() ?? "Pending"; // Ensure it's retrieved correctly

            // Print Payment Status
            graphics.DrawString($"Payment Status: {paymentStatus}", bodyFont, brush, 100, yPos);
            yPos += 30;

            // Print Column Headers including Discount Percentage
            int xProduct = 100, xSupplier = 350, xQuantity = 500, xPrice = 563, xDiscount = 650, xTotal = 750;
            graphics.DrawString("Product", bodyFont, brush, xProduct, yPos);
            graphics.DrawString("Supplier", bodyFont, brush, xSupplier, yPos);
            graphics.DrawString("Qty", bodyFont, brush, xQuantity, yPos);
            graphics.DrawString("Price", bodyFont, brush, xPrice, yPos);
            graphics.DrawString("Discount (%)", bodyFont, brush, xDiscount, yPos);
            graphics.DrawString("Total", bodyFont, brush, xTotal, yPos);
            yPos += 20;

            // Declare variables for VAT and discount totals
            decimal totalVat = 0;
            decimal totalDiscount = 0;
            decimal totalAmountBeforeVat = 0; // Total amount before VAT for final calculation

            // Print Order Items
            foreach (DataRow row in orderDataTable.Rows)
            {
                decimal totalAmount = Convert.ToDecimal(row["Total Amount"]);
                decimal discountPercentage = Convert.ToDecimal(row["Discount"]); // Assuming discount is in percentage
                decimal vat = totalAmount * 0.11m; // VAT calculation (11%)

                decimal amountAfterVat = totalAmount + vat;

                // Calculate the discount amount after VAT
                decimal discountAmount = amountAfterVat * (discountPercentage / 100);

                totalVat += vat;
                totalDiscount += discountAmount;
                totalAmountBeforeVat += totalAmount; // Add the total amount to the before VAT total

                // Print the row data, including the discount percentage
                graphics.DrawString(row["Product Info"].ToString(), bodyFont, brush, xProduct, yPos);
                graphics.DrawString(row["Supplier Info"].ToString(), bodyFont, brush, xSupplier, yPos);
                graphics.DrawString(row["Quantity"].ToString(), bodyFont, brush, xQuantity, yPos);
                graphics.DrawString(row["Cost Price"].ToString(), bodyFont, brush, xPrice, yPos);
                graphics.DrawString(discountPercentage.ToString("F2") + "%", bodyFont, brush, xDiscount, yPos);  // Discount percentage
                graphics.DrawString(totalAmount.ToString("C"), bodyFont, brush, xTotal, yPos);
                yPos += 20;
            }

            // Print Total VAT, Discount, and Amount
            decimal finalAmount = totalAmountBeforeVat + totalVat - totalDiscount;

            
            yPos += 20;
            graphics.DrawString($"Total VAT (11%): {totalVat:C}", bodyFont, brush, 100, yPos);
            yPos += 20;
            graphics.DrawString($"Total Discount: {totalDiscount:C}", bodyFont, brush, 100, yPos);
            yPos += 20;
            graphics.DrawString($"Total Amount After VAT & Discount: {finalAmount:C}", titleFont, brush, 100, yPos);
        }







        private void btnPrintOrder_Click(object sender, EventArgs e)
        {
            if (orderDataTable.Rows.Count == 0)
            {
                MessageBox.Show("No orders to print!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBoxPaymentStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            printDocument.Print();
        }
        private void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearchProduct.Text.ToLower(); // Get the search text, converting it to lowercase

            // Filter the products based on name or ID
            var filteredProducts = allProducts.Where(p => p.Text.ToLower().Contains(searchText) || p.Value.ToString().Contains(searchText)).ToList();

            // Clear the list and repopulate with filtered products
            lstProducts.Items.Clear();
            lstProducts.Items.AddRange(filteredProducts.ToArray());
        }
    }
}     
    


