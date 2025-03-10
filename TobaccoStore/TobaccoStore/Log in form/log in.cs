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
        public static string currentUsername; // Store the username
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            UserRole role = AuthenticateUser(username, password);

            if (role != UserRole.Invalid) // Ensure the role is valid before proceeding
            {
                log_in.currentUserRole = role; // Store the role for later access control
                log_in.currentUsername = username; // Store the username

                this.Hide();
                panel_testing mainForm = new panel_testing();
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
                string query = "SELECT UserRole FROM Users WHERE Username = @Username AND Password = @Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                try
                {
                    con.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return (UserRole)Enum.Parse(typeof(UserRole), result.ToString()); // Convert role string to Enum
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database error: " + ex.Message);
                }
            }
            return UserRole.Invalid; // Return Invalid if login fails
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