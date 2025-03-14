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
            panel1.Visible = false;
        }

        private void View_Suppliers_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            txtSearch.Focus();
        }
        private void LoadSuppliers(string searchKeyword = "")
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT * 
            FROM Supplier 
            WHERE supplier_name LIKE @keyword OR supplier_id LIKE @keyword OR address LIKE @keyword";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@keyword", "%" + searchKeyword + "%");

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvSuppliers.DataSource = dt;
                dgvSuppliers.ReadOnly = true;
            }

            // Set the width of all columns to 80 (or any other default)
            foreach (DataGridViewColumn column in dgvSuppliers.Columns)
            {
                column.Width = 80;
            }

            // Set the width of the last column to 150
            if (dgvSuppliers.Columns.Count > 0)
            {
                dgvSuppliers.Columns[dgvSuppliers.Columns.Count - 1].Width = 250;
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

        private DeleteSupplier deletFormInstance;
        private UpdateSupplier updateFormInstance;
        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            

            // Prevent multiple instances
            if (deletFormInstance == null || deletFormInstance.IsDisposed)
            {
                deletFormInstance = new DeleteSupplier();
                deletFormInstance.TopLevel = false;
                deletFormInstance.FormBorderStyle = FormBorderStyle.None;
                deletFormInstance.Dock = DockStyle.Fill;

                panel1.Controls.Clear();
                panel1.Controls.Add(deletFormInstance);
                deletFormInstance.Show();
            }
        }

        private void btnUpdateSupplier_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel1.Visible = true; // Show the update panel

            // Prevent multiple instances
            if (updateFormInstance == null || updateFormInstance.IsDisposed)
            {
                updateFormInstance = new UpdateSupplier();
                updateFormInstance.TopLevel = false;
                updateFormInstance.FormBorderStyle = FormBorderStyle.None;
                updateFormInstance.Dock = DockStyle.Fill;

                panel1.Controls.Clear();
                panel1.Controls.Add(updateFormInstance);
                updateFormInstance.Show();
            }
        }
    }
}
