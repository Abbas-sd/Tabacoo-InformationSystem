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
    public partial class View_Customer_Orders : Form
    {
        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";
        public View_Customer_Orders()
        {
            InitializeComponent();
            LoadCustomerOrders();
            txtSearch.TextChanged += new EventHandler(txtSearch_TextChanged); // Attach TextChanged event
        }

        private void View_Customer_Orders_Load(object sender, EventArgs e)
        {

        }
        private void LoadCustomerOrders(string searchKeyword = "")
        {
            string query = @"
                SELECT co.customer_order_id, c.fname, c.lname, co.order_date, co.total_amount
                FROM CustomerOrder co
                JOIN Customer c ON co.customer_id = c.customer_id
                WHERE c.fname LIKE @keyword OR c.lname LIKE @keyword OR co.order_date LIKE @keyword";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + searchKeyword + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    try
                    {
                        conn.Open();
                        adapter.Fill(dt);
                        dgvCustomerOrders.DataSource = dt; // Bind data to DataGridView
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchKeyword = txtSearch.Text.Trim();
            LoadCustomerOrders(searchKeyword); // Reload orders with search filter
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();

            form1.Show();

            this.Close();
        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
