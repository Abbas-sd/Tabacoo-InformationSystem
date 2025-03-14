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

namespace TobaccoStore.View_Forms
{
    public partial class View_Employee : Form
    {
        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";

        public View_Employee()
        {
            InitializeComponent();
            panel1.Visible = false; // Show the update panel
        }

        private void View_Employee_Load(object sender, EventArgs e)
        {
            LoadEmployeeData(); // Load initial data
            txtSearch.Focus();
        }

        private DataTable GetEmployeeData(string searchQuery = "")
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT employee_id, fname, lname, sex, age, position, salary, phone, hire_date FROM Employee";

                // If there is a search query, add a WHERE clause
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query += " WHERE employee_id LIKE @search OR fname LIKE @search OR lname LIKE @search OR position LIKE @search";
                }

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@search", "%" + searchQuery + "%"); // Use the search query in the WHERE clause

                try
                {
                    connection.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable); // Fill the DataTable with the query results
                    dgvViewemployee.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return dataTable;
        }

        private void LoadEmployeeData(string searchQuery = "")
        {
            DataTable employeeData = GetEmployeeData(searchQuery); // Fetch data from the database

            if (employeeData != null && employeeData.Rows.Count > 0)
            {
                dgvViewemployee.DataSource = employeeData; // Bind the DataTable to the DataGridView
            }
            else
            {
                MessageBox.Show("No employee data found.");
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchQuery = txtSearch.Text.Trim(); // Get the text from the search box
            LoadEmployeeData(searchQuery); // Load the filtered data based on the search query
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchQuery = txtSearch.Text.Trim(); // Get the current text from the search box
            LoadEmployeeData(searchQuery); // Load the filtered data based on the search query
        }

        private delet_employee deletFormInstance;
        private Update_Employee updateFormInstance;
        private void btnUpdateCustomer_Click(object sender, EventArgs e)
        {
            panel1.Visible = true; // Show the update panel

            // Prevent multiple instances
            if (updateFormInstance == null || updateFormInstance.IsDisposed)
            {
                updateFormInstance = new Update_Employee();
                updateFormInstance.TopLevel = false;
                updateFormInstance.FormBorderStyle = FormBorderStyle.None;
                updateFormInstance.Dock = DockStyle.Fill;

                panel1.Controls.Clear();
                panel1.Controls.Add(updateFormInstance);
                updateFormInstance.Show();
            }
        }
        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            panel1.Visible = true; // Show the update panel

            // Prevent multiple instances
            if (deletFormInstance == null || deletFormInstance.IsDisposed)
            {
                deletFormInstance = new delet_employee();
                deletFormInstance.TopLevel = false;
                deletFormInstance.FormBorderStyle = FormBorderStyle.None;
                deletFormInstance.Dock = DockStyle.Fill;

                panel1.Controls.Clear();
                panel1.Controls.Add(deletFormInstance);
                deletFormInstance.Show();
            }
        }
    }
}
