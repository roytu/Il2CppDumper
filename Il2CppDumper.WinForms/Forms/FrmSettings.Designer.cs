namespace Il2CppDumper.WinForms.Forms
{
    partial class FrmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSettings));
            chkAutoSetDir = new CheckBox();
            groupBox1 = new GroupBox();
            label1 = new Label();
            rad32 = new RadioButton();
            rad64 = new RadioButton();
            groupBox2 = new GroupBox();
            chkExtDat = new CheckBox();
            chkExtBin = new CheckBox();
            btnApply = new Button();
            groupBox3 = new GroupBox();
            clbScripts = new CheckedListBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // chkAutoSetDir
            // 
            chkAutoSetDir.AutoSize = true;
            chkAutoSetDir.Checked = true;
            chkAutoSetDir.CheckState = CheckState.Checked;
            chkAutoSetDir.Location = new Point(6, 22);
            chkAutoSetDir.Name = "chkAutoSetDir";
            chkAutoSetDir.Size = new Size(159, 19);
            chkAutoSetDir.TabIndex = 1;
            chkAutoSetDir.Text = "Auto set output directory";
            chkAutoSetDir.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(rad32);
            groupBox1.Controls.Add(rad64);
            groupBox1.Controls.Add(chkAutoSetDir);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(195, 71);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "General";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 48);
            label1.Name = "label1";
            label1.Size = new Size(54, 15);
            label1.TabIndex = 4;
            label1.Text = "Mach-O:";
            // 
            // rad32
            // 
            rad32.AutoSize = true;
            rad32.Location = new Point(66, 46);
            rad32.Name = "rad32";
            rad32.Size = new Size(56, 19);
            rad32.TabIndex = 3;
            rad32.TabStop = true;
            rad32.Text = "32-bit";
            rad32.UseVisualStyleBackColor = true;
            // 
            // rad64
            // 
            rad64.AutoSize = true;
            rad64.Checked = true;
            rad64.Location = new Point(133, 46);
            rad64.Name = "rad64";
            rad64.Size = new Size(56, 19);
            rad64.TabIndex = 2;
            rad64.TabStop = true;
            rad64.Text = "64-bit";
            rad64.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(chkExtDat);
            groupBox2.Controls.Add(chkExtBin);
            groupBox2.Location = new Point(12, 89);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(195, 76);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "File auto dumping";
            // 
            // chkExtDat
            // 
            chkExtDat.AutoSize = true;
            chkExtDat.Location = new Point(6, 47);
            chkExtDat.Name = "chkExtDat";
            chkExtDat.Size = new Size(174, 19);
            chkExtDat.TabIndex = 1;
            chkExtDat.Text = "Extract Global-metadata.dat";
            chkExtDat.UseVisualStyleBackColor = true;
            // 
            // chkExtBin
            // 
            chkExtBin.AutoSize = true;
            chkExtBin.Location = new Point(6, 22);
            chkExtBin.Name = "chkExtBin";
            chkExtBin.Size = new Size(117, 19);
            chkExtBin.TabIndex = 0;
            chkExtBin.Text = "Extract binary file";
            chkExtBin.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            btnApply.Location = new Point(12, 371);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(195, 23);
            btnApply.TabIndex = 4;
            btnApply.Text = "Apply";
            btnApply.UseVisualStyleBackColor = true;
            btnApply.Click += btnApply_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(clbScripts);
            groupBox3.Location = new Point(12, 171);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(195, 194);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Text = "Auto copy script files";
            // 
            // clbScripts
            // 
            clbScripts.FormattingEnabled = true;
            clbScripts.Items.AddRange(new object[] { "ghidra.py", "ghidra_wasm.py", "ghidra_with_struct.py", "il2cpp_header_to_ghidra.py", "ida.py", "ida_py3.py", "ida_with_struct.py", "ida_with_struct_py3.py", "hopper-py3.py", "il2cpp_header_to_binja.py" });
            clbScripts.Location = new Point(6, 22);
            clbScripts.Name = "clbScripts";
            clbScripts.Size = new Size(183, 166);
            clbScripts.TabIndex = 1;
            // 
            // FrmSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(219, 406);
            Controls.Add(groupBox3);
            Controls.Add(btnApply);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmSettings";
            StartPosition = FormStartPosition.Manual;
            Text = "Settings";
            TopMost = true;
            Load += FrmSettings_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkAutoSetDir;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkExtBin;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckedListBox clbScripts;
        private System.Windows.Forms.CheckBox chkExtDat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rad32;
        private System.Windows.Forms.RadioButton rad64;
    }
}