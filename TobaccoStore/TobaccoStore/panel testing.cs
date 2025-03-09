using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TobaccoStore
{
    public partial class panel_testing : Form
    {
        public panel_testing()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private Button btn1;
        private Button btn2;
        private Point secondButtonOriginalLocation;
        private void btnOpenForm_Click(object sender, EventArgs e)
        {
            int spacing = 5; // Space between buttons

            if (btn1 == null || btn2 == null)
            {
                // Create First Button
                btn1 = new Button();
                btn1.Text = "Button 1";
                btn1.Size = new Size(100, 30);
                btn1.Location = new Point(btnOpenForm.Location.X, btnOpenForm.Location.Y + btnOpenForm.Height + spacing);
                btn1.Click += Btn1_Click;

                // Create Second Button
                btn2 = new Button();
                btn2.Text = "Button 2";
                btn2.Size = new Size(100, 30);
                btn2.Location = new Point(btnOpenForm.Location.X, btn1.Location.Y + btn1.Height + spacing);
                btn2.Click += Btn2_Click;

                // Add buttons to the form
                this.Controls.Add(btn1);
                this.Controls.Add(btn2);
            }

            // Toggle visibility of the new buttons
            bool buttonsVisible = !btn1.Visible;
            btn1.Visible = buttonsVisible;
            btn2.Visible = buttonsVisible;

            // Move the second button down when showing new buttons
            if (buttonsVisible)
            {
                btnopencustomer.Location = new Point(secondButtonOriginalLocation.X, secondButtonOriginalLocation.Y + btn1.Height + btn2.Height + (2 * spacing));
            }
            else
            {
                // Move it back to its original position
                btnopencustomer.Location = secondButtonOriginalLocation;
            }
        }
        private void Btn1_Click(object sender, EventArgs e)
        {
            panelContainer.Controls.Clear(); // Clear previous content

            Form1 frm = new Form1();  // Replace with your form name
            frm.TopLevel = false;  // Make it behave like a control
            frm.FormBorderStyle = FormBorderStyle.None; // Hide the title bar
            frm.Dock = DockStyle.Fill; // Optional: Fill the panel
            panelContainer.Controls.Add(frm);
            frm.Show();
        }

        private void Btn2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Button 2 Clicked");
        }
        private void btnopencustomer_Click(object sender, EventArgs e)
        {
            panelContainer.Controls.Clear(); // Clear previous content

            Form1 frm = new Form1();  // Replace with your form name
            frm.TopLevel = false;  // Make it behave like a control
            frm.FormBorderStyle = FormBorderStyle.None; // Hide the title bar
            frm.Dock = DockStyle.Fill; // Optional: Fill the panel
            panelContainer.Controls.Add(frm);
            frm.Show();
        }

        private void panel_testing_Load(object sender, EventArgs e)
        {
            // Store the original location of the second button
            secondButtonOriginalLocation = btnopencustomer.Location;
        }
    }
}
