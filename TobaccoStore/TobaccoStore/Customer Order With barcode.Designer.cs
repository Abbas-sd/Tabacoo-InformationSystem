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
            this.comboBoxCustomers = new System.Windows.Forms.ComboBox();
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
            this.dataGridViewSale.Size = new System.Drawing.Size(587, 358);
            this.dataGridViewSale.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 415);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Location = new System.Drawing.Point(12, 214);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(44, 16);
            this.lblTotalAmount.TabIndex = 3;
            this.lblTotalAmount.Text = "label1";
            // 
            // Btnclear
            // 
            this.Btnclear.Location = new System.Drawing.Point(669, 12);
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
            this.dateTimePickerOrderDate.Location = new System.Drawing.Point(413, 13);
            this.dateTimePickerOrderDate.Name = "dateTimePickerOrderDate";
            this.dateTimePickerOrderDate.Size = new System.Drawing.Size(200, 22);
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
            // comboBoxCustomers
            // 
            this.comboBoxCustomers.FormattingEnabled = true;
            this.comboBoxCustomers.Location = new System.Drawing.Point(243, 15);
            this.comboBoxCustomers.Name = "comboBoxCustomers";
            this.comboBoxCustomers.Size = new System.Drawing.Size(121, 24);
            this.comboBoxCustomers.TabIndex = 8;
            // 
            // Customer_Order_With_barcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.comboBoxCustomers);
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
        private System.Windows.Forms.ComboBox comboBoxCustomers;
    }
}