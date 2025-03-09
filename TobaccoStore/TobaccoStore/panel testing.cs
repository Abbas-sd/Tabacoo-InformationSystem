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

        private void btnOpenForm_Click(object sender, EventArgs e)
        {
            panelContainer.Controls.Clear(); // Clear previous content

            Main frm = new Main();  // Replace with your form name
            frm.TopLevel = false;  // Make it behave like a control
            frm.FormBorderStyle = FormBorderStyle.None; // Hide the title bar
            frm.Dock = DockStyle.Fill; // Optional: Fill the panel
            panelContainer.Controls.Add(frm);
            frm.Show();
        }


    }
}
