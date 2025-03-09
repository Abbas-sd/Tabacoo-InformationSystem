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
            this.SuspendLayout();
            // 
            // panelContainer
            // 
            this.panelContainer.Location = new System.Drawing.Point(299, 83);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(312, 226);
            this.panelContainer.TabIndex = 0;
            this.panelContainer.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnOpenForm
            // 
            this.btnOpenForm.Location = new System.Drawing.Point(172, 360);
            this.btnOpenForm.Name = "btnOpenForm";
            this.btnOpenForm.Size = new System.Drawing.Size(75, 23);
            this.btnOpenForm.TabIndex = 1;
            this.btnOpenForm.Text = "button1";
            this.btnOpenForm.UseVisualStyleBackColor = true;
            this.btnOpenForm.Click += new System.EventHandler(this.btnOpenForm_Click);
            // 
            // panel_testing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnOpenForm);
            this.Controls.Add(this.panelContainer);
            this.Name = "panel_testing";
            this.Text = "panel_testing";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.Button btnOpenForm;
    }
}