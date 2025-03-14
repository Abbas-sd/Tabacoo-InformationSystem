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
    public partial class delet_employee : Form
    {
        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";
        public delet_employee()
        {
            InitializeComponent();

            if (log_in.currentUserRole == Main.UserRole.User || log_in.currentUserRole == Main.UserRole.Cashier || log_in.currentUserRole == Main.UserRole.Stoker)
            {
                button1.Enabled = false;  // Disable button1 (search button) for Cashier
            }
        }

        private void delet_employee_Load(object sender, EventArgs e)
        {
            LoadEmployeeData(); // Load data when the form loads
            txtEmployeeID.Focus();
        }
        private DataTable GetEmployeeData()
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT employee_id, fname, lname, sex, age, position, salary, phone, hire_date FROM Employee";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable); // Fill the DataTable with the query results
                    dgvdeletemployee.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return dataTable;
        }
        private void LoadEmployeeData()
        {
            DataTable employeeData = GetEmployeeData(); // Fetch data from the database

            if (employeeData != null && employeeData.Rows.Count > 0)
            {
                dgvdeletemployee.DataSource = employeeData; // Bind the DataTable to the DataGridView
            }
            else
            {
                MessageBox.Show("No employee data found.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtEmployeeID.Text.Trim(), out int employeeID) || employeeID <= 0)
            {
                MessageBox.Show("Invalid Employee ID. Please enter a valid number.");
                return;
            }

            // Confirmation dialog
            DialogResult confirm = MessageBox.Show($"Are you sure you want to delete Employee ID {employeeID}?",
                                                    "Confirm Deletion",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Warning);

            if (confirm == DialogResult.No)
            {
                return; // Stop if user cancels
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Employee WHERE employee_id = @employeeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@employeeID", employeeID);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Employee deleted successfully!");
                            LoadEmployeeData(); // Refresh DataGridView
                        }
                        else
                        {
                            MessageBox.Show("Employee ID not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            GetEmployeeData();
        }

        private void txtEmployeeID_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
