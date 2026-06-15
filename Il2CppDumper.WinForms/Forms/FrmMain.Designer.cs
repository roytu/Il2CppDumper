namespace Il2CppDumper.WinForms.Forms
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label9 = new Label();
            txtBinaryPath = new TextBox();
            txtMetadataPath = new TextBox();
            txtOutputDir = new TextBox();
            txtCodeRegis = new TextBox();
            txtMetaRegis = new TextBox();
            btnSelectBinary = new Button();
            btnSelectMetadata = new Button();
            btnSelectDir = new Button();
            btnOpenDir = new Button();
            btnSettings = new Button();
            btnDump = new Button();
            groupBox1 = new GroupBox();
            rbLog = new RichTextBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            menuCopy = new ToolStripMenuItem();
            openBin = new System.Windows.Forms.OpenFileDialog();
            openDat = new System.Windows.Forms.OpenFileDialog();
            openDir = new FolderBrowserDialog();
            cbArch = new ComboBox();
            groupBox1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(31, 15);
            label1.Name = "label1";
            label1.Size = new Size(99, 15);
            label1.TabIndex = 0;
            label1.Text = "Il2Cpp binary file:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 44);
            label2.Name = "label2";
            label2.Size = new Size(118, 15);
            label2.TabIndex = 1;
            label2.Text = "global-metadata.dat:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(32, 73);
            label3.Name = "label3";
            label3.Size = new Size(98, 15);
            label3.TabIndex = 2;
            label3.Text = "Output directory:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(26, 102);
            label4.Name = "label4";
            label4.Size = new Size(104, 15);
            label4.TabIndex = 3;
            label4.Text = "Code Registration:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(239, 102);
            label5.Name = "label5";
            label5.Size = new Size(126, 15);
            label5.TabIndex = 4;
            label5.Text = "Metadata Registration:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(49, 131);
            label6.Name = "label6";
            label6.Size = new Size(81, 15);
            label6.TabIndex = 5;
            label6.Text = "Android Arch:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(123, 205);
            label9.Name = "label9";
            label9.Size = new Size(259, 15);
            label9.TabIndex = 8;
            label9.Text = "Drop APK or decrypted IPA file to start dumping";
            // 
            // txtBinaryPath
            // 
            txtBinaryPath.Location = new Point(136, 12);
            txtBinaryPath.Name = "txtBinaryPath";
            txtBinaryPath.Size = new Size(281, 23);
            txtBinaryPath.TabIndex = 9;
            // 
            // txtMetadataPath
            // 
            txtMetadataPath.Location = new Point(136, 41);
            txtMetadataPath.Name = "txtMetadataPath";
            txtMetadataPath.Size = new Size(281, 23);
            txtMetadataPath.TabIndex = 10;
            // 
            // txtOutputDir
            // 
            txtOutputDir.Location = new Point(136, 70);
            txtOutputDir.Name = "txtOutputDir";
            txtOutputDir.Size = new Size(229, 23);
            txtOutputDir.TabIndex = 11;
            // 
            // txtCodeRegis
            // 
            txtCodeRegis.Location = new Point(136, 99);
            txtCodeRegis.Name = "txtCodeRegis";
            txtCodeRegis.Size = new Size(100, 23);
            txtCodeRegis.TabIndex = 12;
            // 
            // txtMetaRegis
            // 
            txtMetaRegis.Location = new Point(371, 99);
            txtMetaRegis.Name = "txtMetaRegis";
            txtMetaRegis.Size = new Size(126, 23);
            txtMetaRegis.TabIndex = 13;
            // 
            // btnSelectBinary
            // 
            btnSelectBinary.Location = new Point(423, 11);
            btnSelectBinary.Name = "btnSelectBinary";
            btnSelectBinary.Size = new Size(74, 23);
            btnSelectBinary.TabIndex = 14;
            btnSelectBinary.Text = "Select";
            btnSelectBinary.UseVisualStyleBackColor = true;
            btnSelectBinary.Click += btnSelectBinary_Click;
            // 
            // btnSelectMetadata
            // 
            btnSelectMetadata.Location = new Point(423, 41);
            btnSelectMetadata.Name = "btnSelectMetadata";
            btnSelectMetadata.Size = new Size(74, 23);
            btnSelectMetadata.TabIndex = 14;
            btnSelectMetadata.Text = "Select";
            btnSelectMetadata.UseVisualStyleBackColor = true;
            btnSelectMetadata.Click += btnSelectMetadata_Click;
            // 
            // btnSelectDir
            // 
            btnSelectDir.Location = new Point(423, 71);
            btnSelectDir.Name = "btnSelectDir";
            btnSelectDir.Size = new Size(74, 23);
            btnSelectDir.TabIndex = 14;
            btnSelectDir.Text = "Select";
            btnSelectDir.UseVisualStyleBackColor = true;
            btnSelectDir.Click += btnSelectDir_Click;
            // 
            // btnOpenDir
            // 
            btnOpenDir.Location = new Point(371, 70);
            btnOpenDir.Name = "btnOpenDir";
            btnOpenDir.Size = new Size(46, 23);
            btnOpenDir.TabIndex = 14;
            btnOpenDir.Text = "Open";
            btnOpenDir.UseVisualStyleBackColor = true;
            btnOpenDir.Click += btnOpenDir_Click;
            // 
            // btnSettings
            // 
            btnSettings.Location = new Point(423, 128);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(75, 23);
            btnSettings.TabIndex = 17;
            btnSettings.Text = "Settings";
            btnSettings.UseVisualStyleBackColor = true;
            btnSettings.Click += btnSettings_Click;
            // 
            // btnDump
            // 
            btnDump.Location = new Point(12, 157);
            btnDump.Name = "btnDump";
            btnDump.Size = new Size(485, 45);
            btnDump.TabIndex = 18;
            btnDump.Text = "Dump";
            btnDump.UseVisualStyleBackColor = true;
            btnDump.Click += btnDump_ClickAsync;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rbLog);
            groupBox1.Location = new Point(12, 223);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(485, 226);
            groupBox1.TabIndex = 19;
            groupBox1.TabStop = false;
            groupBox1.Text = "Log Output";
            // 
            // rbLog
            // 
            rbLog.Dock = DockStyle.Fill;
            rbLog.Location = new Point(3, 19);
            rbLog.Name = "rbLog";
            rbLog.Size = new Size(479, 204);
            rbLog.TabIndex = 0;
            rbLog.Text = "";
            rbLog.TextChanged += rbLog_TextChanged;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { menuCopy });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(103, 26);
            // 
            // menuCopy
            // 
            menuCopy.Name = "menuCopy";
            menuCopy.Size = new Size(102, 22);
            menuCopy.Text = "Copy";
            menuCopy.Click += menuCopy_Click;
            // 
            // openBin
            // 
            openBin.Filter = "Il2Cpp binary file|*.*";
            openBin.Title = "Select Il2Cpp binary file";
            // 
            // openDat
            // 
            openDat.Filter = "global-metadata|global-metadata.dat";
            openDat.Title = "Select global-metadata.dat";
            // 
            // openDir
            // 
            openDir.Description = "Select output directory";
            // 
            // cbArch
            // 
            cbArch.DropDownStyle = ComboBoxStyle.DropDownList;
            cbArch.FormattingEnabled = true;
            cbArch.Items.AddRange(new object[] { "All", "ARMv7", "ARM64" });
            cbArch.Location = new Point(136, 128);
            cbArch.Name = "cbArch";
            cbArch.Size = new Size(126, 23);
            cbArch.TabIndex = 20;
            // 
            // FrmMain
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(509, 461);
            Controls.Add(cbArch);
            Controls.Add(groupBox1);
            Controls.Add(btnDump);
            Controls.Add(btnSettings);
            Controls.Add(btnOpenDir);
            Controls.Add(btnSelectDir);
            Controls.Add(btnSelectMetadata);
            Controls.Add(btnSelectBinary);
            Controls.Add(txtMetaRegis);
            Controls.Add(txtCodeRegis);
            Controls.Add(txtOutputDir);
            Controls.Add(txtMetadataPath);
            Controls.Add(txtBinaryPath);
            Controls.Add(label9);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "FrmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Il2Cpp Dumper";
            FormClosing += FrmMain_FormClosing;
            Load += FrmMain_Load;
            DragDrop += FrmMain_DragDropAsync;
            DragEnter += FrmMain_DragEnter;
            groupBox1.ResumeLayout(false);
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtBinaryPath;
        private System.Windows.Forms.TextBox txtMetadataPath;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.TextBox txtCodeRegis;
        private System.Windows.Forms.TextBox txtMetaRegis;
        private System.Windows.Forms.Button btnSelectBinary;
        private System.Windows.Forms.Button btnSelectMetadata;
        private System.Windows.Forms.Button btnSelectDir;
        private System.Windows.Forms.Button btnOpenDir;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnDump;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox rbLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuCopy;
        private System.Windows.Forms.OpenFileDialog openBin;
        private System.Windows.Forms.OpenFileDialog openDat;
        private System.Windows.Forms.FolderBrowserDialog openDir;
        private System.Windows.Forms.ComboBox cbArch;
    }
}

