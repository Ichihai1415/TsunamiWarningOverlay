namespace TsunamiWarningOverlay
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Ti_ViewChange = new System.Windows.Forms.Timer(components);
            PB_Main = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)PB_Main).BeginInit();
            SuspendLayout();
            // 
            // Ti_ViewChange
            // 
            Ti_ViewChange.Interval = 2000;
            Ti_ViewChange.Tick += Ti_ViewChange_Tick;
            // 
            // PB_Main
            // 
            PB_Main.Dock = DockStyle.Fill;
            PB_Main.Location = new Point(0, 0);
            PB_Main.Name = "PB_Main";
            PB_Main.Size = new Size(600, 600);
            PB_Main.TabIndex = 0;
            PB_Main.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 600);
            Controls.Add(PB_Main);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Form1";
            Text = "TsunamiWarningOverlay";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)PB_Main).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer Ti_ViewChange;
        private PictureBox PB_Main;
    }
}
