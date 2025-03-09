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
    public partial class New_User : Form
    {
        public New_User()
        {
            InitializeComponent();
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=Tabacoostore;Trusted_Connection=True;";

        private void New_User_Load(object sender, EventArgs e)
        {
            // Populate the ComboBox with user roles
            cmbUserRole.Items.Add("Admin");
            cmbUserRole.Items.Add("User");
            cmbUserRole.Items.Add("Stoker");
            cmbUserRole.Items.Add("Cashier");

            // Set default selection
            cmbUserRole.SelectedIndex = 1; // Default to "User"
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string userRole = cmbUserRole.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(userRole))
            {
                MessageBox.Show("Please enter a valid username, password, and select a role.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (RegisterUser(username, password, userRole))
            {
                MessageBox.Show("User registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPassword.Clear();
                txtUsername.Clear();
                cmbUserRole.SelectedIndex = 1; // Reset to "User"
            }
            else
            {
                MessageBox.Show("User registration failed. Username might already exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool RegisterUser(string username, string password, string userRole)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (Username, Password, UserRole) VALUES (@Username, @Password, @UserRole)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);  // Store plain text password (not recommended, use hashing)
                cmd.Parameters.AddWithValue("@UserRole", userRole);

                try
                {
                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Database error: " + ex.Message);
                    return false;
                }
            }
        }
        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            // Toggle password visibility
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btnback_Click(object sender, EventArgs e)
        {
            Main mainForm = new Main();
            mainForm.Show();
            this.Hide();
        }
    }
}
