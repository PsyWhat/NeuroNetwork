namespace EvolutionNetwork
{
    partial class Form1
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
            this.passGenerationBtn = new System.Windows.Forms.Button();
            this.generateNewBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.IndividualSelector = new System.Windows.Forms.NumericUpDown();
            this.cpyGenerationBtn = new System.Windows.Forms.Button();
            this.populationCount = new System.Windows.Forms.TextBox();
            this.smcTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.umcTB = new System.Windows.Forms.TextBox();
            this.structmcTB = new System.Windows.Forms.TextBox();
            this.complexitymcTB = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.balanceTask2 = new EvolutionNetwork.BalanceTask();
            this.status = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IndividualSelector)).BeginInit();
            this.SuspendLayout();
            // 
            // passGenerationBtn
            // 
            this.passGenerationBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.passGenerationBtn.Location = new System.Drawing.Point(0, 65);
            this.passGenerationBtn.Margin = new System.Windows.Forms.Padding(5);
            this.passGenerationBtn.Name = "passGenerationBtn";
            this.passGenerationBtn.Padding = new System.Windows.Forms.Padding(5);
            this.passGenerationBtn.Size = new System.Drawing.Size(86, 45);
            this.passGenerationBtn.TabIndex = 1;
            this.passGenerationBtn.Text = "Pass Generation";
            this.passGenerationBtn.UseVisualStyleBackColor = true;
            this.passGenerationBtn.Click += new System.EventHandler(this.passGenerationBtn_Click);
            // 
            // generateNewBtn
            // 
            this.generateNewBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.generateNewBtn.Location = new System.Drawing.Point(0, 0);
            this.generateNewBtn.Margin = new System.Windows.Forms.Padding(5);
            this.generateNewBtn.Name = "generateNewBtn";
            this.generateNewBtn.Padding = new System.Windows.Forms.Padding(5);
            this.generateNewBtn.Size = new System.Drawing.Size(86, 45);
            this.generateNewBtn.TabIndex = 2;
            this.generateNewBtn.Text = "Generate New Population";
            this.generateNewBtn.UseVisualStyleBackColor = true;
            this.generateNewBtn.Click += new System.EventHandler(this.generateNewBtn_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.status);
            this.panel1.Controls.Add(this.IndividualSelector);
            this.panel1.Controls.Add(this.cpyGenerationBtn);
            this.panel1.Controls.Add(this.passGenerationBtn);
            this.panel1.Controls.Add(this.populationCount);
            this.panel1.Controls.Add(this.generateNewBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1025, 0);
            this.panel1.MinimumSize = new System.Drawing.Size(0, 135);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(86, 376);
            this.panel1.TabIndex = 3;
            this.panel1.Visible = false;
            // 
            // IndividualSelector
            // 
            this.IndividualSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.IndividualSelector.Location = new System.Drawing.Point(0, 155);
            this.IndividualSelector.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.IndividualSelector.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.IndividualSelector.Name = "IndividualSelector";
            this.IndividualSelector.Size = new System.Drawing.Size(86, 20);
            this.IndividualSelector.TabIndex = 14;
            this.IndividualSelector.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cpyGenerationBtn
            // 
            this.cpyGenerationBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.cpyGenerationBtn.Location = new System.Drawing.Point(0, 110);
            this.cpyGenerationBtn.Name = "cpyGenerationBtn";
            this.cpyGenerationBtn.Size = new System.Drawing.Size(86, 45);
            this.cpyGenerationBtn.TabIndex = 3;
            this.cpyGenerationBtn.Text = "Show Selected";
            this.cpyGenerationBtn.UseVisualStyleBackColor = true;
            this.cpyGenerationBtn.Click += new System.EventHandler(this.cpyGenerationBtn_Click);
            // 
            // populationCount
            // 
            this.populationCount.Dock = System.Windows.Forms.DockStyle.Top;
            this.populationCount.Location = new System.Drawing.Point(0, 45);
            this.populationCount.Name = "populationCount";
            this.populationCount.Size = new System.Drawing.Size(86, 20);
            this.populationCount.TabIndex = 13;
            // 
            // smcTB
            // 
            this.smcTB.Location = new System.Drawing.Point(15, 25);
            this.smcTB.Name = "smcTB";
            this.smcTB.Size = new System.Drawing.Size(126, 20);
            this.smcTB.TabIndex = 4;
            this.smcTB.Text = "1.0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Score Mutation Coefficient";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Usual Mutation Coefficient";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(287, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Structural Mutation Coefficient";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(442, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Complexity Mutation Coefficient";
            // 
            // umcTB
            // 
            this.umcTB.Location = new System.Drawing.Point(153, 25);
            this.umcTB.Name = "umcTB";
            this.umcTB.Size = new System.Drawing.Size(128, 20);
            this.umcTB.TabIndex = 9;
            this.umcTB.Text = "1.0";
            // 
            // structmcTB
            // 
            this.structmcTB.Location = new System.Drawing.Point(290, 25);
            this.structmcTB.Name = "structmcTB";
            this.structmcTB.Size = new System.Drawing.Size(146, 20);
            this.structmcTB.TabIndex = 10;
            this.structmcTB.Text = "1.0";
            // 
            // complexitymcTB
            // 
            this.complexitymcTB.Location = new System.Drawing.Point(445, 25);
            this.complexitymcTB.Name = "complexitymcTB";
            this.complexitymcTB.Size = new System.Drawing.Size(151, 20);
            this.complexitymcTB.TabIndex = 11;
            this.complexitymcTB.Text = "1.0";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(602, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 33);
            this.button1.TabIndex = 12;
            this.button1.Text = "Build Teacher";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // balanceTask2
            // 
            this.balanceTask2.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.balanceTask2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.balanceTask2.Location = new System.Drawing.Point(0, 376);
            this.balanceTask2.Name = "balanceTask2";
            this.balanceTask2.Size = new System.Drawing.Size(1111, 300);
            this.balanceTask2.TabIndex = 13;
            this.balanceTask2.Text = "balanceTask2";
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Location = new System.Drawing.Point(3, 178);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(35, 13);
            this.status.TabIndex = 15;
            this.status.Text = "label5";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1111, 676);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.complexitymcTB);
            this.Controls.Add(this.structmcTB);
            this.Controls.Add(this.umcTB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.smcTB);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.balanceTask2);
            this.MinimumSize = new System.Drawing.Size(16, 480);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IndividualSelector)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BalanceTask balanceTask1;
        private System.Windows.Forms.Button passGenerationBtn;
        private System.Windows.Forms.Button generateNewBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cpyGenerationBtn;
        private System.Windows.Forms.TextBox smcTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox umcTB;
        private System.Windows.Forms.TextBox structmcTB;
        private System.Windows.Forms.TextBox complexitymcTB;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox populationCount;
        private BalanceTask balanceTask2;
        private System.Windows.Forms.NumericUpDown IndividualSelector;
        private System.Windows.Forms.Label status;
    }
}

