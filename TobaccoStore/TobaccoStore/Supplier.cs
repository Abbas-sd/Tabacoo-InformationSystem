using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TobaccoStore
{
    public partial class Supplier : Form
    {
        public Supplier()
        {
            InitializeComponent();
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";

        private bool IsValidPhone(string phone)
        {
            // Allow only digits and enforce a length of 10-15 digits
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{8,15}$");
        }

        private void ClearSupplierFields()
        {
            txtsuplliername.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
        }
        private void btnsupplieradd_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtsuplliername.Text))
            {
                MessageBox.Show("Supplier Name required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtAddress.Text))
            {
                MessageBox.Show("Address is required.");
                return;
            }

            if (!IsValidPhone(txtPhone.Text))
            {
                MessageBox.Show("Please enter a valid phone number (8 to 15 digits).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Supplier (supplier_name, Phone, Address) VALUES (@fname, @phone, @address)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@fname", txtsuplliername.Text);
                command.Parameters.AddWithValue("@phone", txtPhone.Text);
                command.Parameters.AddWithValue("@address", txtAddress.Text);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Supplier added successfully.");
                        ClearSupplierFields();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add supplier.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();

            form1.Show();

            this.Hide();
        }

        private void btnUpdatessupplier_Click(object sender, EventArgs e)
        {
            UpdatSupplier form2 = new UpdatSupplier(this); // Pass 'this' as the calling form
            form2.Show();
            this.Hide();
        }

        private void btnDeletesupplier_Click(object sender, EventArgs e)
        {
            DeleteSupplier form78 = new DeleteSupplier(this); // Pass 'this' as the calling form
            form78.Show();
            this.Hide();
        }

        private void Supplier_Load(object sender, EventArgs e)
        {

        }
    }
}
