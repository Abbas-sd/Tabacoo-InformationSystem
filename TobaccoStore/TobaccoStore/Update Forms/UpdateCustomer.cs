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
    public partial class UpdateCustomer : Form
    {
        
        public UpdateCustomer()
        {
            InitializeComponent();
            
            if (log_in.currentUserRole == Main.UserRole.User || log_in.currentUserRole == Main.UserRole.Cashier)
            {
                BtnUpdate.Enabled = false;
            }

        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";
        private void ClearCustomerFields()
        {
            txtFname.Clear();
            txtLname.Clear();
            txtAge.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
            rbtnMale.Checked = false;
            rbtnFemale.Checked = false;
        }

        private void LoadCustomers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Customer";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dgvCustomers.DataSource = dataTable;
            }

            // Set default width for all columns
            foreach (DataGridViewColumn column in dgvCustomers.Columns)
            {
                column.Width = 80;
            }

            // Set the width of the last column to 150
            if (dgvCustomers.Columns.Count > 0)
            {
                dgvCustomers.Columns[dgvCustomers.Columns.Count - 1].Width = 200; // Last column
            }
        }

        private bool IsValidPhone(string phone)
        {
            // Allow only digits and enforce a length of 10-15 digits
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{8,15}$");
        }
        private void BtnUpdate_Click(object sender, EventArgs e)
        {


            // Step 1: Validate Customer ID input
            if (string.IsNullOrWhiteSpace(txtCustomerId.Text))
            {
                MessageBox.Show("Please enter the Customer ID to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtCustomerId.Text, out int customerId))
            {
                MessageBox.Show("Customer ID must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Step 2: Confirm Update
            var confirmation = MessageBox.Show(
                "Are you sure you want to update this customer?",
                "Confirm Update",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmation == DialogResult.No)
            {
                return; // Exit if the user cancels the operation
            }

            // Step 3: Build Update Query Dynamically
            List<string> updateFields = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(txtFname.Text))
            {
                updateFields.Add("fname = @fname");
                parameters.Add(new SqlParameter("@fname", txtFname.Text));
            }

            if (!string.IsNullOrWhiteSpace(txtLname.Text))
            {
                updateFields.Add("lname = @lname");
                parameters.Add(new SqlParameter("@lname", txtLname.Text));
            }

            if (rbtnMale.Checked || rbtnFemale.Checked)
            {
                string sex = rbtnMale.Checked ? "M" : "F";
                updateFields.Add("sex = @sex");
                parameters.Add(new SqlParameter("@sex", sex));
            }

            if (!string.IsNullOrWhiteSpace(txtAge.Text))
            {
                if (int.TryParse(txtAge.Text, out int age) && age > 0)
                {
                    updateFields.Add("age = @age");
                    parameters.Add(new SqlParameter("@age", age));
                }
                else
                {
                    MessageBox.Show("Age must be a valid positive number greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                updateFields.Add("address = @address");
                parameters.Add(new SqlParameter("@address", txtAddress.Text));
            }


            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                string phone = txtPhone.Text.Trim();

                // Validate the phone number
                if (!IsValidPhone(phone))
                {
                    MessageBox.Show("Please enter a valid phone number (8 to 15 digits only).",
                                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Add phone to update fields
                updateFields.Add("phone = @phone");
                parameters.Add(new SqlParameter("@phone", phone));
            }

            // If no fields are filled, show an error message
            if (updateFields.Count == 0)
            {
                MessageBox.Show("Please fill at least one field to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Combine the update fields into the SQL query
            string updateQuery = "UPDATE Customer SET " + string.Join(", ", updateFields) + " WHERE customer_id = @cid";
            parameters.Add(new SqlParameter("@cid", customerId));

            // Step 4: Execute Update Query
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddRange(parameters.ToArray());

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Customer updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearCustomerFields(); // Optionally clear input fields
                    }
                    else
                    {
                        MessageBox.Show("No customer found with the given ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show($"Database error: {sqlEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                LoadCustomers();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void UpdateCustomer_Load(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Close(); // Close the update form
            
        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCustomerId_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtFname_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtLname_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAge_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
