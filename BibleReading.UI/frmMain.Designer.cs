namespace BibleReading.UI
{
    partial class FrmMain
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
            this.lblVerse = new System.Windows.Forms.Label();
            this.lblReference = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStartPause = new System.Windows.Forms.Button();
            this.lblStatistics = new System.Windows.Forms.Label();
            this.lblPaused = new System.Windows.Forms.Label();
            this.lblTimeRemaining = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblVerse
            // 
            this.lblVerse.AutoSize = true;
            this.lblVerse.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblVerse.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVerse.Location = new System.Drawing.Point(165, 283);
            this.lblVerse.Name = "lblVerse";
            this.lblVerse.Size = new System.Drawing.Size(102, 29);
            this.lblVerse.TabIndex = 0;
            this.lblVerse.Text = "lblVerse";
            this.lblVerse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblReference
            // 
            this.lblReference.AutoSize = true;
            this.lblReference.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReference.ForeColor = System.Drawing.Color.Red;
            this.lblReference.Location = new System.Drawing.Point(167, 248);
            this.lblReference.Name = "lblReference";
            this.lblReference.Size = new System.Drawing.Size(151, 29);
            this.lblReference.TabIndex = 1;
            this.lblReference.Text = "lblReference";
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Enabled = false;
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(1345, 716);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(135, 57);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "&STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStartPause
            // 
            this.btnStartPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartPause.Location = new System.Drawing.Point(1195, 716);
            this.btnStartPause.Name = "btnStartPause";
            this.btnStartPause.Size = new System.Drawing.Size(135, 57);
            this.btnStartPause.TabIndex = 3;
            this.btnStartPause.Text = "&START";
            this.btnStartPause.UseVisualStyleBackColor = true;
            this.btnStartPause.Click += new System.EventHandler(this.btnStartPause_Click);
            // 
            // lblStatistics
            // 
            this.lblStatistics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatistics.AutoSize = true;
            this.lblStatistics.Location = new System.Drawing.Point(12, 568);
            this.lblStatistics.Name = "lblStatistics";
            this.lblStatistics.Size = new System.Drawing.Size(59, 13);
            this.lblStatistics.TabIndex = 4;
            this.lblStatistics.Text = "lblStatistics";
            // 
            // lblPaused
            // 
            this.lblPaused.BackColor = System.Drawing.Color.LightBlue;
            this.lblPaused.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPaused.Location = new System.Drawing.Point(244, 527);
            this.lblPaused.Name = "lblPaused";
            this.lblPaused.Size = new System.Drawing.Size(690, 34);
            this.lblPaused.TabIndex = 5;
            this.lblPaused.Text = "You are not reading at the moment. Click \"START\" to start reading.";
            this.lblPaused.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTimeRemaining
            // 
            this.lblTimeRemaining.AutoSize = true;
            this.lblTimeRemaining.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeRemaining.Location = new System.Drawing.Point(580, 62);
            this.lblTimeRemaining.Name = "lblTimeRemaining";
            this.lblTimeRemaining.Size = new System.Drawing.Size(211, 29);
            this.lblTimeRemaining.TabIndex = 6;
            this.lblTimeRemaining.Text = "lblTimeRemaining";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1492, 785);
            this.Controls.Add(this.lblTimeRemaining);
            this.Controls.Add(this.lblPaused);
            this.Controls.Add(this.lblStatistics);
            this.Controls.Add(this.btnStartPause);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lblReference);
            this.Controls.Add(this.lblVerse);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bible Reading";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.SizeChanged += new System.EventHandler(this.FrmMain_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FrmMain_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblVerse;
        private System.Windows.Forms.Label lblReference;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStartPause;
        private System.Windows.Forms.Label lblStatistics;
        private System.Windows.Forms.Label lblPaused;
        private System.Windows.Forms.Label lblTimeRemaining;

    }
}

