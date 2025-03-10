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
            this.panelContainer = new System.Windows.Forms.Panel();
            this.btnOpenForm = new System.Windows.Forms.Button();
            this.btnopencustomer = new System.Windows.Forms.Button();
            this.btnopenorders = new System.Windows.Forms.Button();
            this.btnopenview = new System.Windows.Forms.Button();
            this.btnopensearch = new System.Windows.Forms.Button();
            this.btnopenabout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panelContainer
            // 
            this.panelContainer.Location = new System.Drawing.Point(248, 0);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(1275, 679);
            this.panelContainer.TabIndex = 0;
            this.panelContainer.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnOpenForm
            // 
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
            this.btnopenorders.Location = new System.Drawing.Point(12, 111);
            this.btnopenorders.Name = "btnopenorders";
            this.btnopenorders.Size = new System.Drawing.Size(209, 33);
            this.btnopenorders.TabIndex = 3;
            this.btnopenorders.Text = "Orders";
            this.btnopenorders.UseVisualStyleBackColor = true;
            // 
            // btnopenview
            // 
            this.btnopenview.Location = new System.Drawing.Point(12, 161);
            this.btnopenview.Name = "btnopenview";
            this.btnopenview.Size = new System.Drawing.Size(209, 33);
            this.btnopenview.TabIndex = 4;
            this.btnopenview.Text = "View";
            this.btnopenview.UseVisualStyleBackColor = true;
            // 
            // btnopensearch
            // 
            this.btnopensearch.Location = new System.Drawing.Point(12, 214);
            this.btnopensearch.Name = "btnopensearch";
            this.btnopensearch.Size = new System.Drawing.Size(209, 33);
            this.btnopensearch.TabIndex = 5;
            this.btnopensearch.Text = "Search";
            this.btnopensearch.UseVisualStyleBackColor = true;
            // 
            // btnopenabout
            // 
            this.btnopenabout.Location = new System.Drawing.Point(12, 271);
            this.btnopenabout.Name = "btnopenabout";
            this.btnopenabout.Size = new System.Drawing.Size(209, 33);
            this.btnopenabout.TabIndex = 6;
            this.btnopenabout.Text = "About";
            this.btnopenabout.UseVisualStyleBackColor = true;
            // 
            // panel_testing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1523, 720);
            this.Controls.Add(this.btnopenabout);
            this.Controls.Add(this.btnopensearch);
            this.Controls.Add(this.btnopenview);
            this.Controls.Add(this.btnopenorders);
            this.Controls.Add(this.btnopencustomer);
            this.Controls.Add(this.btnOpenForm);
            this.Controls.Add(this.panelContainer);
            this.Name = "panel_testing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "panel_testing";
            this.Load += new System.EventHandler(this.panel_testing_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.Button btnOpenForm;
        private System.Windows.Forms.Button btnopencustomer;
        private System.Windows.Forms.Button btnopenorders;
        private System.Windows.Forms.Button btnopenview;
        private System.Windows.Forms.Button btnopensearch;
        private System.Windows.Forms.Button btnopenabout;
    }
}