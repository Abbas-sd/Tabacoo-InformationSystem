namespace TobaccoStore
{
    partial class panel_testing
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(panel_testing));
            this.panelContainer = new System.Windows.Forms.Panel();
            this.btnOpenForm = new System.Windows.Forms.Button();
            this.btnopencustomer = new System.Windows.Forms.Button();
            this.btnopenorders = new System.Windows.Forms.Button();
            this.btnopenview = new System.Windows.Forms.Button();
            this.btnopensearch = new System.Windows.Forms.Button();
            this.btnopenabout = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelContainer
            // 
            this.panelContainer.BackColor = System.Drawing.Color.DarkGray;
            this.panelContainer.ForeColor = System.Drawing.Color.AntiqueWhite;
            this.panelContainer.Location = new System.Drawing.Point(248, 0);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(1275, 720);
            this.panelContainer.TabIndex = 0;
            this.panelContainer.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnOpenForm
            // 
            this.btnOpenForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenForm.ForeColor = System.Drawing.Color.DarkRed;
            this.btnOpenForm.Location = new System.Drawing.Point(12, 12);
            this.btnOpenForm.Name = "btnOpenForm";
            this.btnOpenForm.Size = new System.Drawing.Size(209, 31);
            this.btnOpenForm.TabIndex = 1;
            this.btnOpenForm.Text = "File";
            this.btnOpenForm.UseVisualStyleBackColor = true;
            this.btnOpenForm.Click += new System.EventHandler(this.btnOpenForm_Click);
            // 
            // btnopencustomer
            // 
            this.btnopencustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnopencustomer.ForeColor = System.Drawing.Color.DarkRed;
            this.btnopencustomer.Location = new System.Drawing.Point(12, 62);
            this.btnopencustomer.Name = "btnopencustomer";
            this.btnopencustomer.Size = new System.Drawing.Size(209, 33);
            this.btnopencustomer.TabIndex = 2;
            this.btnopencustomer.Text = "Managment";
            this.btnopencustomer.UseVisualStyleBackColor = true;
            this.btnopencustomer.Click += new System.EventHandler(this.btnopencustomer_Click);
            // 
            // btnopenorders
            // 
            this.btnopenorders.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnopenorders.ForeColor = System.Drawing.Color.DarkRed;
            this.btnopenorders.Location = new System.Drawing.Point(12, 111);
            this.btnopenorders.Name = "btnopenorders";
            this.btnopenorders.Size = new System.Drawing.Size(209, 33);
            this.btnopenorders.TabIndex = 3;
            this.btnopenorders.Text = "Orders";
            this.btnopenorders.UseVisualStyleBackColor = true;
            // 
            // btnopenview
            // 
            this.btnopenview.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnopenview.ForeColor = System.Drawing.Color.DarkRed;
            this.btnopenview.Location = new System.Drawing.Point(12, 161);
            this.btnopenview.Name = "btnopenview";
            this.btnopenview.Size = new System.Drawing.Size(209, 33);
            this.btnopenview.TabIndex = 4;
            this.btnopenview.Text = "View";
            this.btnopenview.UseVisualStyleBackColor = true;
            // 
            // btnopensearch
            // 
            this.btnopensearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnopensearch.ForeColor = System.Drawing.Color.DarkRed;
            this.btnopensearch.Location = new System.Drawing.Point(12, 214);
            this.btnopensearch.Name = "btnopensearch";
            this.btnopensearch.Size = new System.Drawing.Size(209, 33);
            this.btnopensearch.TabIndex = 5;
            this.btnopensearch.Text = "Search";
            this.btnopensearch.UseVisualStyleBackColor = true;
            // 
            // btnopenabout
            // 
            this.btnopenabout.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnopenabout.ForeColor = System.Drawing.Color.DarkRed;
            this.btnopenabout.Location = new System.Drawing.Point(12, 271);
            this.btnopenabout.Name = "btnopenabout";
            this.btnopenabout.Size = new System.Drawing.Size(209, 33);
            this.btnopenabout.TabIndex = 6;
            this.btnopenabout.Text = "About";
            this.btnopenabout.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.White;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 694);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1523, 26);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(94, 20);
            this.toolStripStatusLabel1.Text = "Current Time";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(82, 20);
            this.toolStripStatusLabel2.Text = "User Name";
            // 
            // panel_testing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cyan;
            this.ClientSize = new System.Drawing.Size(1523, 720);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnopenabout);
            this.Controls.Add(this.btnopensearch);
            this.Controls.Add(this.btnopenview);
            this.Controls.Add(this.btnopenorders);
            this.Controls.Add(this.btnopencustomer);
            this.Controls.Add(this.btnOpenForm);
            this.Controls.Add(this.panelContainer);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "panel_testing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TSIS | Main ";
            this.Load += new System.EventHandler(this.panel_testing_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.Button btnOpenForm;
        private System.Windows.Forms.Button btnopencustomer;
        private System.Windows.Forms.Button btnopenorders;
        private System.Windows.Forms.Button btnopenview;
        private System.Windows.Forms.Button btnopensearch;
        private System.Windows.Forms.Button btnopenabout;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    }
}