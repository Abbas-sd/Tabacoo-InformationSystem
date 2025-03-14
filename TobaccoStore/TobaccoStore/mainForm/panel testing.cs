using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TobaccoStore.View_Forms;
using static TobaccoStore.Main;

namespace TobaccoStore
{
    public partial class panel_testing : Form
    {
        private Button NewLogoutButton;
        private Button NewExitButton;


        public panel_testing()
        {
            InitializeComponent();
            this.Resize += Panel_testing_Resize; // Handle form resizing
            ApplyRoleBasedAccessControl();
        }

        private void Panel_testing_Resize(object sender, EventArgs e)
        {
            // Recalculate the bottom position for the new buttons when the form is resized
            int bottomPadding = 30; // Padding from the bottom edge
            int buttonHeight = 25; // Height of the buttons
            int buttonSpacing = 10; // Spacing between the buttons

            NewLogoutButton.Location = new Point(10, this.ClientSize.Height - buttonHeight - bottomPadding);
            NewExitButton.Location = new Point(NewLogoutButton.Right + buttonSpacing, this.ClientSize.Height - buttonHeight - bottomPadding);
            this.Refresh(); // Force the form to redraw with updated positions
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        
        private void btnOpenForm_Click(object sender, EventArgs e)
        {
        }
        private void btnopencustomer_Click(object sender, EventArgs e)
        {
        }

        private void ApplyRoleBasedAccessControl()
        {
            
        }





        




        // Dictionary to store sub-buttons for each main button
        private Dictionary<Button, List<Button>> buttonGroups = new Dictionary<Button, List<Button>>();

        // Original locations of main buttons
        private Dictionary<Button, Point> originalPositions = new Dictionary<Button, Point>();
        private void panel_testing_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Date: " + DateTime.Now.ToString("yyyy-MM-dd");

            // Display the username in the StatusStrip
            if (!string.IsNullOrEmpty(log_in.currentUsername))
            {
                toolStripStatusLabel2.Text = "Logged in as: " + log_in.currentUsername;
            }

            ApplyRoleBasedAccessControl();



            // Calculate the bottom position for the buttons
            int bottomPadding = 30; // Padding from the bottom edge
            int buttonHeight = 25; // Height of the buttons
            int buttonSpacing = 10; // Spacing between the buttons

            // Define the dark red color
            Color darkRed = Color.FromArgb(139, 0, 0); // Dark red color

            // Initialize NewLogoutButton
            NewLogoutButton = new Button
            {
                Text = "Logout",
                Size = new Size(80, buttonHeight), // Smaller size
                Location = new Point(10, this.ClientSize.Height - buttonHeight - bottomPadding), // Bottom-left position
                Visible = true,
                ForeColor = darkRed // Set text color to dark red
            };
            NewLogoutButton.Click += NewLogoutButton_Click; // Attach the new logout click event

            // Initialize NewExitButton
            NewExitButton = new Button
            {
                Text = "Exit",
                Size = new Size(80, buttonHeight), // Smaller size
                Location = new Point(NewLogoutButton.Right + buttonSpacing, this.ClientSize.Height - buttonHeight - bottomPadding), // Bottom-left position (next to New Logout)
                Visible = true,
                ForeColor = darkRed // Set text color to dark red
            };
            NewExitButton.Click += NewExitButton_Click; // Attach the new exit click event

            // Add new buttons to the form
            this.Controls.Add(NewLogoutButton);
            this.Controls.Add(NewExitButton);



            // Store original positions of main buttons
            foreach (var button in new[] { btnOpenForm, btnopencustomer, btnopenorders, btnopenview, btnopensearch, btnopenabout })
            {
                originalPositions[button] = button.Location;
                button.Click += MainButton_Click; // Attach click event dynamically
            }

            // Initialize button groups
            buttonGroups[btnOpenForm] = CreateSubButtons(btnOpenForm, new string[] { "Add User" , "New Employee"});
            buttonGroups[btnopencustomer] = CreateSubButtons(btnopencustomer, new string[] { "New Customer", "New Product", "New Supplier" });
            buttonGroups[btnopenorders] = CreateSubButtons(btnopenorders, new string[] { "Customer Order", "Supplier Order" });
            buttonGroups[btnopenview] = CreateSubButtons(btnopenview, new string[] { "View Customers", "View Products", "View Suppliers", "View Employee", "View Customer Orders", "View Supplier Orders" });
            buttonGroups[btnopensearch] = new List<Button>(); // No sub-buttons for Search
            buttonGroups[btnopenabout] = new List<Button>(); // No sub-buttons for About


        }

        // Create sub-button dynamically
        private List<Button> CreateSubButtons(Button parentButton, string[] buttonNames)
        {
            int spacing = 5;
            int startY = parentButton.Location.Y + parentButton.Height + spacing;
            List<Button> subButtons = new List<Button>();

            foreach (string name in buttonNames)
            {
                Button btn = new Button
                {
                    Text = name,
                    Size = new Size(parentButton.Width - 20, 25), // Decrease width by 20 and height to 25
                    Location = new Point(parentButton.Location.X + 10, startY), // Adjust X position for alignment
                    Visible = false,
                    Enabled = HasPermissionForSubButton(name) // Apply role-based restriction
                };

                // Attach click event
                btn.Click += (sender, e) => SubButton_Click(name);

                this.Controls.Add(btn);
                subButtons.Add(btn);
                startY += btn.Height + spacing;
            }

            return subButtons;
        }
        private bool HasPermissionForSubButton(string buttonName)
        {
            switch (log_in.currentUserRole)
            {
                case UserRole.Admin:
                    return true;

                case UserRole.User:
                    return buttonName != "Add User" && buttonName != "New Employee";

                case UserRole.Cashier:
                    return buttonName == "Customer Order" || buttonName.Contains("View"); 

                case UserRole.Stoker:
                    return buttonName.Contains("View") || buttonName == "About" || buttonName.Contains("New") && buttonName != "New Employee";


                default:
                    return false;
            }
        }

        private void SubButton_Click(string buttonName)
        {
            panelContainer.Controls.Clear(); // Clear previous content

            if (buttonName == "About")
            {
                // Show the "About" message box when the About button is clicked
                MessageBox.Show("This system helps any tobacco store owner to manage his inventory and his customer/supplier relationships in the smoothest and simplest way possible" +
                    "\n\nFeatures:" +
                    "\n- scanning new barcodes" +
                    "\n- add-update-delete customers" +
                    "\n- User authentication",
                    "About This System",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return; // No need to open any form for the About button
            }

            Form formToOpen = null;

            switch (buttonName)
            {
                case "Add User":
                    formToOpen = new New_User();
                    break;
                case "New Employee":
                    formToOpen = new Add_Employee();
                    break;
                case "New Customer":
                    formToOpen = new Form1(); // Replace with actual form class
                    break;
                case "New Product":
                    formToOpen = new product_barcode(); // Replace with actual form class
                    break;
                case "New Supplier":
                    formToOpen = new Supplier(); // Replace with actual form class
                    break;
                case "Customer Order":
                    formToOpen = new Customer_Order_With_barcode(); // Replace with actual form class
                    break;
                case "Supplier Order":
                    formToOpen = new Suporder(); // Replace with actual form class
                    break;
                case "View Customers":
                    formToOpen = new View_Customers(); // Replace with actual form class
                    break;
                case "View Products":
                    formToOpen = new View_Product(); // Replace with actual form class
                    break;
                case "View Suppliers":
                    formToOpen = new View_Suppliers(); // Replace with actual form class
                    break;
                case "View Employee":
                    formToOpen = new View_Employee(); // Replace with actual form class
                    break;
                case "View Customer Orders":
                    formToOpen = new View_Customer_Orders(); // Replace with actual form class
                    break;
                case "View Supplier Orders":
                    formToOpen = new View_Orders(); // Replace with actual form class
                    break;
                default:
                    MessageBox.Show("No form assigned for this button.");
                    return;
            }

            if (formToOpen != null)
            {
                formToOpen.TopLevel = false;
                formToOpen.FormBorderStyle = FormBorderStyle.None;
                formToOpen.Dock = DockStyle.Fill;
                panelContainer.Controls.Add(formToOpen);
                formToOpen.Show();
            }
        }


        private void MainButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            // Handle the "About" button directly
            if (clickedButton == btnopenabout)
            {
                MessageBox.Show("This system helps any tobacco store owner to manage his inventory and his customer/supplier relationships in the smoothest and simplest way possible" +
                    "\n\nFeatures:" +
                    "\n- scanning new barcodes" +
                    "\n- add-update-delete customers" +
                    "\n- User authentication",
                    "About This System",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return; // Exit here since no form needs to be opened
            }

            // Handle the Search button separately
            if (clickedButton == btnopensearch)
            {
                // Clear previous content in the panelContainer
                panelContainer.Controls.Clear();

                // Create an instance of the form you want to open (replace "SearchForm" with your actual form class)
                Form searchForm = new search(); // Replace "SearchForm" with your form's class name

                // Set the form properties to make it behave like a control
                searchForm.TopLevel = false; // Make it non-top-level
                searchForm.FormBorderStyle = FormBorderStyle.None; // Remove the border
                searchForm.Dock = DockStyle.Fill; // Fill the panelContainer

                // Add the form to the panelContainer
                panelContainer.Controls.Add(searchForm);

                // Show the form
                searchForm.Show();
                return; // Exit the method after handling the Search button
            }

            // Handle other buttons (existing logic)
            foreach (var group in buttonGroups)
            {
                bool subButtonVisible = group.Value.Any() && !group.Value[0].Visible; // Check if there are any sub-buttons
                SetSubButtonsVisibility(group.Value, group.Key == clickedButton && subButtonVisible);
            }


            AdjustButtonPositions();
        }

        private void SetSubButtonsVisibility(List<Button> buttons, bool visible)
        {
            if (buttons == null || !buttons.Any())
            {
                // If the list is null or empty, simply return early
                return;
            }

            int spacing = 5;
            int startY = buttons.First().Location.Y;

            foreach (var btn in buttons)
            {
                btn.Visible = visible;
                if (visible)
                {
                    btn.Location = new Point(btn.Location.X, startY);
                    startY += btn.Height + spacing;
                }
            }
        }

        private void AdjustButtonPositions()
        {
            int yOffset = 0;

            foreach (var button in originalPositions.Keys)
            {
                // Skip Btnlogout and Btnexit
                if (button == NewLogoutButton || button == NewExitButton)
                    continue;

                button.Location = new Point(originalPositions[button].X, originalPositions[button].Y + yOffset);

                if (buttonGroups.ContainsKey(button) && buttonGroups[button].Any(b => b.Visible))
                {
                    yOffset += buttonGroups[button].Count * (buttonGroups[button][0].Height + 5);
                }
            }
        }

        // Event handlers for the new buttons
        private void NewLogoutButton_Click(object sender, EventArgs e)
        {
            log_in form5000 = new log_in();
            form5000.Show();
            this.Hide();
        }

        private void NewExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
    
}
