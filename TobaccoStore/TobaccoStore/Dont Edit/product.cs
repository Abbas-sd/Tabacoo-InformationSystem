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
    public partial class product : Form
    {
        public product()
        {
            InitializeComponent();
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=Tabacoostore;Trusted_Connection=True;";


        private void product_Load(object sender, EventArgs e)
        {
           
        }
        private void btnAddProduct_Click(object sender, EventArgs e)
        {// Validate inputs
            if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtSupplierId.Text) ||
                string.IsNullOrWhiteSpace(txtCostPrice.Text) ||
                string.IsNullOrWhiteSpace(txtSellingPrice.Text) ||
                string.IsNullOrWhiteSpace(txtStockQuantity.Text) ||
                string.IsNullOrWhiteSpace(txtProductType.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate cost price
            if (!decimal.TryParse(txtCostPrice.Text, out decimal costPrice) || costPrice <= 0)
            {
                MessageBox.Show("Cost Price must be a valid positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate selling price
            if (!decimal.TryParse(txtSellingPrice.Text, out decimal sellingPrice) || sellingPrice <= 0)
            {
                MessageBox.Show("Selling Price must be a valid positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate stock quantity
            if (!int.TryParse(txtStockQuantity.Text, out int stockQuantity) || stockQuantity < 0)
            {
                MessageBox.Show("Stock Quantity must be a valid non-negative number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate supplier ID
            if (!int.TryParse(txtSupplierId.Text, out int supplierId))
            {
                MessageBox.Show("Supplier ID must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Insert into the Product table
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Product (product_name, supplier_id, cost_price, selling_price, stock_quantity, product_type) " +
                               "VALUES (@productName, @supplierId, @costPrice, @sellingPrice, @stockQuantity, @productType)";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@productName", txtProductName.Text);
                command.Parameters.AddWithValue("@supplierId", supplierId);
                command.Parameters.AddWithValue("@costPrice", costPrice);
                command.Parameters.AddWithValue("@sellingPrice", sellingPrice);
                command.Parameters.AddWithValue("@stockQuantity", stockQuantity);
                command.Parameters.AddWithValue("@productType", txtProductType.Text);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Product added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear fields after successful insertion
                    txtProductName.Clear();
                    txtSupplierId.Clear();
                    txtCostPrice.Clear();
                    txtSellingPrice.Clear();
                    txtStockQuantity.Clear();
                    txtProductType.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            UpdateProduct form33 = new UpdateProduct();

            form33.Show();

            this.Hide();
        }



        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {

            DeleteProduct form1 = new DeleteProduct();

            form1.Show();

            this.Hide();

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();

            form1.Show();

            this.Close();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
