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
        public enum UserRole
        {
            Invalid = 0, // For failed logins
            Admin = 1,   // Full access
            Manager = 2, // Some restricted admin access
            Cashier = 3, // Can process sales but not modify users
            Stocker = 4, // Can manage inventory but not process sales
            User = 5     // Regular customer/user with minimal access
        }


        public Main()
        {
            InitializeComponent();
            this.Icon = new Icon("Tabacoo-Icon2.ico"); // Set icon
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
            ApplyRoleBasedAccessControl();
        }
        private void ApplyRoleBasedAccessControl()
        {
            if (log_in.currentUserRole == UserRole.User)
            {
                addUserToolStripMenuItem.Enabled = false;
            }
            else if (log_in.currentUserRole == UserRole.Admin)
            {
                addUserToolStripMenuItem.Enabled = true;
                
            }
            switch (log_in.currentUserRole)
            {
                case UserRole.Admin:
                    // Admin has full access, enable everything
                    addUserToolStripMenuItem.Enabled = true;
                    btnAdminFeatures.Enabled = true;
                    break;

                case UserRole.Manager:
                    // Manager has limited admin rights (but no user management)
                    adminMenuItem.Enabled = true;
                    addUserToolStripMenuItem.Enabled = false; // Maybe disable user-related actions
                    break;

                case UserRole.Cashier:
                    // Cashier can process sales but not change settings
                    addUserToolStripMenuItem.Enabled = false;
                    btnAdminFeatures.Enabled = false;
                    salesMenuItem.Enabled = true;
                    break;

                case UserRole.Stocker:
                    // Stocker can manage inventory but not sales
                    inventoryMenuItem.Enabled = true;
                    salesMenuItem.Enabled = false;
                    adminMenuItem.Enabled = false;
                    break;

                case UserRole.User:
                    // Regular user has minimal access
                    adminMenuItem.Enabled = false;
                    btnAdminFeatures.Enabled = false;
                    inventoryMenuItem.Enabled = false;
                    salesMenuItem.Enabled = false;
                    break;

                default:
                    // If something goes wrong, disable everything
                    adminMenuItem.Enabled = false;
                    btnAdminFeatures.Enabled = false;
                    inventoryMenuItem.Enabled = false;
                    salesMenuItem.Enabled = false;
                    MessageBox.Show("Access Denied! Contact Administrator.");
                    break;
            }
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
            product_barcode form2 = new product_barcode();

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

        private void button1_Click_1(object sender, EventArgs e)
        {
            log_in form5000 = new log_in();

            form5000.Show();

            this.Hide();
        }
    }

}