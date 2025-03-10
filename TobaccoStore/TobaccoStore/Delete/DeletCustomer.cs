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
    public partial class DeletCustomer : Form
    {
        private Form _callingForm; // Store the calling form
        public DeletCustomer(Form callingForm)
        {
            InitializeComponent();
            _callingForm = callingForm; // Store the calling form

            // Disable the search button for Cashier role
            if (log_in.currentUserRole == Main.UserRole.User)
            {
                button1.Enabled = false;  // Disable button1 (search button) for Cashier
            }

            if (log_in.currentUserRole == Main.UserRole.Cashier)
            {
                button1.Enabled = false;  // Disable button1 (search button) for Cashier
            }
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";

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
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerId.Text))
            {
                MessageBox.Show("Please enter a valid Customer ID to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtCustomerId.Text, out int customerId))
            {
                MessageBox.Show("Customer ID must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Step 2: Confirm Deletion
            var confirmation = MessageBox.Show(
                "Are you sure you want to delete this customer?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmation == DialogResult.No)
            {
                return; // Exit if the user cancels the operation
            }

            // Step 3: Perform Delete Operation
            string query = "DELETE FROM Customer WHERE customer_id = @cid";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@cid", customerId);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Customer deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtCustomerId.Clear();
                        
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

        private void DeletCustomer_Load(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Close the delete form
            _callingForm.Show(); // Show the calling form
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
