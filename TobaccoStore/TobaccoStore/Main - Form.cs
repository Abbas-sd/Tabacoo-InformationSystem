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
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Create an instance of Form2
            Form1 form2 = new Form1();

            // Show Form2
            form2.Show();

            // Optionally, hide Form1
            this.Hide();

        }

        private void Main_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Date: " + DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void gotocustomer_Click(object sender, EventArgs e)
        {
            
           
        }

        private void gotoproduct_Click(object sender, EventArgs e)
        {
           
        }

        private void gotoorder_Click(object sender, EventArgs e)
        {
            
        }

        private void newCustumerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form2 = new Form1();

            form2.Show();

            this.Hide();
        }

        private void newProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            product form2 = new product();

            form2.Show();

            this.Hide();
        }

        private void newOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void viewCustomersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View_Customers form4 = new View_Customers();

            form4.Show();

            this.Hide();
        }

        private void viewProductsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View_Product form5 = new View_Product();

            form5.Show();

            this.Hide();
        }

        private void viewOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
              
        }

        private void searchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            search form7 = new search();

            form7.Show();   

            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void newSupplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Supplier form8 = new Supplier();

            form8.Show();

            this.Hide();
        }

        private void customerOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customer_Order_With_barcode form3 = new Customer_Order_With_barcode();

            form3.Show();

            this.Hide();
        }

        private void supplierOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Suporder form4 = new Suporder();

            form4.Show();

            this.Hide();
        }

        private void addUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            New_User form7 = new New_User();

            form7.Show();

            this.Hide();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This system helps any tobacco store owner to manage his inventory and his customer/supplier relationships in the smoothest and simplest way possible\n\nFeatures:\n-scanning new barcodes \n- add-update-delete customers\n- User authentication",
                    "About This System",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
        }

        private void customersOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View_Customer_Orders form70 = new View_Customer_Orders();

            form70.Show();

            this.Hide();
        }

        private void suppliersOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View_Orders form71 = new View_Orders();

            form71.Show();

            this.Hide();
        }

        private void viewSuppliersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View_Suppliers form72 = new View_Suppliers();

            form72.Show(); 
            
            this.Hide(); 
        }
    }
    
}
