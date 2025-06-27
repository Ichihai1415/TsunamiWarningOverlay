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
            CMS = new ContextMenuStrip(components);
            TSMI_reboot = new ToolStripMenuItem();
            TSMI_exit = new ToolStripMenuItem();
            La_Times = new Label();
            Ti_GetP2PQ = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)PB_Main).BeginInit();
            CMS.SuspendLayout();
            SuspendLayout();
            // 
            // Ti_ViewChange
            // 
            Ti_ViewChange.Interval = 2000;
            Ti_ViewChange.Tick += Ti_ViewChange_Tick;
            // 
            // PB_Main
            // 
            PB_Main.BackgroundImageLayout = ImageLayout.None;
            PB_Main.ContextMenuStrip = CMS;
            PB_Main.Dock = DockStyle.Fill;
            PB_Main.Location = new Point(0, 0);
            PB_Main.Name = "PB_Main";
            PB_Main.Size = new Size(600, 600);
            PB_Main.SizeMode = PictureBoxSizeMode.Zoom;
            PB_Main.TabIndex = 0;
            PB_Main.TabStop = false;
            PB_Main.MouseDown += PB_Main_MouseDown;
            PB_Main.MouseMove += PB_Main_MouseMove;
            // 
            // CMS
            // 
            CMS.Items.AddRange(new ToolStripItem[] { TSMI_reboot, TSMI_exit });
            CMS.Name = "contextMenuStrip1";
            CMS.Size = new Size(111, 48);
            // 
            // TSMI_reboot
            // 
            TSMI_reboot.Name = "TSMI_reboot";
            TSMI_reboot.Size = new Size(110, 22);
            TSMI_reboot.Text = "再起動";
            TSMI_reboot.Click += TSMI_reboot_Click;
            // 
            // TSMI_exit
            // 
            TSMI_exit.Name = "TSMI_exit";
            TSMI_exit.Size = new Size(110, 22);
            TSMI_exit.Text = "終了";
            TSMI_exit.Click += TSMI_exit_Click;
            // 
            // La_Times
            // 
            La_Times.AutoSize = true;
            La_Times.ContextMenuStrip = CMS;
            La_Times.Font = new Font("Yu Gothic UI", 10F);
            La_Times.Location = new Point(0, 0);
            La_Times.Name = "La_Times";
            La_Times.Size = new Size(0, 19);
            La_Times.TabIndex = 1;
            La_Times.MouseDown += La_Times_MouseDown;
            La_Times.MouseMove += La_Times_MouseMove;
            // 
            // Ti_GetP2PQ
            // 
            Ti_GetP2PQ.Enabled = true;
            Ti_GetP2PQ.Interval = 60000;
            Ti_GetP2PQ.Tick += Ti_GetP2PQ_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(600, 600);
            ContextMenuStrip = CMS;
            Controls.Add(La_Times);
            Controls.Add(PB_Main);
            ForeColor = SystemColors.Control;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            Text = "TsunamiWarningOverlay";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)PB_Main).EndInit();
            CMS.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer Ti_ViewChange;
        private PictureBox PB_Main;
        private Label La_Times;
        private System.Windows.Forms.Timer Ti_GetP2PQ;
        private ContextMenuStrip CMS;
        private ToolStripMenuItem TSMI_reboot;
        private ToolStripMenuItem TSMI_exit;
    }
}
