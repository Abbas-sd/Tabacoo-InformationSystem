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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace TobaccoStore
{
    public partial class order : Form
    {
        public order()
        {
            InitializeComponent();
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TobaccoStoreIS;Trusted_Connection=True;";

        private void order_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            LoadProducts();
            LoadOrders();


            cmbOrderStatus.Items.Add("Pending");
            cmbOrderStatus.Items.Add("Processing");
            cmbOrderStatus.Items.Add("Shipped");
            cmbOrderStatus.Items.Add("Completed");
            cmbOrderStatus.Items.Add("Canceled");
            cmbOrderStatus.Items.Add("Refunded");
            cmbOrderStatus.Items.Add("On Hold");
            cmbOrderStatus.SelectedIndex = 0; // Default to "Pending"
        }

        private void ClearForm()
        {
            cmbCustomer.SelectedIndex = 0;
            cmbOrderStatus.SelectedIndex = 0;
            txtQuantity.Clear();
            lstProducts.ClearSelected(); // Clear product selection
        }

        private void LoadCustomers()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT cid, CONCAT(fname, ' ', lname) AS full_name FROM Customer";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmbCustomer.DataSource = dt;
                cmbCustomer.DisplayMember = "full_name";
                cmbCustomer.ValueMember = "cid";
            }
        }

        private void LoadProducts()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT pid, product_name FROM Product";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                lstProducts.DataSource = dt;
                lstProducts.DisplayMember = "product_name";
                lstProducts.ValueMember = "pid";
            }
        }

        private void LoadOrders()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT o.oid, c.fname + ' ' + c.lname AS customer_name, o.order_status, o.total_amount
                         FROM CustomerOrders o
                         JOIN Customer c ON o.cid = c.cid";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridViewOrders.DataSource = dt;
            }
        }

        private void btnAddOrder_Click(object sender, EventArgs e)
        {

            if (cmbCustomer.SelectedValue == null || lstProducts.SelectedItems.Count == 0 || cmbOrderStatus.Text == null)
            {
                MessageBox.Show("Please select a customer, products, and provide an order status.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Insert into Order table
                    string insertOrderQuery = @"INSERT INTO CustomerOrders (cid, order_status, total_amount) 
                                        OUTPUT INSERTED.oid 
                                        VALUES (@cid, @status, @totalAmount)";
                    SqlCommand cmdOrder = new SqlCommand(insertOrderQuery, conn, transaction);
                    cmdOrder.Parameters.AddWithValue("@cid", cmbCustomer.SelectedValue);
                    cmdOrder.Parameters.AddWithValue("@status", cmbOrderStatus.SelectedItem.ToString());
                    cmdOrder.Parameters.AddWithValue("@totalAmount", 0); // Placeholder, calculate later
                    int orderId = (int)cmdOrder.ExecuteScalar();

                    // Insert into OrderProduct table
                    decimal totalAmount = 0;

                    foreach (DataRowView product in lstProducts.SelectedItems)
                    {
                        int productId = (int)product["pid"];
                        int quantity = Convert.ToInt32(txtQuantity.Text);

                        // Get product price
                        string getPriceQuery = "SELECT price FROM Product WHERE pid = @pid";
                        SqlCommand cmdPrice = new SqlCommand(getPriceQuery, conn, transaction);
                        cmdPrice.Parameters.AddWithValue("@pid", productId);
                        decimal price = (decimal)cmdPrice.ExecuteScalar();

                        // Insert into OrderProduct
                        string insertOrderProductQuery = @"INSERT INTO Order_Product (oid, pid, quantity, price)
                                                   VALUES (@oid, @pid, @quantity, @price)";
                        SqlCommand cmdOrderProduct = new SqlCommand(insertOrderProductQuery, conn, transaction);
                        cmdOrderProduct.Parameters.AddWithValue("@oid", orderId);
                        cmdOrderProduct.Parameters.AddWithValue("@pid", productId);
                        cmdOrderProduct.Parameters.AddWithValue("@quantity", quantity);
                        cmdOrderProduct.Parameters.AddWithValue("@price", price);
                        cmdOrderProduct.ExecuteNonQuery();

                        // Calculate total amount
                        totalAmount += price * quantity;
                    }

                    // Update total amount in Order table
                    string updateOrderQuery = "UPDATE CustomerOrders SET total_amount = @totalAmount WHERE oid = @oid";
                    SqlCommand cmdUpdateOrder = new SqlCommand(updateOrderQuery, conn, transaction);
                    cmdUpdateOrder.Parameters.AddWithValue("@totalAmount", totalAmount);
                    cmdUpdateOrder.Parameters.AddWithValue("@oid", orderId);
                    cmdUpdateOrder.ExecuteNonQuery();

                    transaction.Commit();
                    MessageBox.Show("Order added successfully!");

                    // Refresh data
                    LoadOrders();
                    ClearForm();
                }
                catch
                {
                    transaction.Rollback();
                    MessageBox.Show("Failed to add the order.");
                }
            }
        }

        private void dataGridViewOrders_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewOrders.Rows[e.RowIndex];
                // Populate fields with selected row data
                cmbCustomer.SelectedValue = row.Cells["cid"].Value;
                cmbOrderStatus.SelectedValue = row.Cells["order_status"].Value.ToString();
                // Load related products from OrderProduct table if needed
            }
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();

            form1.Show();

            this.Close();
        }

        private void cmbOrderStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbOrderStatus.SelectedValue = cmbOrderStatus.Text;
        }

    }
}
