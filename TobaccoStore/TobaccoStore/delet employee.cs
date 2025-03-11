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
        }

        private void delet_employee_Load(object sender, EventArgs e)
        {
            LoadEmployeeData(); // Load data when the form loads
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
    }
}
