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
    public partial class Add_Employee : Form
    {
        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";
        public Add_Employee()
        {
            InitializeComponent();
            AddImageToPanel();
            PopulatePositionComboBox();
            panel2.Visible = false;
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
        private void PopulatePositionComboBox()
        {
            // Add positions to the ComboBox
            cmbPosition.Items.AddRange(new string[]
            {
            "Manager",
            "Cashier",
            "InventorySpecialist",
            "CustomerService",
            "ITSupport",
            "HRManager"
            });

            // Optionally, set a default selected item
            cmbPosition.SelectedIndex = 0; // Select the first item by default
        }
        private void Add_Employee_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Retrieve values from the form controls
            string fname = txtFname.Text;
            string lname = txtLname.Text;
            string sex = rbtnMale.Checked ? "M" : rbtnFemale.Checked ? "F" : "";
            int age;
            string position = cmbPosition.SelectedItem?.ToString();
            decimal salary;
            string phone = txtPhone.Text;
            

            // Retrieve values from the form controls
            DateTime hireDate = dtpHireDate.Value;

            // Validate hire date
            if (hireDate > DateTime.Now)
            {
                MessageBox.Show("Hire date cannot be in the future.");
                return;
            }

            if (hireDate < new DateTime(1900, 1, 1))
            {
                MessageBox.Show("Please enter a valid hire date.");
                return;
            }

            // Validate the data
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
                MessageBox.Show("Please select the employee's gender.");
                return;
            }

            if (!int.TryParse(txtAge.Text, out age) || age < 0)
            {
                MessageBox.Show("Please enter a valid age.");
                return;
            }

            if (string.IsNullOrEmpty(position))
            {
                MessageBox.Show("Please select a position.");
                return;
            }

            if (!decimal.TryParse(txtSalary.Text, out salary) || salary < 0)
            {
                MessageBox.Show("Please enter a valid salary.");
                return;
            }

            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Phone number is required.");
                return;
            }

            // Insert the data into the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    INSERT INTO Employee (fname, lname, sex, age, position, salary, phone, hire_date)
                    VALUES (@fname, @lname, @sex, @age, @position, @salary, @phone, @hireDate)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@fname", fname);
                command.Parameters.AddWithValue("@lname", lname);
                command.Parameters.AddWithValue("@sex", sex);
                command.Parameters.AddWithValue("@age", age);
                command.Parameters.AddWithValue("@position", position);
                command.Parameters.AddWithValue("@salary", salary);
                command.Parameters.AddWithValue("@phone", phone);
                command.Parameters.AddWithValue("@hireDate", hireDate);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Employee added successfully!");
                        ClearForm(); // Clear the form after saving
                    }
                    else
                    {
                        MessageBox.Show("Failed to add employee.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

        }
        private void ClearForm()
        {
            // Clear all form controls
            txtFname.Clear();
            txtLname.Clear();
            txtAge.Clear();
            txtSalary.Clear();
            txtPhone.Clear();
            rbtnMale.Checked = false;
            rbtnFemale.Checked = false;
            cmbPosition.SelectedIndex = -1;
            dtpHireDate.Value = DateTime.Now;
        }

        private delet_employee deletFormInstance;
        private Update_Employee updateFormInstance;
        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true; // Show the update panel

            // Prevent multiple instances
            if (updateFormInstance == null || updateFormInstance.IsDisposed)
            {
                updateFormInstance = new Update_Employee();
                updateFormInstance.TopLevel = false;
                updateFormInstance.FormBorderStyle = FormBorderStyle.None;
                updateFormInstance.Dock = DockStyle.Fill;

                panel2.Controls.Clear();
                panel2.Controls.Add(updateFormInstance);
                updateFormInstance.Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true; // Show the update panel

            // Prevent multiple instances
            if (deletFormInstance == null || deletFormInstance.IsDisposed)
            {
                deletFormInstance = new delet_employee();
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
