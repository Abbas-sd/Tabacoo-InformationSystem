﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace TobaccoStore
{
    public partial class product_barcode : Form
    {
        public product_barcode()
        {
            InitializeComponent();
            // Wire up the Shown event
            this.Shown += new EventHandler(product_Shown);
            // Wire up the TextChanged event for txtBarcode
            txtBarcode.TextChanged += new EventHandler(txtBarcode_TextChanged);
            // Wire up the KeyPress event for txtBarcode
            txtBarcode.KeyPress += new KeyPressEventHandler(txtBarcode_KeyPress);
        }

        private void product_Shown(object sender, EventArgs e)
        {
            // Set focus to the barcode text box when the form is shown
            txtBarcode.Focus();
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=Tabacoostore;Trusted_Connection=True;";
        private void product_barcode_Load(object sender, EventArgs e)
        {
            
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtBarcode.Text) ||
                string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtSupplierId.Text) ||
                string.IsNullOrWhiteSpace(txtCostPrice.Text) ||
                string.IsNullOrWhiteSpace(txtSellingPrice.Text) ||
                string.IsNullOrWhiteSpace(txtStockQuantity.Text) ||
                string.IsNullOrWhiteSpace(txtProductType.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate barcode
            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                MessageBox.Show("Barcode is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                string query = "INSERT INTO Product (barcode, supplier_id, product_name, product_type, cost_price, selling_price, stock_quantity) " +
                               "VALUES (@barcode, @supplierId, @productName, @productType, @costPrice, @sellingPrice, @stockQuantity)";
                SqlCommand command = new SqlCommand(query, connection);

                // Add parameters
                command.Parameters.AddWithValue("@barcode", txtBarcode.Text);
                command.Parameters.AddWithValue("@supplierId", supplierId);
                command.Parameters.AddWithValue("@productName", txtProductName.Text);
                command.Parameters.AddWithValue("@productType", txtProductType.Text);
                command.Parameters.AddWithValue("@costPrice", costPrice);
                command.Parameters.AddWithValue("@sellingPrice", sellingPrice);
                command.Parameters.AddWithValue("@stockQuantity", stockQuantity);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Product added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Clear fields after successful insertion
                        txtBarcode.Clear();
                        txtProductName.Clear();
                        txtSupplierId.Clear();
                        txtCostPrice.Clear();
                        txtSellingPrice.Clear();
                        txtStockQuantity.Clear();
                        txtProductType.Clear();
                    }
                    else
                    {
                        MessageBox.Show("No rows were inserted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) // Unique constraint violation (barcode already exists)
                    {
                        MessageBox.Show("A product with this barcode already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the Enter key(carriage return) is pressed
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Move focus to the next text box
                txtProductName.Focus();
                // Suppress the beep sound
                e.Handled = true;
            }
        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
