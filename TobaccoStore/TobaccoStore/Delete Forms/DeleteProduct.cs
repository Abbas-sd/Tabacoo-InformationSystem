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
    public partial class DeleteProduct : Form
    {
        public DeleteProduct()
        {
            InitializeComponent();

            // Disable the search button for Cashier role
            if (log_in.currentUserRole == Main.UserRole.User)
            {
                BtnDelete.Enabled = false;  // Disable button1 (search button) for Cashier
            }

            if (log_in.currentUserRole == Main.UserRole.Cashier)
            {
                BtnDelete.Enabled = false;  // Disable button1 (search button) for Cashier
            }
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductId.Text))
            {
                MessageBox.Show("Please enter the Product ID to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtProductId.Text, out int productId))
            {
                MessageBox.Show("Product ID must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this product?",
                                                  "Confirm Deletion",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.No)
            {
                return; // Exit if user cancels
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Product WHERE product_id = @productId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@productId", productId);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Product deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtProductId.Clear();
                    }
                    else
                    {
                        MessageBox.Show("No product found with the given ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void loadProducts()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Product";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dgvProduct.DataSource = dataTable;
                dgvProduct.ReadOnly = true;
            }
            foreach (DataGridViewColumn column in dgvProduct.Columns)
            {
                column.Width = 80;
            }

            // Check if the fourth column exists before setting width
            if (dgvProduct.Columns.Count >= 4)
            {
                dgvProduct.Columns[3].Width = 150; // Index 3 = fourth column (zero-based index)
            }
        }
        
        private void DeleteProduct_Load(object sender, EventArgs e)
        {
            loadProducts();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            loadProducts();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            product form22 = new product();

            form22.Show();

            this.Hide();
        }
    }
}
