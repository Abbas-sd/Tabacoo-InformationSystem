namespace TobaccoStore
{
    partial class Customer_Order_With_barcode
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.dataGridViewSale = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.Btnclear = new System.Windows.Forms.Button();
            this.listBoxCustomers = new System.Windows.Forms.ListBox();
            this.dateTimePickerOrderDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearchCustomer = new System.Windows.Forms.TextBox();
            this.btnremove = new System.Windows.Forms.Button();
            this.txtManualBarcode = new System.Windows.Forms.TextBox();
            this.btnManualEntry = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSale)).BeginInit();
            this.SuspendLayout();
            // 
            // txtBarcode
            // 
            this.txtBarcode.Location = new System.Drawing.Point(75, 33);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(100, 22);
            this.txtBarcode.TabIndex = 0;
            this.txtBarcode.TextChanged += new System.EventHandler(this.txtBarcode_TextChanged);
            this.txtBarcode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBarcode_KeyPress);
            // 
            // dataGridViewSale
            // 
            this.dataGridViewSale.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSale.Location = new System.Drawing.Point(201, 60);
            this.dataGridViewSale.Name = "dataGridViewSale";
            this.dataGridViewSale.RowHeadersWidth = 51;
            this.dataGridViewSale.RowTemplate.Height = 24;
            this.dataGridViewSale.Size = new System.Drawing.Size(896, 442);
            this.dataGridViewSale.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(484, 508);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Location = new System.Drawing.Point(27, 369);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(16, 16);
            this.lblTotalAmount.TabIndex = 3;
            this.lblTotalAmount.Text = "...";
            // 
            // Btnclear
            // 
            this.Btnclear.Location = new System.Drawing.Point(286, 512);
            this.Btnclear.Name = "Btnclear";
            this.Btnclear.Size = new System.Drawing.Size(75, 23);
            this.Btnclear.TabIndex = 4;
            this.Btnclear.Text = "Clear";
            this.Btnclear.UseVisualStyleBackColor = true;
            this.Btnclear.Click += new System.EventHandler(this.Btnclear_Click);
            // 
            // listBoxCustomers
            // 
            this.listBoxCustomers.FormattingEnabled = true;
            this.listBoxCustomers.ItemHeight = 16;
            this.listBoxCustomers.Location = new System.Drawing.Point(46, 77);
            this.listBoxCustomers.Name = "listBoxCustomers";
            this.listBoxCustomers.Size = new System.Drawing.Size(120, 84);
            this.listBoxCustomers.TabIndex = 5;
            this.listBoxCustomers.SelectedIndexChanged += new System.EventHandler(this.listBoxCustomers_SelectedIndexChanged);
            // 
            // dateTimePickerOrderDate
            // 
            this.dateTimePickerOrderDate.Location = new System.Drawing.Point(860, 13);
            this.dateTimePickerOrderDate.Name = "dateTimePickerOrderDate";
            this.dateTimePickerOrderDate.Size = new System.Drawing.Size(225, 22);
            this.dateTimePickerOrderDate.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Barcode";
            // 
            // txtSearchCustomer
            // 
            this.txtSearchCustomer.Location = new System.Drawing.Point(46, 167);
            this.txtSearchCustomer.Name = "txtSearchCustomer";
            this.txtSearchCustomer.Size = new System.Drawing.Size(100, 22);
            this.txtSearchCustomer.TabIndex = 8;
            this.txtSearchCustomer.TextChanged += new System.EventHandler(this.txtSearchCustomer_TextChanged);
            // 
            // btnremove
            // 
            this.btnremove.Location = new System.Drawing.Point(184, 512);
            this.btnremove.Name = "btnremove";
            this.btnremove.Size = new System.Drawing.Size(75, 23);
            this.btnremove.TabIndex = 9;
            this.btnremove.Text = "Remove";
            this.btnremove.UseVisualStyleBackColor = true;
            this.btnremove.Click += new System.EventHandler(this.btnremove_Click);
            // 
            // txtManualBarcode
            // 
            this.txtManualBarcode.Location = new System.Drawing.Point(459, 13);
            this.txtManualBarcode.Name = "txtManualBarcode";
            this.txtManualBarcode.Size = new System.Drawing.Size(100, 22);
            this.txtManualBarcode.TabIndex = 10;
            this.txtManualBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtManualBarcode_KeyDown);
            // 
            // btnManualEntry
            // 
            this.btnManualEntry.Location = new System.Drawing.Point(749, 508);
            this.btnManualEntry.Name = "btnManualEntry";
            this.btnManualEntry.Size = new System.Drawing.Size(75, 23);
            this.btnManualEntry.TabIndex = 11;
            this.btnManualEntry.Text = "Manual";
            this.btnManualEntry.UseVisualStyleBackColor = true;
            this.btnManualEntry.Click += new System.EventHandler(this.btnManualEntry_Click);
            this.btnManualEntry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnManualEntry_KeyDown);
            // 
            // Customer_Order_With_barcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1109, 547);
            this.Controls.Add(this.btnManualEntry);
            this.Controls.Add(this.txtManualBarcode);
            this.Controls.Add(this.btnremove);
            this.Controls.Add(this.txtSearchCustomer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePickerOrderDate);
            this.Controls.Add(this.listBoxCustomers);
            this.Controls.Add(this.Btnclear);
            this.Controls.Add(this.lblTotalAmount);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridViewSale);
            this.Controls.Add(this.txtBarcode);
            this.Name = "Customer_Order_With_barcode";
            this.Text = "Customer_Order_With_barcode";
            this.Load += new System.EventHandler(this.Customer_Order_With_barcode_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSale)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.DataGridView dataGridViewSale;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.Button Btnclear;
        private System.Windows.Forms.ListBox listBoxCustomers;
        private System.Windows.Forms.DateTimePicker dateTimePickerOrderDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSearchCustomer;
        private System.Windows.Forms.Button btnremove;
        private System.Windows.Forms.TextBox txtManualBarcode;
        private System.Windows.Forms.Button btnManualEntry;
    }
}