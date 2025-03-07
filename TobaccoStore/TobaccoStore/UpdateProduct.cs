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
    public partial class UpdateProduct : Form
    {
        public UpdateProduct()
        {
            InitializeComponent();

            // Disable the search button for Cashier role
            if (log_in.currentUserRole == Main.UserRole.User)
            {
                BtnUpdate.Enabled = false;  // Disable button1 (search button) for Cashier
            }

            if (log_in.currentUserRole == Main.UserRole.Cashier)
            {
                BtnUpdate.Enabled = false;  // Disable button1 (search button) for Cashier
            }
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=Tabacoostore;Trusted_Connection=True;";

        private void loadProducts()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Product";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dgvProduct.DataSource = dataTable;
            }
        }
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductId.Text))
            {
                MessageBox.Show("Please enter the Product ID to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtProductId.Text, out int productId))
            {
                MessageBox.Show("Product ID must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> updateFields = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                updateFields.Add("product_name = @productName");
                parameters.Add(new SqlParameter("@productName", txtProductName.Text));
            }

            if (!string.IsNullOrWhiteSpace(txtProductType.Text))
            {
                updateFields.Add("product_type = @productType");
                parameters.Add(new SqlParameter("@productType", txtProductType.Text));
            }

            if (!string.IsNullOrWhiteSpace(txtSupplierId.Text))
            {
                if (int.TryParse(txtSupplierId.Text, out int supplierId))
                {
                    updateFields.Add("supplier_id = @supplierId");
                    parameters.Add(new SqlParameter("@supplierId", supplierId));
                }
                else
                {
                    MessageBox.Show("Supplier ID must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtCostPrice.Text))
            {
                if (decimal.TryParse(txtCostPrice.Text, out decimal costPrice) && costPrice > 0)
                {
                    updateFields.Add("cost_price = @costPrice");
                    parameters.Add(new SqlParameter("@costPrice", costPrice));
                }
                else
                {
                    MessageBox.Show("Cost Price must be a valid positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtSellingPrice.Text))
            {
                if (decimal.TryParse(txtSellingPrice.Text, out decimal sellingPrice) && sellingPrice > 0)
                {
                    updateFields.Add("selling_price = @sellingPrice");
                    parameters.Add(new SqlParameter("@sellingPrice", sellingPrice));
                }
                else
                {
                    MessageBox.Show("Selling Price must be a valid positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtStockQuantity.Text))
            {
                if (int.TryParse(txtStockQuantity.Text, out int stockQuantity) && stockQuantity >= 0)
                {
                    updateFields.Add("stock_quantity = @stockQuantity");
                    parameters.Add(new SqlParameter("@stockQuantity", stockQuantity));
                }
                else
                {
                    MessageBox.Show("Stock Quantity must be a valid non-negative number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (updateFields.Count == 0)
            {
                MessageBox.Show("Please fill at least one field to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string updateQuery = "UPDATE Product SET " + string.Join(", ", updateFields) + " WHERE product_id = @productId";
            parameters.Add(new SqlParameter("@productId", productId));

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(updateQuery, connection);
                command.Parameters.AddRange(parameters.ToArray());

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Product updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        loadProducts();  // Refresh the data grid
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

                // Clear fields after update
                txtProductId.Clear();
                txtProductName.Clear();
                txtSupplierId.Clear();
                txtCostPrice.Clear();
                txtSellingPrice.Clear();
                txtStockQuantity.Clear();
                txtProductType.Clear();
            }
        }


        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            loadProducts();
        }

        private void UpdateProduct_Load(object sender, EventArgs e)
        {
            loadProducts();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            product form7 = new product();

            form7.Show();

            this.Hide();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
