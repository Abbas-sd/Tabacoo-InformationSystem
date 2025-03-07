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
using static TobaccoStore.Main;

namespace TobaccoStore
{

    public partial class log_in : Form
    {
        public static UserRole currentUserRole; // Use the UserRole enum
        public log_in()
        {
            InitializeComponent();
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=Tabacoostore;Trusted_Connection=True;";

        private void log_in_Load(object sender, EventArgs e)
        {

        }

        // private static string currentUserRole = "";  // Global variable to store the current user's role

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            UserRole role = AuthenticateUser(username, password);

            if (role != UserRole.Invalid) // Ensures valid login
            {
                log_in.currentUserRole = role; // Store role globally

                this.Hide();
                Main mainForm = new Main();
                mainForm.Show();
            }
            else
            {
                lblErrorMessage.Text = "Invalid username or password.";
                lblErrorMessage.Visible = true;
            }
        }



        private UserRole AuthenticateUser(string username, string password)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT UserRole FROM Users WHERE Username = @Username AND PasswordHash = @Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                try
                {
                    con.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int roleValue = Convert.ToInt32(result);
                        return (UserRole)roleValue; // Convert to UserRole enum
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database error: " + ex.Message);
                }
            }
            return UserRole.Invalid; // Return Invalid (0) if login fails
        }


        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            // Toggle password visibility
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}