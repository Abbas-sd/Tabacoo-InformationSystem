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

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            DeletCustomer form1 = new DeletCustomer();

            form1.Show();

            this.Close();
        }

        private void btnUpdateCustomer_Click(object sender, EventArgs e)
        {
            UpdateCustomer form1 = new UpdateCustomer();

            form1.Show();

            this.Close();
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
    }
}
