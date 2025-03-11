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
    public partial class UpdateSupplier : Form
    {
        
        public UpdateSupplier()
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

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";
        private void ClearSupplierFields()
        {
            txtSupplierId.Clear();
            txtsuplliername.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
        }

        private void LoadSuppliers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Supplier";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dgvSupplier.DataSource = dataTable;
            }

            foreach (DataGridViewColumn column in dgvSupplier.Columns)
            {
                column.Width = 150;
            }
        }
        private bool IsValidPhone(string phone)
        {
            // Allow only digits and enforce a length of 10-15 digits
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{8,15}$");
        }
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSupplierId.Text))
            {
                MessageBox.Show("Please enter the Supplier ID to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int supplierId;
            if (!int.TryParse(txtSupplierId.Text, out supplierId))
            {
                MessageBox.Show("Supplier ID must be a number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirmation dialog
            DialogResult result = MessageBox.Show("Are you sure you want to update this supplier?",
                                                  "Confirm Update",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.No) return;

            List<string> updateFields = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(txtsuplliername.Text))
            {
                updateFields.Add("supplier_name = @fname");
                parameters.Add(new SqlParameter("@fname", txtsuplliername.Text));
            }
            
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                if (IsValidPhone(txtPhone.Text))
                {
                    updateFields.Add("Phone = @phone");
                    parameters.Add(new SqlParameter("@phone", txtPhone.Text));
                }
                else
                {
                    MessageBox.Show("Please enter a valid phone number (8 to 15 digits).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (!string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                updateFields.Add("Address = @address");
                parameters.Add(new SqlParameter("@address", txtAddress.Text));
            }

            if (updateFields.Count == 0)
            {
                MessageBox.Show("Please fill at least one field to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string updateQuery = "UPDATE Supplier SET " + string.Join(", ", updateFields) + " WHERE supplier_id  = @supplierId";
            parameters.Add(new SqlParameter("@supplierId", supplierId));

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
                        MessageBox.Show("Supplier updated successfully.");
                        ClearSupplierFields();
                    }
                    else
                    {
                        MessageBox.Show("No supplier found with the given ID.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                LoadSuppliers();
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadSuppliers();
        }

        private void UpdatSupplier_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtSupplierId_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
