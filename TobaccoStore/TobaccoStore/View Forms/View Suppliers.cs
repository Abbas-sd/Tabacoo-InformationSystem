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
    public partial class View_Suppliers : Form
    {
        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";
        public View_Suppliers()
        {
            InitializeComponent();
            txtSearch.TextChanged += new EventHandler(txtSearch_TextChanged); // Attach TextChanged event
        }

        private void View_Suppliers_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
        }
        private void LoadSuppliers(string searchKeyword = "")
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT * 
                    FROM Supplier 
                    WHERE supplier_name LIKE @keyword OR phone LIKE @keyword OR address LIKE @keyword";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@keyword", "%" + searchKeyword + "%");

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvSuppliers.DataSource = dt;
            }
        }
       

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchKeyword = txtSearch.Text.Trim();
            LoadSuppliers(searchKeyword); // Reload suppliers with search filter
        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main form8 = new Main();

            form8.Show();

            this.Hide();

        }

        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            DeleteSupplier form1 = new DeleteSupplier(); // Pass 'this' as the calling form
            form1.Show();
            this.Hide();
        }

        private void btnUpdateSupplier_Click(object sender, EventArgs e)
        {
            UpdateSupplier form2 = new UpdateSupplier(); // Pass 'this' as the calling form
            form2.Show();
            this.Hide();
        }
    }
}
