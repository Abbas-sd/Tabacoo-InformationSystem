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
    public partial class DeleteSupplier : Form
    {
        private Form _callingForm; // Store the reference to the calling form

        public DeleteSupplier(Form callingForm)
        {
            InitializeComponent();
            _callingForm = _callingForm; // Store the calling form

            // Disable the search button for Cashier role
            if (log_in.currentUserRole == Main.UserRole.User)
            {
                button1.Enabled = false;  // Disable button1 (search button) for Cashier
            }

            if (log_in.currentUserRole == Main.UserRole.Cashier)
            {
                button1.Enabled = false;  // Disable button1 (search button) for Cashier
            }
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";
        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoadSuppliers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Supplier";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dgvSupplier.DataSource = dataTable;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSupplierId.Text))
            {
                MessageBox.Show("Please enter the Supplier ID to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtSupplierId.Text, out int supplierId))
            {
                MessageBox.Show("Supplier ID must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this supplier?",
                                                  "Confirm Delete",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.No) return;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Supplier WHERE supplier_id = @supplierId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@supplierId", supplierId);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Supplier deleted successfully.");
                        
                    }
                    else
                    {
                        MessageBox.Show("No supplier found with the given ID.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                txtSupplierId.Clear();      
                LoadSuppliers();
            }
        }

        private void DeleteSupplier_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Navigate back to the calling form
            if (_callingForm != null)
            {
                _callingForm.Show();
                this.Hide();
            }
            else
            {
                // Default behavior if no calling form is provided
                Supplier form7 = new Supplier();
                form7.Show();
                this.Hide();
            }
        }
    }
}
