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
    
    public partial class Update_Employee : Form
    {
        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";
        public Update_Employee()
        {
            InitializeComponent();

            if (log_in.currentUserRole == Main.UserRole.User || log_in.currentUserRole == Main.UserRole.Cashier || log_in.currentUserRole == Main.UserRole.Stoker)
            {
                btnUpdate.Enabled = false;
            }
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
                    dgvupdateemployee.ReadOnly = true;
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
                dgvupdateemployee.DataSource = employeeData; // Bind the DataTable to the DataGridView
            }
            else
            {
                MessageBox.Show("No employee data found.");
            }
        }
        private void Update_Employee_Load(object sender, EventArgs e)
        {
            LoadEmployeeData(); // Load data when the form loads
            dtpHireDate.MaxDate = DateTime.Today;

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        { 

            if (!int.TryParse(textBox1.Text.Trim(), out int employeeID) || employeeID <= 0)
            {
                MessageBox.Show("Invalid employee ID. Please enter a valid number.");
                return;
            }

            // Build the update query dynamically
            List<string> updates = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(txtFname.Text))
            {
                updates.Add("fname = @fname");
                parameters.Add(new SqlParameter("@fname", txtFname.Text.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(txtLname.Text))
            {
                updates.Add("lname = @lname");
                parameters.Add(new SqlParameter("@lname", txtLname.Text.Trim()));
            }
            if (rbtnMale.Checked || rbtnFemale.Checked)
            {
                updates.Add("sex = @sex");
                string sex = rbtnMale.Checked ? "M" : "F";
                parameters.Add(new SqlParameter("@sex", sex));
            }
            if (int.TryParse(txtAge.Text, out int age) && age > 0)
            {
                updates.Add("age = @age");
                parameters.Add(new SqlParameter("@age", age));
            }
            if (cmbPosition.SelectedItem != null)
            {
                updates.Add("position = @position");
                parameters.Add(new SqlParameter("@position", cmbPosition.SelectedItem.ToString()));
            }
            if (decimal.TryParse(txtSalary.Text, out decimal salary) && salary >= 0)
            {
                updates.Add("salary = @salary");
                parameters.Add(new SqlParameter("@salary", salary));
            }
            if (!string.IsNullOrWhiteSpace(txtPhone.Text) && IsValidPhone(txtPhone.Text))
            {
                updates.Add("phone = @phone");
                parameters.Add(new SqlParameter("@phone", txtPhone.Text.Trim()));
            }
            if (dtpHireDate.Value <= DateTime.Now && dtpHireDate.Value >= new DateTime(1900, 1, 1))
            {
                updates.Add("hire_date = @hireDate");
                parameters.Add(new SqlParameter("@hireDate", dtpHireDate.Value));
            }

            // If nothing to update, stop here
            if (updates.Count == 0)
            {
                MessageBox.Show("No changes detected.");
                return;
            }

            // Construct the query
            string query = $"UPDATE Employee SET {string.Join(", ", updates)} WHERE employee_id = @employeeID";
            parameters.Add(new SqlParameter("@employeeID", employeeID));

            // Execute the query
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters.ToArray());
                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Employee updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No changes made.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                ClearForm();
                LoadEmployeeData();
            }
        }



        private bool IsValidPhone(string phone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{8,15}$");
        }
        private void ClearForm()
        {
            textBox1.Clear();
            txtFname.Clear();
            txtLname.Clear();
            txtAge.Clear();
            txtSalary.Clear();
            txtPhone.Clear();
            rbtnMale.Checked = false;
            rbtnFemale.Checked = false;
            cmbPosition.SelectedIndex = -1;
            
            
        }

        private void btnrefresh_Click(object sender, EventArgs e)
        {
            LoadEmployeeData();
        }
    }
}
