namespace PuzzleWin
{
    partial class PuzzleForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PuzzleForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuPause = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowPic = new System.Windows.Forms.ToolStripMenuItem();
            this.menuComplete = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(355, 157);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(355, 216);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 30);
            this.label2.TabIndex = 1;
            this.label2.Text = "label2";
            this.label2.Visible = false;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuPause,
            this.menuShowPic,
            this.menuComplete,
            this.menuSave});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(181, 114);
            // 
            // menuPause
            // 
            this.menuPause.Name = "menuPause";
            this.menuPause.Size = new System.Drawing.Size(180, 22);
            this.menuPause.Text = "Pause";
            this.menuPause.Click += new System.EventHandler(this.Pause_OnClick);
            // 
            // menuShowPic
            // 
            this.menuShowPic.Name = "menuShowPic";
            this.menuShowPic.Size = new System.Drawing.Size(180, 22);
            this.menuShowPic.Text = "Show Picture";
            this.menuShowPic.Click += new System.EventHandler(this.ShowPic_OnClick);
            // 
            // menuComplete
            // 
            this.menuComplete.Name = "menuComplete";
            this.menuComplete.Size = new System.Drawing.Size(180, 22);
            this.menuComplete.Text = "complete";
            this.menuComplete.Click += new System.EventHandler(this.menuComplete_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Tag = "";
            this.notifyIcon.Text = "Resume Puzzle";
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // menuSave
            // 
            this.menuSave.Name = "menuSave";
            this.menuSave.Size = new System.Drawing.Size(180, 22);
            this.menuSave.Text = "Save";
            // 
            // PuzzleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ContextMenuStrip = this.contextMenuStrip;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PuzzleForm";
            this.Text = "Puzzle";
            this.Load += new System.EventHandler(this.PuzzleForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PuzzleForm_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PuzzleForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PuzzleForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PuzzleForm_MouseUp);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Label label2;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem menuPause;
        private NotifyIcon notifyIcon;
        private ToolStripMenuItem menuShowPic;
        private ToolStripMenuItem menuComplete;
        private ToolStripMenuItem menuSave;
    }
}