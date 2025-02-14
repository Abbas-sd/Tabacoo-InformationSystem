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

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";

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
                    query = "SELECT * FROM Customer WHERE fname LIKE @keyword OR lname LIKE @keyword OR phone LIKE @keyword";
                    break;
                case "Supplier":
                    query = "SELECT * FROM Supplier WHERE supplier_name LIKE @keyword OR phone LIKE @keyword";
                    break;
                case "Product":
                    query = "SELECT * FROM Product WHERE product_name LIKE @keyword OR product_type LIKE @keyword";
                    break;
                case "CustomerOrder":
                    query = "SELECT co.customer_order_id, c.fname, c.lname, co.order_date, co.total_amount " +
                            "FROM CustomerOrder co JOIN Customer c ON co.customer_id = c.customer_id " +
                            "WHERE co.customer_order_id LIKE @keyword OR c.fname LIKE @keyword OR c.lname LIKE @keyword";
                    break;
                case "Employee":
                    query = "SELECT * FROM Employee WHERE fname LIKE @keyword OR lname LIKE @keyword OR position LIKE @keyword";
                    break;
                case "SupplierOrder":
                    query = "SELECT so.supplier_order_id, s.supplier_name, so.order_date, so.total_amount " +
                            "FROM SupplierOrder so JOIN Supplier s ON so.supplier_id = s.supplier_id " +
                            "WHERE so.supplier_order_id LIKE @keyword OR s.supplier_name LIKE @keyword";
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

                    try
                    {
                        conn.Open();
                        adapter.Fill(results);
                        dgvSearchResults.DataSource = results; // dgvSearchResults is the DataGridView
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

        private void button2_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();

            form1.Show();

            this.Close();
        }

        private void search_Load(object sender, EventArgs e)
        {

        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Check if the Enter key is pressed
            {
                e.SuppressKeyPress = true; // Prevent the "ding" sound when pressing Enter
                button1_Click(sender, e); // Trigger the search button click event
            }
        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
