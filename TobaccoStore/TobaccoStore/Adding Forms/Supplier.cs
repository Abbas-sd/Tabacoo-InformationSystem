using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TobaccoStore
{
    public partial class Supplier : Form
    {
        public Supplier()
        {
            InitializeComponent();
            panel2.Visible = false; // Show the update panel
            AddImageToPanel();
            // Disable the search button for Cashier role
            if (log_in.currentUserRole == Main.UserRole.User)
            {
                btnsupplieradd.Enabled = false;  // Disable button1 (search button) for Cashier
            }

            if (log_in.currentUserRole == Main.UserRole.Cashier)
            {
                btnsupplieradd.Enabled = false;  // Disable button1 (search button) for Cashier
            }
        }

        private void AddImageToPanel()
        {
            // Create PictureBox
            PictureBox pictureBox = new PictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Image = Image.FromFile("C:\\Users\\abbas\\Source\\Repos\\Tabacoo-InformationSystem\\TobaccoStore\\TobaccoStore\\Resources\\tabacoo.jpg"); // Change the path to your image
            pictureBox.Size = new Size(200, 150); // Set size
            pictureBox.Location = new Point(panel1.Width - pictureBox.Width - 0, 0); // Position at the right

            // Add to panel
            panel1.Controls.Add(pictureBox);
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";
        private void Supplier_Load(object sender, EventArgs e)
        {

        }
        private bool IsValidPhone(string phone)
        {
            // Allow only digits and enforce a length of 10-15 digits
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{8,15}$");
        }

        private void ClearSupplierFields()
        {
            txtsuplliername.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
        }
        private void btnsupplieradd_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtsuplliername.Text))
            {
                MessageBox.Show("Supplier Name required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtAddress.Text))
            {
                MessageBox.Show("Address is required.");
                return;
            }

            if (!IsValidPhone(txtPhone.Text))
            {
                MessageBox.Show("Please enter a valid phone number (8 to 15 digits).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Supplier (supplier_name, Phone, Address) VALUES (@fname, @phone, @address)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@fname", txtsuplliername.Text);
                command.Parameters.AddWithValue("@phone", txtPhone.Text);
                command.Parameters.AddWithValue("@address", txtAddress.Text);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Supplier added successfully.");
                        ClearSupplierFields();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add supplier.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();

            form1.Show();

            this.Hide();
        }

        // Declare global references to prevent disposal
        private DeleteSupplier deletFormInstance;
        private UpdateSupplier updateFormInstance; // Ensure correct class name

        private void btnUpdatessupplier_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true; // Show the update panel

            // Prevent multiple instances
            if (updateFormInstance == null || updateFormInstance.IsDisposed)
            {
                updateFormInstance = new UpdateSupplier();
                updateFormInstance.TopLevel = false;
                updateFormInstance.FormBorderStyle = FormBorderStyle.None;
                updateFormInstance.Dock = DockStyle.Fill;

                panel2.Controls.Clear();
                panel2.Controls.Add(updateFormInstance);
                updateFormInstance.Show();
            }
        }

        private void btnDeletesupplier_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true; // Show the update panel

            // Prevent multiple instances
            if (deletFormInstance == null || deletFormInstance.IsDisposed)
            {
                deletFormInstance = new DeleteSupplier();
                deletFormInstance.TopLevel = false;
                deletFormInstance.FormBorderStyle = FormBorderStyle.None;
                deletFormInstance.Dock = DockStyle.Fill;

                panel2.Controls.Clear();
                panel2.Controls.Add(deletFormInstance);
                deletFormInstance.Show();
            }
        }

       
    }
}
