
namespace PolyphasicScheduleFinder
{
    partial class FinderWindow
    {
        /// <summary> Required designer variable. </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> Clean up any resources being used. </summary>
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

        /// <summary>  Required method for Designer support - do not modify the contents of this method with the code editor. </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FinderWindow));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Unsure = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.start1 = new System.Windows.Forms.MaskedTextBox();
            this.End1 = new System.Windows.Forms.MaskedTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.End2 = new System.Windows.Forms.MaskedTextBox();
            this.start2 = new System.Windows.Forms.MaskedTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.End3 = new System.Windows.Forms.MaskedTextBox();
            this.start3 = new System.Windows.Forms.MaskedTextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.End4 = new System.Windows.Forms.MaskedTextBox();
            this.start4 = new System.Windows.Forms.MaskedTextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.End5 = new System.Windows.Forms.MaskedTextBox();
            this.start5 = new System.Windows.Forms.MaskedTextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.End6 = new System.Windows.Forms.MaskedTextBox();
            this.start6 = new System.Windows.Forms.MaskedTextBox();
            this.NoRestrictions = new System.Windows.Forms.CheckBox();
            this.Mono = new System.Windows.Forms.NumericUpDown();
            this.Age = new System.Windows.Forms.NumericUpDown();
            this.Done = new System.Windows.Forms.Button();
            this.Results = new System.Windows.Forms.DataGridView();
            this.ScheduleName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Activity = new System.Windows.Forms.ComboBox();
            this.Experience = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Mono)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Age)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Results)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(162, 94);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Age";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(118, 45);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Activity Level";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 118);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(162, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Monophasic sleep length (Hours)";
            // 
            // Unsure
            // 
            this.Unsure.AutoSize = true;
            this.Unsure.Location = new System.Drawing.Point(242, 117);
            this.Unsure.Margin = new System.Windows.Forms.Padding(2);
            this.Unsure.Name = "Unsure";
            this.Unsure.Size = new System.Drawing.Size(66, 17);
            this.Unsure.TabIndex = 6;
            this.Unsure.Text = "Unsure?";
            this.Unsure.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(9, 15);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(334, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Fill in each of the fields below in order to get your results.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 157);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.MaximumSize = new System.Drawing.Size(333, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(330, 26);
            this.label5.TabIndex = 8;
            this.label5.Text = "Fill in the boxes below with the times you\'d be able to sleep between on a daily " +
    "basis. (Formatting: 00:00/23:59)";
            // 
            // start1
            // 
            this.start1.Location = new System.Drawing.Point(107, 210);
            this.start1.Margin = new System.Windows.Forms.Padding(2);
            this.start1.Name = "start1";
            this.start1.Size = new System.Drawing.Size(58, 20);
            this.start1.TabIndex = 9;
            // 
            // End1
            // 
            this.End1.Location = new System.Drawing.Point(197, 210);
            this.End1.Margin = new System.Windows.Forms.Padding(2);
            this.End1.Name = "End1";
            this.End1.Size = new System.Drawing.Size(58, 20);
            this.End1.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(72, 213);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Start";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(167, 213);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "End";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(167, 233);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "End";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(72, 233);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Start";
            // 
            // End2
            // 
            this.End2.Location = new System.Drawing.Point(197, 231);
            this.End2.Margin = new System.Windows.Forms.Padding(2);
            this.End2.Name = "End2";
            this.End2.Size = new System.Drawing.Size(58, 20);
            this.End2.TabIndex = 14;
            // 
            // start2
            // 
            this.start2.Location = new System.Drawing.Point(107, 231);
            this.start2.Margin = new System.Windows.Forms.Padding(2);
            this.start2.Name = "start2";
            this.start2.Size = new System.Drawing.Size(58, 20);
            this.start2.TabIndex = 13;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(167, 254);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(26, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "End";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(72, 254);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "Start";
            // 
            // End3
            // 
            this.End3.Location = new System.Drawing.Point(197, 252);
            this.End3.Margin = new System.Windows.Forms.Padding(2);
            this.End3.Name = "End3";
            this.End3.Size = new System.Drawing.Size(58, 20);
            this.End3.TabIndex = 18;
            // 
            // start3
            // 
            this.start3.Location = new System.Drawing.Point(107, 252);
            this.start3.Margin = new System.Windows.Forms.Padding(2);
            this.start3.Name = "start3";
            this.start3.Size = new System.Drawing.Size(58, 20);
            this.start3.TabIndex = 17;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(167, 274);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(26, 13);
            this.label12.TabIndex = 24;
            this.label12.Text = "End";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(72, 274);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 13);
            this.label13.TabIndex = 23;
            this.label13.Text = "Start";
            // 
            // End4
            // 
            this.End4.Location = new System.Drawing.Point(197, 273);
            this.End4.Margin = new System.Windows.Forms.Padding(2);
            this.End4.Name = "End4";
            this.End4.Size = new System.Drawing.Size(58, 20);
            this.End4.TabIndex = 22;
            // 
            // start4
            // 
            this.start4.Location = new System.Drawing.Point(107, 273);
            this.start4.Margin = new System.Windows.Forms.Padding(2);
            this.start4.Name = "start4";
            this.start4.Size = new System.Drawing.Size(58, 20);
            this.start4.TabIndex = 21;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(167, 295);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(26, 13);
            this.label14.TabIndex = 28;
            this.label14.Text = "End";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(72, 295);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(29, 13);
            this.label15.TabIndex = 27;
            this.label15.Text = "Start";
            // 
            // End5
            // 
            this.End5.Location = new System.Drawing.Point(197, 294);
            this.End5.Margin = new System.Windows.Forms.Padding(2);
            this.End5.Name = "End5";
            this.End5.Size = new System.Drawing.Size(58, 20);
            this.End5.TabIndex = 26;
            // 
            // start5
            // 
            this.start5.Location = new System.Drawing.Point(107, 294);
            this.start5.Margin = new System.Windows.Forms.Padding(2);
            this.start5.Name = "start5";
            this.start5.Size = new System.Drawing.Size(58, 20);
            this.start5.TabIndex = 25;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(167, 317);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(26, 13);
            this.label16.TabIndex = 32;
            this.label16.Text = "End";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(72, 317);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(29, 13);
            this.label17.TabIndex = 31;
            this.label17.Text = "Start";
            // 
            // End6
            // 
            this.End6.Location = new System.Drawing.Point(197, 314);
            this.End6.Margin = new System.Windows.Forms.Padding(2);
            this.End6.Name = "End6";
            this.End6.Size = new System.Drawing.Size(58, 20);
            this.End6.TabIndex = 30;
            // 
            // start6
            // 
            this.start6.Location = new System.Drawing.Point(107, 314);
            this.start6.Margin = new System.Windows.Forms.Padding(2);
            this.start6.Name = "start6";
            this.start6.Size = new System.Drawing.Size(58, 20);
            this.start6.TabIndex = 29;
            // 
            // NoRestrictions
            // 
            this.NoRestrictions.AutoSize = true;
            this.NoRestrictions.Location = new System.Drawing.Point(131, 191);
            this.NoRestrictions.Margin = new System.Windows.Forms.Padding(2);
            this.NoRestrictions.Name = "NoRestrictions";
            this.NoRestrictions.Size = new System.Drawing.Size(98, 17);
            this.NoRestrictions.TabIndex = 33;
            this.NoRestrictions.Text = "No Restrictions";
            this.NoRestrictions.UseVisualStyleBackColor = true;
            // 
            // Mono
            // 
            this.Mono.DecimalPlaces = 2;
            this.Mono.Location = new System.Drawing.Point(192, 116);
            this.Mono.Margin = new System.Windows.Forms.Padding(2);
            this.Mono.Name = "Mono";
            this.Mono.Size = new System.Drawing.Size(45, 20);
            this.Mono.TabIndex = 34;
            // 
            // Age
            // 
            this.Age.Location = new System.Drawing.Point(192, 92);
            this.Age.Margin = new System.Windows.Forms.Padding(2);
            this.Age.Name = "Age";
            this.Age.Size = new System.Drawing.Size(45, 20);
            this.Age.TabIndex = 36;
            // 
            // Done
            // 
            this.Done.Location = new System.Drawing.Point(123, 342);
            this.Done.Margin = new System.Windows.Forms.Padding(2);
            this.Done.Name = "Done";
            this.Done.Size = new System.Drawing.Size(105, 32);
            this.Done.TabIndex = 37;
            this.Done.Text = "Generate Results!";
            this.Done.UseVisualStyleBackColor = true;
            this.Done.Click += new System.EventHandler(this.Done_Click);
            // 
            // Results
            // 
            this.Results.AllowUserToAddRows = false;
            this.Results.AllowUserToDeleteRows = false;
            this.Results.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.Results.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Results.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ScheduleName,
            this.Column2,
            this.Column3});
            this.Results.Location = new System.Drawing.Point(344, 15);
            this.Results.Margin = new System.Windows.Forms.Padding(2);
            this.Results.Name = "Results";
            this.Results.ReadOnly = true;
            this.Results.RowHeadersWidth = 51;
            this.Results.RowTemplate.Height = 28;
            this.Results.Size = new System.Drawing.Size(331, 331);
            this.Results.TabIndex = 38;
            // 
            // ScheduleName
            // 
            this.ScheduleName.HeaderText = "Shedule Name";
            this.ScheduleName.MinimumWidth = 6;
            this.ScheduleName.Name = "ScheduleName";
            this.ScheduleName.ReadOnly = true;
            this.ScheduleName.Width = 94;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "# Of Sleeps";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 81;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Total Sleep Time";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 103;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(7, 141);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(334, 8);
            this.panel1.TabIndex = 39;
            // 
            // Activity
            // 
            this.Activity.FormattingEnabled = true;
            this.Activity.Items.AddRange(new object[] {
            "None",
            "Mid",
            "High"});
            this.Activity.Location = new System.Drawing.Point(192, 42);
            this.Activity.Margin = new System.Windows.Forms.Padding(2);
            this.Activity.Name = "Activity";
            this.Activity.Size = new System.Drawing.Size(45, 21);
            this.Activity.TabIndex = 40;
            // 
            // experience
            // 
            this.Experience.FormattingEnabled = true;
            this.Experience.Items.AddRange(new object[] {
            "None",
            "Mid",
            "High"});
            this.Experience.Location = new System.Drawing.Point(192, 67);
            this.Experience.Margin = new System.Windows.Forms.Padding(2);
            this.Experience.Name = "experience";
            this.Experience.Size = new System.Drawing.Size(45, 21);
            this.Experience.TabIndex = 42;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(74, 70);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(114, 13);
            this.label18.TabIndex = 41;
            this.label18.Text = "Polyphasic Experience";
            // 
            // FinderWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 379);
            this.Controls.Add(this.Experience);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.Activity);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Results);
            this.Controls.Add(this.Done);
            this.Controls.Add(this.Age);
            this.Controls.Add(this.Mono);
            this.Controls.Add(this.NoRestrictions);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.End6);
            this.Controls.Add(this.start6);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.End5);
            this.Controls.Add(this.start5);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.End4);
            this.Controls.Add(this.start4);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.End3);
            this.Controls.Add(this.start3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.End2);
            this.Controls.Add(this.start2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.End1);
            this.Controls.Add(this.start1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Unsure);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FinderWindow";
            this.Text = "Polyphasic ScheduleFinder";
            ((System.ComponentModel.ISupportInitialize)(this.Mono)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Age)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Results)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox Unsure;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.MaskedTextBox start1;
        private System.Windows.Forms.MaskedTextBox End1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.MaskedTextBox End2;
        private System.Windows.Forms.MaskedTextBox start2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.MaskedTextBox End3;
        private System.Windows.Forms.MaskedTextBox start3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.MaskedTextBox End4;
        private System.Windows.Forms.MaskedTextBox start4;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.MaskedTextBox End5;
        private System.Windows.Forms.MaskedTextBox start5;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.MaskedTextBox End6;
        private System.Windows.Forms.MaskedTextBox start6;
        private System.Windows.Forms.CheckBox NoRestrictions;
        private System.Windows.Forms.NumericUpDown Mono;
        private System.Windows.Forms.NumericUpDown Age;
        private System.Windows.Forms.Button Done;
        private System.Windows.Forms.DataGridView Results;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScheduleName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox Activity;
        private System.Windows.Forms.ComboBox Experience;
        private System.Windows.Forms.Label label18;
    }
}

