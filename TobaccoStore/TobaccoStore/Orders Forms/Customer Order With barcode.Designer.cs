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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Customer_Order_With_barcode));
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
            this.label2 = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPrintPreview = new System.Windows.Forms.Button();
            this.comboBoxPaymentStatus = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSale)).BeginInit();
            this.SuspendLayout();
            // 
            // txtBarcode
            // 
            this.txtBarcode.Location = new System.Drawing.Point(75, 22);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(100, 22);
            this.txtBarcode.TabIndex = 0;
            this.txtBarcode.TextChanged += new System.EventHandler(this.txtBarcode_TextChanged);
            this.txtBarcode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBarcode_KeyPress);
            // 
            // dataGridViewSale
            // 
            this.dataGridViewSale.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridViewSale.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSale.Location = new System.Drawing.Point(215, 63);
            this.dataGridViewSale.Name = "dataGridViewSale";
            this.dataGridViewSale.RowHeadersWidth = 51;
            this.dataGridViewSale.RowTemplate.Height = 24;
            this.dataGridViewSale.Size = new System.Drawing.Size(1030, 569);
            this.dataGridViewSale.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.ForeColor = System.Drawing.Color.DarkRed;
            this.button1.Location = new System.Drawing.Point(1161, 638);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.ForeColor = System.Drawing.Color.Gold;
            this.lblTotalAmount.Location = new System.Drawing.Point(24, 441);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(16, 16);
            this.lblTotalAmount.TabIndex = 3;
            this.lblTotalAmount.Text = "...";
            // 
            // Btnclear
            // 
            this.Btnclear.ForeColor = System.Drawing.Color.DarkRed;
            this.Btnclear.Location = new System.Drawing.Point(218, 638);
            this.Btnclear.Name = "Btnclear";
            this.Btnclear.Size = new System.Drawing.Size(75, 23);
            this.Btnclear.TabIndex = 8;
            this.Btnclear.Text = "Clear";
            this.Btnclear.UseVisualStyleBackColor = true;
            this.Btnclear.Click += new System.EventHandler(this.Btnclear_Click);
            // 
            // listBoxCustomers
            // 
            this.listBoxCustomers.FormattingEnabled = true;
            this.listBoxCustomers.ItemHeight = 16;
            this.listBoxCustomers.Location = new System.Drawing.Point(9, 129);
            this.listBoxCustomers.Name = "listBoxCustomers";
            this.listBoxCustomers.Size = new System.Drawing.Size(183, 84);
            this.listBoxCustomers.TabIndex = 5;
            this.listBoxCustomers.SelectedIndexChanged += new System.EventHandler(this.listBoxCustomers_SelectedIndexChanged);
            // 
            // dateTimePickerOrderDate
            // 
            this.dateTimePickerOrderDate.Location = new System.Drawing.Point(1002, 19);
            this.dateTimePickerOrderDate.Name = "dateTimePickerOrderDate";
            this.dateTimePickerOrderDate.Size = new System.Drawing.Size(243, 22);
            this.dateTimePickerOrderDate.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Gold;
            this.label1.Location = new System.Drawing.Point(12, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Barcode";
            // 
            // txtSearchCustomer
            // 
            this.txtSearchCustomer.Location = new System.Drawing.Point(92, 231);
            this.txtSearchCustomer.Name = "txtSearchCustomer";
            this.txtSearchCustomer.Size = new System.Drawing.Size(100, 22);
            this.txtSearchCustomer.TabIndex = 3;
            this.txtSearchCustomer.TextChanged += new System.EventHandler(this.txtSearchCustomer_TextChanged);
            // 
            // btnremove
            // 
            this.btnremove.ForeColor = System.Drawing.Color.DarkRed;
            this.btnremove.Location = new System.Drawing.Point(333, 638);
            this.btnremove.Name = "btnremove";
            this.btnremove.Size = new System.Drawing.Size(75, 23);
            this.btnremove.TabIndex = 9;
            this.btnremove.Text = "Remove";
            this.btnremove.UseVisualStyleBackColor = true;
            this.btnremove.Click += new System.EventHandler(this.btnremove_Click);
            // 
            // txtManualBarcode
            // 
            this.txtManualBarcode.Location = new System.Drawing.Point(459, 20);
            this.txtManualBarcode.Name = "txtManualBarcode";
            this.txtManualBarcode.Size = new System.Drawing.Size(100, 22);
            this.txtManualBarcode.TabIndex = 1;
            this.txtManualBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtManualBarcode_KeyDown);
            // 
            // btnManualEntry
            // 
            this.btnManualEntry.ForeColor = System.Drawing.Color.DarkRed;
            this.btnManualEntry.Location = new System.Drawing.Point(591, 19);
            this.btnManualEntry.Name = "btnManualEntry";
            this.btnManualEntry.Size = new System.Drawing.Size(75, 23);
            this.btnManualEntry.TabIndex = 2;
            this.btnManualEntry.Text = "Manual";
            this.btnManualEntry.UseVisualStyleBackColor = true;
            this.btnManualEntry.Click += new System.EventHandler(this.btnManualEntry_Click);
            this.btnManualEntry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnManualEntry_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Gold;
            this.label2.Location = new System.Drawing.Point(349, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 16);
            this.label2.TabIndex = 14;
            this.label2.Text = "Manual Eenter";
            // 
            // btnPrint
            // 
            this.btnPrint.ForeColor = System.Drawing.Color.DarkRed;
            this.btnPrint.Location = new System.Drawing.Point(37, 638);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 7;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnPrintPreview
            // 
            this.btnPrintPreview.ForeColor = System.Drawing.Color.DarkRed;
            this.btnPrintPreview.Location = new System.Drawing.Point(15, 587);
            this.btnPrintPreview.Name = "btnPrintPreview";
            this.btnPrintPreview.Size = new System.Drawing.Size(120, 23);
            this.btnPrintPreview.TabIndex = 6;
            this.btnPrintPreview.Text = "Print Preview";
            this.btnPrintPreview.UseVisualStyleBackColor = true;
            this.btnPrintPreview.Click += new System.EventHandler(this.btnPrintPreview_Click);
            // 
            // comboBoxPaymentStatus
            // 
            this.comboBoxPaymentStatus.FormattingEnabled = true;
            this.comboBoxPaymentStatus.Location = new System.Drawing.Point(27, 347);
            this.comboBoxPaymentStatus.Name = "comboBoxPaymentStatus";
            this.comboBoxPaymentStatus.Size = new System.Drawing.Size(121, 24);
            this.comboBoxPaymentStatus.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Gold;
            this.label3.Location = new System.Drawing.Point(31, 237);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 16);
            this.label3.TabIndex = 18;
            this.label3.Text = "Search";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Gold;
            this.label4.Location = new System.Drawing.Point(43, 318);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 16);
            this.label4.TabIndex = 19;
            this.label4.Text = "Payment Status";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Gold;
            this.label5.Location = new System.Drawing.Point(951, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 16);
            this.label5.TabIndex = 20;
            this.label5.Text = "Date";
            // 
            // Customer_Order_With_barcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1257, 673);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxPaymentStatus);
            this.Controls.Add(this.btnPrintPreview);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.label2);
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
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Customer_Order_With_barcode";
            this.Text = "Customer_Order_With_barcode";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Customer_Order_With_barcode_FormClosing);
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPrintPreview;
        private System.Windows.Forms.ComboBox comboBoxPaymentStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}