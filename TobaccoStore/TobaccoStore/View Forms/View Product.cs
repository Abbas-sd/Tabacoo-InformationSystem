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
    public partial class View_Product : Form
    {
        public View_Product()
        {
            InitializeComponent();
            txtSearch.TextChanged += new EventHandler(txtSearch_TextChanged); // Attach TextChanged event
            panel2.Visible = false;
        }


        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";

        private void LoadProducts(string searchKeyword = "")
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT * 
                    FROM Product 
                    WHERE product_name LIKE @keyword OR product_type LIKE @keyword OR barcode LIKE @keyword";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@keyword", "%" + searchKeyword + "%");

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvProducts.DataSource = dt;
                dgvProducts.ReadOnly = true;
            }
        }
        private void View_Product_Load(object sender, EventArgs e)
        {
            txtSearch.Focus();
            LoadProducts(); // Load all products initially
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchKeyword = txtSearch.Text.Trim();
            LoadProducts(searchKeyword); // Reload products with search filter
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();

            form1.Show();

            this.Close();
        }

        private DeleteProduct deletFormInstance;
        private UpdateProduct updateFormInstance;
        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            
            panel2.Visible = true; // Show the update panel

            // Prevent multiple instances
            if (deletFormInstance == null || deletFormInstance.IsDisposed)
            {
                deletFormInstance = new DeleteProduct();
                deletFormInstance.TopLevel = false;
                deletFormInstance.FormBorderStyle = FormBorderStyle.None;
                deletFormInstance.Dock = DockStyle.Fill;

                panel2.Controls.Clear();
                panel2.Controls.Add(deletFormInstance);
                deletFormInstance.Show();
            }
        }

        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            
            panel2.Visible = true; // Show the update panel

            // Prevent multiple instances
            if (updateFormInstance == null || updateFormInstance.IsDisposed)
            {
                updateFormInstance = new UpdateProduct();
                updateFormInstance.TopLevel = false;
                updateFormInstance.FormBorderStyle = FormBorderStyle.None;
                updateFormInstance.Dock = DockStyle.Fill;

                panel2.Controls.Clear();
                panel2.Controls.Add(updateFormInstance);
                updateFormInstance.Show();
            }
        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
