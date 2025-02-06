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
        private void View_Customers_Load(object sender, EventArgs e)
        {
            LoadCustomers();
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

        private void btnUpdateCustomer_Click(object sender, EventArgs e)
        {
            UpdateCustomer form12 = new UpdateCustomer();

            form12.Show();

            this.Hide();
        }

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            DeletCustomer form13 = new DeletCustomer();

            form13.Show();

            this.Hide();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
