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
            LoadSupplierOrders();
            txtSearch.TextChanged += new EventHandler(txtSearch_TextChanged); // Attach TextChanged event
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";
        private void LoadSupplierOrders(string searchKeyword = "")
        {
            string query = @"
        SELECT so.supplier_order_id, s.supplier_name, so.order_date, so.total_amount,
               sod.product_id, sod.quantity
        FROM SupplierOrder so
        JOIN Supplier s ON so.supplier_id = s.supplier_id
        JOIN SupplierOrderDetails sod ON so.supplier_order_id = sod.supplier_order_id";  // Join SupplierOrderDetails

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    try
                    {
                        conn.Open();
                        adapter.Fill(dt);
                        dgvOrders.DataSource = dt; // Bind data to DataGridView
                        dgvOrders.ReadOnly = true;
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

        private void View_Orders_Load(object sender, EventArgs e)
        {
            LoadSupplierOrders();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();

            form1.Show();

            this.Close();
        }

        private void View_Orders_TextChanged(object sender, EventArgs e)
        {

        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchKeyword = txtSearch.Text.Trim();
            LoadSupplierOrders(searchKeyword); // Reload orders with search filter
        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
