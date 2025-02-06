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
    public partial class search : Form
    {
        public search()
        {
            InitializeComponent();
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TobaccoStoreIS;Trusted_Connection=True;"; 

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            string searchType = comboBox1.SelectedItem.ToString();
            string keyword = txtSearch.Text.Trim();
            string query = "";

            switch (searchType)
            {
                case "Customer":
                    query = "SELECT * FROM Customer WHERE fname LIKE @keyword OR lname LIKE @keyword OR phone_number LIKE @keyword OR email LIKE @keyword";
                    break;
                case "Supplier":
                    query = "SELECT * FROM Supplier WHERE supname LIKE @keyword OR suporder LIKE @keyword";
                    break;
                case "Product":
                    query = "SELECT * FROM Product WHERE product_name LIKE @keyword OR category LIKE @keyword";
                    break;
                case "Order":
                    query = "SELECT o.oid, c.fname, c.lname, o.order_status, o.total_amount " +
                            "FROM Orders o JOIN Customer c ON o.cid = c.cid " +
                            "WHERE o.oid LIKE @keyword OR c.fname LIKE @keyword OR c.lname LIKE @keyword";
                    break;
                default:
                    MessageBox.Show("Please select a valid search type.");
                    return;
            }

            // Execute query
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable results = new DataTable();

                    conn.Open();
                    adapter.Fill(results);
                    dgvSearchResults.DataSource = results; // dgvSearchResults is the DataGridView
                }
            }
        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();

            form1.Show();

            this.Close();
        }
    }
}
