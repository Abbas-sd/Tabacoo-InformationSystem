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
    public partial class View_Orders : Form
    {
        public View_Orders()
        {
            InitializeComponent();
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";
        private void LoadOrders()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT o.oid, c.fname + ' ' + c.lname AS customer_name, o.order_status, o.total_amount
                         FROM CustomerOrders o
                         JOIN Customer c ON o.cid = c.cid";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgvOrders.DataSource = dt;
            }
        }

        private void View_Orders_Load(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();

            form1.Show();

            this.Close();
        }
    }
}
