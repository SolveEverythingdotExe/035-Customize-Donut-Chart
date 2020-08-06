namespace MainApplication
{
    partial class MainForm
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
            MainApplication.Slice slice1 = new MainApplication.Slice();
            MainApplication.Slice slice2 = new MainApplication.Slice();
            MainApplication.Slice slice3 = new MainApplication.Slice();
            MainApplication.Slice slice4 = new MainApplication.Slice();
            this.donutChart1 = new MainApplication.DonutChart();
            this.SuspendLayout();
            // 
            // donutChart1
            // 
            this.donutChart1.BackColor = System.Drawing.SystemColors.Control;
            this.donutChart1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.donutChart1.Location = new System.Drawing.Point(226, 12);
            this.donutChart1.MinimumSize = new System.Drawing.Size(220, 220);
            this.donutChart1.Name = "donutChart1";
            this.donutChart1.Size = new System.Drawing.Size(220, 330);
            slice1.Text = "Cash on Hand";
            slice1.Value = 1800D;
            slice2.Text = "Cash in Bank";
            slice2.Value = 1200D;
            slice3.Text = "Equipment";
            slice3.Value = 500D;
            slice4.Text = "Office Supplies";
            slice4.Value = 750D;
            this.donutChart1.Slices.Add(slice1);
            this.donutChart1.Slices.Add(slice2);
            this.donutChart1.Slices.Add(slice3);
            this.donutChart1.Slices.Add(slice4);
            this.donutChart1.TabIndex = 0;
            this.donutChart1.Text = "ASSETS";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 469);
            this.Controls.Add(this.donutChart1);
            this.Name = "MainForm";
            this.Text = "Main Form";
            this.ResumeLayout(false);

        }

        #endregion

        private DonutChart donutChart1;
    }
}

