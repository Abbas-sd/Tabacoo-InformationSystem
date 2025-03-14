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
    public partial class View_Customers : Form
    {
        public View_Customers()
        {
            InitializeComponent();
            txtSearch.TextChanged += new EventHandler(txtSearch_TextChanged); // Attach TextChanged event
            panel2.Visible = false; // Show the update panel
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";

        private void LoadCustomers(string searchKeyword = "")
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT * 
                    FROM Customer 
                    WHERE fname LIKE @keyword OR lname LIKE @keyword OR customer_id LIKE @keyword OR address LIKE @keyword";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@keyword", "%" + searchKeyword + "%");

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dgvCustomers.DataSource = dataTable;

                dgvCustomers.ReadOnly = true;
            }
        }
        private void View_Customers_Load(object sender, EventArgs e)
        {
            LoadCustomers(); // Load all customers initially
            txtSearch.Focus();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchKeyword = txtSearch.Text.Trim();
            LoadCustomers(searchKeyword); // Reload customers with search filter
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();
            form1.Show();
            this.Close();
        }

        

        private void btncustomeradd_Click(object sender, EventArgs e)
        {
            Form1 form11 = new Form1();

            form11.Show();

            this.Hide();
        }

        // Declare global references to prevent disposal
        private DeletCustomer deletFormInstance;
        private UpdateCustomer updateFormInstance;
        private void btnUpdateCustomer_Click(object sender, EventArgs e)
        {
            panel2.Visible = true; // Show the update panel

            // Prevent multiple instances
            if (updateFormInstance == null || updateFormInstance.IsDisposed)
            {
                updateFormInstance = new UpdateCustomer();
                updateFormInstance.TopLevel = false;
                updateFormInstance.FormBorderStyle = FormBorderStyle.None;
                updateFormInstance.Dock = DockStyle.Fill;

                panel2.Controls.Clear();
                panel2.Controls.Add(updateFormInstance);
                updateFormInstance.Show();
            }
        }
        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            panel2.Visible = true; // Show the update panel

            // Prevent multiple instances
            if (deletFormInstance == null || deletFormInstance.IsDisposed)
            {
                deletFormInstance = new DeletCustomer();
                deletFormInstance.TopLevel = false;
                deletFormInstance.FormBorderStyle = FormBorderStyle.None;
                deletFormInstance.Dock = DockStyle.Fill;

                panel2.Controls.Clear();
                panel2.Controls.Add(deletFormInstance);
                deletFormInstance.Show();
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
