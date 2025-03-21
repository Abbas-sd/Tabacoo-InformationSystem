﻿using System;
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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            panelUpdateCustomer.Visible = false;

            AddImageToPanel();

            // Disable the search button for Cashier role
            if (log_in.currentUserRole == Main.UserRole.User)
            {
                btncustomeradd.Enabled = false;  // Disable button1 (search button) for Cashier
            }

            if (log_in.currentUserRole == Main.UserRole.Cashier)
            {
                btncustomeradd.Enabled = false;  // Disable button1 (search button) for Cashier
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
        private void ClearCustomerFields()
        {
            txtFname.Clear();
            txtLname.Clear();
            txtAge.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
            rbtnMale.Checked = false;
            rbtnFemale.Checked = false;
        }

        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;"; 
        
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private bool IsValidPhone(string phone)
        {
            // Allow only digits and enforce a length of 10-15 digits
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{8,15}$");
        }
        private async void btncustomeradd_Click(object sender, EventArgs e)
        {
            {
                string fname = txtFname.Text;
                string lname = txtLname.Text;
                string sex = rbtnMale.Checked ? "M" : rbtnFemale.Checked ? "F" : "";
                int age;
                
                string address = txtAddress.Text;
                string phone = txtPhone.Text;

                if (string.IsNullOrEmpty(fname))
                {
                    MessageBox.Show("First name is required.");
                    return;
                }

                if (string.IsNullOrEmpty(lname))
                {
                    MessageBox.Show("Last name is required.");
                    return;
                }

                if (string.IsNullOrEmpty(sex))
                {
                    MessageBox.Show("Please select the customer's gender.");
                    return;
                }

                if (!int.TryParse(txtAge.Text, out age) || age <= 0)
                {
                    MessageBox.Show("Please enter a valid age greater than 0.");
                    return;
                }

                if (string.IsNullOrEmpty(address))
                {
                    MessageBox.Show("Address is required.");
                    return;
                }

                // No phone validation, or you can add custom validation here if needed.
                if (!IsValidPhone(phone))
                {
                    MessageBox.Show("Please enter a valid phone number (8 to 15 digits only).");
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Customer (fname, lname, sex, age, address, phone) " +
                                   "VALUES (@fname, @lname, @sex, @age, @address, @phone)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@fname", fname);
                    command.Parameters.AddWithValue("@lname", lname);
                    command.Parameters.AddWithValue("@sex", sex);
                    command.Parameters.AddWithValue("@age", age);
                    command.Parameters.AddWithValue("@address", address);
                    command.Parameters.AddWithValue("@phone", phone);

                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        MessageBox.Show("Customer added successfully!");
                        ClearCustomerFields();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        // Declare global references to prevent disposal
        private DeletCustomer deletFormInstance;
        private UpdateCustomer updateFormInstance;

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panelUpdateCustomer.Visible = true; // Show the update panel

            // Prevent multiple instances
            if (deletFormInstance == null || deletFormInstance.IsDisposed)
            {
                deletFormInstance = new DeletCustomer();
                deletFormInstance.TopLevel = false;
                deletFormInstance.FormBorderStyle = FormBorderStyle.None;
                deletFormInstance.Dock = DockStyle.Fill;

                panelUpdateCustomer.Controls.Clear();
                panelUpdateCustomer.Controls.Add(deletFormInstance);
                deletFormInstance.Show();
            }
        }

        private void btnUpdateCustomer_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panelUpdateCustomer.Visible = true; // Show the update panel

            // Prevent multiple instances
            if (updateFormInstance == null || updateFormInstance.IsDisposed)
            {
                updateFormInstance = new UpdateCustomer();
                updateFormInstance.TopLevel = false;
                updateFormInstance.FormBorderStyle = FormBorderStyle.None;
                updateFormInstance.Dock = DockStyle.Fill;

                panelUpdateCustomer.Controls.Clear();
                panelUpdateCustomer.Controls.Add(updateFormInstance);
                updateFormInstance.Show();
            }
        }




        private void btnBack_Click(object sender, EventArgs e)
        {
            
            Main form1 = new Main();

            form1.Show();

            this.Close();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearCustomerFields();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
