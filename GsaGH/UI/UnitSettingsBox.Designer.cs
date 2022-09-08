
namespace GsaGH.UI
{
    partial class UnitSettingsBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnitSettingsBox));
      this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.comboBoxEnergy = new System.Windows.Forms.ComboBox();
      this.labelEnergy = new System.Windows.Forms.Label();
      this.labelTolerance = new System.Windows.Forms.Label();
      this.comboBoxTemperature = new System.Windows.Forms.ComboBox();
      this.labelTemperature = new System.Windows.Forms.Label();
      this.labelDescription = new System.Windows.Forms.Label();
      this.labelLengthResult = new System.Windows.Forms.Label();
      this.labelLengthSection = new System.Windows.Forms.Label();
      this.labelLengthGeometry = new System.Windows.Forms.Label();
      this.labelMoment = new System.Windows.Forms.Label();
      this.labelForce = new System.Windows.Forms.Label();
      this.labelMass = new System.Windows.Forms.Label();
      this.labelStress = new System.Windows.Forms.Label();
      this.labelStrain = new System.Windows.Forms.Label();
      this.comboBoxStrain = new System.Windows.Forms.ComboBox();
      this.comboBoxStress = new System.Windows.Forms.ComboBox();
      this.comboBoxMoment = new System.Windows.Forms.ComboBox();
      this.comboBoxForce = new System.Windows.Forms.ComboBox();
      this.comboBoxMass = new System.Windows.Forms.ComboBox();
      this.comboBoxLengthResult = new System.Windows.Forms.ComboBox();
      this.comboBoxLengthSection = new System.Windows.Forms.ComboBox();
      this.comboBoxLengthGeometry = new System.Windows.Forms.ComboBox();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.cancelButton = new System.Windows.Forms.Button();
      this.okButton = new System.Windows.Forms.Button();
      this.useRhinoToleranceCheckBox = new System.Windows.Forms.CheckBox();
      this.toleranceValueBox = new System.Windows.Forms.NumericUpDown();
      this.buttonSIUnits = new System.Windows.Forms.Button();
      this.buttonKNmUnits = new System.Windows.Forms.Button();
      this.buttonKipFtUnits = new System.Windows.Forms.Button();
      this.buttonKipInUnits = new System.Windows.Forms.Button();
      this.tableLayoutPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.toleranceValueBox)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel
      // 
      this.tableLayoutPanel.ColumnCount = 10;
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 129F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 173F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 11F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 51F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 43F));
      this.tableLayoutPanel.Controls.Add(this.comboBoxEnergy, 1, 4);
      this.tableLayoutPanel.Controls.Add(this.labelEnergy, 0, 4);
      this.tableLayoutPanel.Controls.Add(this.labelTolerance, 0, 5);
      this.tableLayoutPanel.Controls.Add(this.comboBoxTemperature, 6, 3);
      this.tableLayoutPanel.Controls.Add(this.labelTemperature, 4, 3);
      this.tableLayoutPanel.Controls.Add(this.labelDescription, 3, 6);
      this.tableLayoutPanel.Controls.Add(this.labelLengthResult, 4, 2);
      this.tableLayoutPanel.Controls.Add(this.labelLengthSection, 4, 1);
      this.tableLayoutPanel.Controls.Add(this.labelLengthGeometry, 4, 0);
      this.tableLayoutPanel.Controls.Add(this.labelMoment, 0, 1);
      this.tableLayoutPanel.Controls.Add(this.labelForce, 0, 0);
      this.tableLayoutPanel.Controls.Add(this.labelMass, 4, 4);
      this.tableLayoutPanel.Controls.Add(this.labelStress, 0, 2);
      this.tableLayoutPanel.Controls.Add(this.labelStrain, 0, 3);
      this.tableLayoutPanel.Controls.Add(this.comboBoxStrain, 1, 3);
      this.tableLayoutPanel.Controls.Add(this.comboBoxStress, 1, 2);
      this.tableLayoutPanel.Controls.Add(this.comboBoxMoment, 1, 1);
      this.tableLayoutPanel.Controls.Add(this.comboBoxForce, 1, 0);
      this.tableLayoutPanel.Controls.Add(this.comboBoxMass, 6, 4);
      this.tableLayoutPanel.Controls.Add(this.comboBoxLengthResult, 6, 2);
      this.tableLayoutPanel.Controls.Add(this.comboBoxLengthSection, 6, 1);
      this.tableLayoutPanel.Controls.Add(this.comboBoxLengthGeometry, 6, 0);
      this.tableLayoutPanel.Controls.Add(this.useRhinoToleranceCheckBox, 2, 5);
      this.tableLayoutPanel.Controls.Add(this.toleranceValueBox, 1, 5);
      this.tableLayoutPanel.Controls.Add(this.buttonSIUnits, 5, 5);
      this.tableLayoutPanel.Controls.Add(this.buttonKNmUnits, 6, 5);
      this.tableLayoutPanel.Controls.Add(this.buttonKipFtUnits, 7, 5);
      this.tableLayoutPanel.Controls.Add(this.buttonKipInUnits, 8, 5);
      this.tableLayoutPanel.Controls.Add(this.pictureBox1, 9, 6);
      this.tableLayoutPanel.Controls.Add(this.cancelButton, 1, 6);
      this.tableLayoutPanel.Controls.Add(this.okButton, 0, 6);
      this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel.Location = new System.Drawing.Point(12, 11);
      this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 7;
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel.Size = new System.Drawing.Size(818, 263);
      this.tableLayoutPanel.TabIndex = 0;
      // 
      // comboBoxEnergy
      // 
      this.tableLayoutPanel.SetColumnSpan(this.comboBoxEnergy, 2);
      this.comboBoxEnergy.Dock = System.Windows.Forms.DockStyle.Fill;
      this.comboBoxEnergy.FormattingEnabled = true;
      this.comboBoxEnergy.Location = new System.Drawing.Point(133, 152);
      this.comboBoxEnergy.Margin = new System.Windows.Forms.Padding(4);
      this.comboBoxEnergy.Name = "comboBoxEnergy";
      this.comboBoxEnergy.Size = new System.Drawing.Size(252, 24);
      this.comboBoxEnergy.TabIndex = 53;
      this.comboBoxEnergy.SelectedIndexChanged += new System.EventHandler(this.comboBoxEnergy_SelectedIndexChanged);
      // 
      // labelEnergy
      // 
      this.labelEnergy.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelEnergy.Location = new System.Drawing.Point(3, 155);
      this.labelEnergy.Margin = new System.Windows.Forms.Padding(3, 7, 13, 0);
      this.labelEnergy.MaximumSize = new System.Drawing.Size(0, 21);
      this.labelEnergy.Name = "labelEnergy";
      this.labelEnergy.Size = new System.Drawing.Size(113, 21);
      this.labelEnergy.TabIndex = 52;
      this.labelEnergy.Text = "Energy";
      // 
      // labelTolerance
      // 
      this.labelTolerance.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelTolerance.Location = new System.Drawing.Point(3, 192);
      this.labelTolerance.Margin = new System.Windows.Forms.Padding(3, 7, 13, 0);
      this.labelTolerance.MaximumSize = new System.Drawing.Size(0, 21);
      this.labelTolerance.Name = "labelTolerance";
      this.labelTolerance.Size = new System.Drawing.Size(113, 21);
      this.labelTolerance.TabIndex = 48;
      this.labelTolerance.Text = "Tolerance";
      // 
      // comboBoxTemperature
      // 
      this.tableLayoutPanel.SetColumnSpan(this.comboBoxTemperature, 4);
      this.comboBoxTemperature.Dock = System.Windows.Forms.DockStyle.Fill;
      this.comboBoxTemperature.FormattingEnabled = true;
      this.comboBoxTemperature.Location = new System.Drawing.Point(559, 115);
      this.comboBoxTemperature.Margin = new System.Windows.Forms.Padding(4);
      this.comboBoxTemperature.Name = "comboBoxTemperature";
      this.comboBoxTemperature.Size = new System.Drawing.Size(255, 24);
      this.comboBoxTemperature.TabIndex = 47;
      // 
      // labelTemperature
      // 
      this.tableLayoutPanel.SetColumnSpan(this.labelTemperature, 2);
      this.labelTemperature.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelTemperature.Location = new System.Drawing.Point(403, 118);
      this.labelTemperature.Margin = new System.Windows.Forms.Padding(3, 7, 13, 0);
      this.labelTemperature.MaximumSize = new System.Drawing.Size(0, 21);
      this.labelTemperature.Name = "labelTemperature";
      this.labelTemperature.Size = new System.Drawing.Size(139, 21);
      this.labelTemperature.TabIndex = 45;
      this.labelTemperature.Text = "Temperature";
      // 
      // labelDescription
      // 
      this.labelDescription.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.tableLayoutPanel.SetColumnSpan(this.labelDescription, 6);
      this.labelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelDescription.Location = new System.Drawing.Point(398, 231);
      this.labelDescription.Margin = new System.Windows.Forms.Padding(8, 2, 5, 5);
      this.labelDescription.MaximumSize = new System.Drawing.Size(400, 345);
      this.labelDescription.Name = "labelDescription";
      this.labelDescription.Size = new System.Drawing.Size(372, 21);
      this.labelDescription.TabIndex = 39;
      this.labelDescription.Text = "Description";
      this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // labelLengthResult
      // 
      this.tableLayoutPanel.SetColumnSpan(this.labelLengthResult, 2);
      this.labelLengthResult.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelLengthResult.Location = new System.Drawing.Point(403, 81);
      this.labelLengthResult.Margin = new System.Windows.Forms.Padding(3, 7, 13, 0);
      this.labelLengthResult.MaximumSize = new System.Drawing.Size(0, 21);
      this.labelLengthResult.Name = "labelLengthResult";
      this.labelLengthResult.Size = new System.Drawing.Size(139, 21);
      this.labelLengthResult.TabIndex = 28;
      this.labelLengthResult.Text = "Length - result";
      // 
      // labelLengthSection
      // 
      this.tableLayoutPanel.SetColumnSpan(this.labelLengthSection, 2);
      this.labelLengthSection.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelLengthSection.Location = new System.Drawing.Point(403, 44);
      this.labelLengthSection.Margin = new System.Windows.Forms.Padding(3, 7, 13, 0);
      this.labelLengthSection.MaximumSize = new System.Drawing.Size(0, 21);
      this.labelLengthSection.Name = "labelLengthSection";
      this.labelLengthSection.Size = new System.Drawing.Size(139, 21);
      this.labelLengthSection.TabIndex = 27;
      this.labelLengthSection.Text = "Length - section";
      // 
      // labelLengthGeometry
      // 
      this.tableLayoutPanel.SetColumnSpan(this.labelLengthGeometry, 2);
      this.labelLengthGeometry.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelLengthGeometry.Location = new System.Drawing.Point(403, 7);
      this.labelLengthGeometry.Margin = new System.Windows.Forms.Padding(3, 7, 13, 0);
      this.labelLengthGeometry.MaximumSize = new System.Drawing.Size(0, 21);
      this.labelLengthGeometry.Name = "labelLengthGeometry";
      this.labelLengthGeometry.Size = new System.Drawing.Size(139, 21);
      this.labelLengthGeometry.TabIndex = 26;
      this.labelLengthGeometry.Text = "Length - geometry";
      // 
      // labelMoment
      // 
      this.labelMoment.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelMoment.Location = new System.Drawing.Point(3, 44);
      this.labelMoment.Margin = new System.Windows.Forms.Padding(3, 7, 13, 0);
      this.labelMoment.MaximumSize = new System.Drawing.Size(0, 21);
      this.labelMoment.Name = "labelMoment";
      this.labelMoment.Size = new System.Drawing.Size(113, 21);
      this.labelMoment.TabIndex = 25;
      this.labelMoment.Text = "Moment";
      // 
      // labelForce
      // 
      this.labelForce.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelForce.Location = new System.Drawing.Point(3, 7);
      this.labelForce.Margin = new System.Windows.Forms.Padding(3, 7, 13, 0);
      this.labelForce.MaximumSize = new System.Drawing.Size(0, 21);
      this.labelForce.Name = "labelForce";
      this.labelForce.Size = new System.Drawing.Size(113, 21);
      this.labelForce.TabIndex = 19;
      this.labelForce.Text = "Force";
      // 
      // labelMass
      // 
      this.labelMass.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelMass.Location = new System.Drawing.Point(403, 155);
      this.labelMass.Margin = new System.Windows.Forms.Padding(3, 7, 13, 0);
      this.labelMass.MaximumSize = new System.Drawing.Size(0, 21);
      this.labelMass.Name = "labelMass";
      this.labelMass.Size = new System.Drawing.Size(52, 21);
      this.labelMass.TabIndex = 0;
      this.labelMass.Text = "Mass";
      // 
      // labelStress
      // 
      this.labelStress.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelStress.Location = new System.Drawing.Point(3, 81);
      this.labelStress.Margin = new System.Windows.Forms.Padding(3, 7, 13, 0);
      this.labelStress.MaximumSize = new System.Drawing.Size(0, 21);
      this.labelStress.Name = "labelStress";
      this.labelStress.Size = new System.Drawing.Size(113, 21);
      this.labelStress.TabIndex = 21;
      this.labelStress.Text = "Stress";
      // 
      // labelStrain
      // 
      this.labelStrain.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelStrain.Location = new System.Drawing.Point(3, 118);
      this.labelStrain.Margin = new System.Windows.Forms.Padding(3, 7, 13, 0);
      this.labelStrain.MaximumSize = new System.Drawing.Size(0, 21);
      this.labelStrain.Name = "labelStrain";
      this.labelStrain.Size = new System.Drawing.Size(113, 21);
      this.labelStrain.TabIndex = 22;
      this.labelStrain.Text = "Strain";
      // 
      // comboBoxStrain
      // 
      this.tableLayoutPanel.SetColumnSpan(this.comboBoxStrain, 2);
      this.comboBoxStrain.Dock = System.Windows.Forms.DockStyle.Fill;
      this.comboBoxStrain.FormattingEnabled = true;
      this.comboBoxStrain.Location = new System.Drawing.Point(133, 115);
      this.comboBoxStrain.Margin = new System.Windows.Forms.Padding(4);
      this.comboBoxStrain.Name = "comboBoxStrain";
      this.comboBoxStrain.Size = new System.Drawing.Size(252, 24);
      this.comboBoxStrain.TabIndex = 34;
      this.comboBoxStrain.SelectedIndexChanged += new System.EventHandler(this.comboBoxStrain_SelectedIndexChanged);
      // 
      // comboBoxStress
      // 
      this.tableLayoutPanel.SetColumnSpan(this.comboBoxStress, 2);
      this.comboBoxStress.Dock = System.Windows.Forms.DockStyle.Fill;
      this.comboBoxStress.FormattingEnabled = true;
      this.comboBoxStress.Location = new System.Drawing.Point(133, 78);
      this.comboBoxStress.Margin = new System.Windows.Forms.Padding(4);
      this.comboBoxStress.Name = "comboBoxStress";
      this.comboBoxStress.Size = new System.Drawing.Size(252, 24);
      this.comboBoxStress.TabIndex = 33;
      // 
      // comboBoxMoment
      // 
      this.tableLayoutPanel.SetColumnSpan(this.comboBoxMoment, 2);
      this.comboBoxMoment.Dock = System.Windows.Forms.DockStyle.Fill;
      this.comboBoxMoment.FormattingEnabled = true;
      this.comboBoxMoment.Location = new System.Drawing.Point(133, 41);
      this.comboBoxMoment.Margin = new System.Windows.Forms.Padding(4);
      this.comboBoxMoment.Name = "comboBoxMoment";
      this.comboBoxMoment.Size = new System.Drawing.Size(252, 24);
      this.comboBoxMoment.TabIndex = 32;
      this.comboBoxMoment.SelectedIndexChanged += new System.EventHandler(this.comboBoxMoment_SelectedIndexChanged);
      // 
      // comboBoxForce
      // 
      this.tableLayoutPanel.SetColumnSpan(this.comboBoxForce, 2);
      this.comboBoxForce.Dock = System.Windows.Forms.DockStyle.Fill;
      this.comboBoxForce.FormattingEnabled = true;
      this.comboBoxForce.Location = new System.Drawing.Point(133, 4);
      this.comboBoxForce.Margin = new System.Windows.Forms.Padding(4);
      this.comboBoxForce.Name = "comboBoxForce";
      this.comboBoxForce.Size = new System.Drawing.Size(252, 24);
      this.comboBoxForce.TabIndex = 29;
      this.comboBoxForce.SelectedIndexChanged += new System.EventHandler(this.comboBoxForce_SelectedIndexChanged);
      // 
      // comboBoxMass
      // 
      this.tableLayoutPanel.SetColumnSpan(this.comboBoxMass, 4);
      this.comboBoxMass.Dock = System.Windows.Forms.DockStyle.Fill;
      this.comboBoxMass.FormattingEnabled = true;
      this.comboBoxMass.Location = new System.Drawing.Point(559, 152);
      this.comboBoxMass.Margin = new System.Windows.Forms.Padding(4);
      this.comboBoxMass.Name = "comboBoxMass";
      this.comboBoxMass.Size = new System.Drawing.Size(255, 24);
      this.comboBoxMass.TabIndex = 30;
      this.comboBoxMass.SelectedIndexChanged += new System.EventHandler(this.comboBoxMass_SelectedIndexChanged);
      // 
      // comboBoxLengthResult
      // 
      this.tableLayoutPanel.SetColumnSpan(this.comboBoxLengthResult, 4);
      this.comboBoxLengthResult.Dock = System.Windows.Forms.DockStyle.Fill;
      this.comboBoxLengthResult.FormattingEnabled = true;
      this.comboBoxLengthResult.Location = new System.Drawing.Point(559, 78);
      this.comboBoxLengthResult.Margin = new System.Windows.Forms.Padding(4);
      this.comboBoxLengthResult.Name = "comboBoxLengthResult";
      this.comboBoxLengthResult.Size = new System.Drawing.Size(255, 24);
      this.comboBoxLengthResult.TabIndex = 37;
      this.comboBoxLengthResult.SelectedIndexChanged += new System.EventHandler(this.comboBoxLengthResult_SelectedIndexChanged);
      // 
      // comboBoxLengthSection
      // 
      this.tableLayoutPanel.SetColumnSpan(this.comboBoxLengthSection, 4);
      this.comboBoxLengthSection.Dock = System.Windows.Forms.DockStyle.Fill;
      this.comboBoxLengthSection.FormattingEnabled = true;
      this.comboBoxLengthSection.Location = new System.Drawing.Point(559, 41);
      this.comboBoxLengthSection.Margin = new System.Windows.Forms.Padding(4);
      this.comboBoxLengthSection.Name = "comboBoxLengthSection";
      this.comboBoxLengthSection.Size = new System.Drawing.Size(255, 24);
      this.comboBoxLengthSection.TabIndex = 36;
      this.comboBoxLengthSection.SelectedIndexChanged += new System.EventHandler(this.comboBoxLengthSection_SelectedIndexChanged);
      // 
      // comboBoxLengthGeometry
      // 
      this.tableLayoutPanel.SetColumnSpan(this.comboBoxLengthGeometry, 4);
      this.comboBoxLengthGeometry.Dock = System.Windows.Forms.DockStyle.Fill;
      this.comboBoxLengthGeometry.FormattingEnabled = true;
      this.comboBoxLengthGeometry.Location = new System.Drawing.Point(559, 4);
      this.comboBoxLengthGeometry.Margin = new System.Windows.Forms.Padding(4);
      this.comboBoxLengthGeometry.Name = "comboBoxLengthGeometry";
      this.comboBoxLengthGeometry.Size = new System.Drawing.Size(255, 24);
      this.comboBoxLengthGeometry.TabIndex = 35;
      this.comboBoxLengthGeometry.SelectedIndexChanged += new System.EventHandler(this.comboBoxLengthGeometry_SelectedIndexChanged);
      // 
      // pictureBox1
      // 
      this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.Location = new System.Drawing.Point(779, 229);
      this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(35, 29);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.pictureBox1.TabIndex = 38;
      this.pictureBox1.TabStop = false;
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.tableLayoutPanel.SetColumnSpan(this.cancelButton, 2);
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(133, 231);
      this.cancelButton.Margin = new System.Windows.Forms.Padding(4);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(121, 28);
      this.cancelButton.TabIndex = 31;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(4, 231);
      this.okButton.Margin = new System.Windows.Forms.Padding(4);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(121, 28);
      this.okButton.TabIndex = 24;
      this.okButton.Text = "O&K";
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // useRhinoToleranceCheckBox
      // 
      this.useRhinoToleranceCheckBox.AutoSize = true;
      this.useRhinoToleranceCheckBox.Location = new System.Drawing.Point(220, 189);
      this.useRhinoToleranceCheckBox.Margin = new System.Windows.Forms.Padding(4);
      this.useRhinoToleranceCheckBox.Name = "useRhinoToleranceCheckBox";
      this.useRhinoToleranceCheckBox.Size = new System.Drawing.Size(151, 20);
      this.useRhinoToleranceCheckBox.TabIndex = 51;
      this.useRhinoToleranceCheckBox.Text = "Use Rhino tolerance";
      this.useRhinoToleranceCheckBox.UseVisualStyleBackColor = true;
      this.useRhinoToleranceCheckBox.CheckedChanged += new System.EventHandler(this.useRhinoToleranceCheckBox_CheckedChanged);
      // 
      // toleranceValueBox
      // 
      this.toleranceValueBox.DecimalPlaces = 4;
      this.toleranceValueBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
      this.toleranceValueBox.Location = new System.Drawing.Point(133, 189);
      this.toleranceValueBox.Margin = new System.Windows.Forms.Padding(4);
      this.toleranceValueBox.Name = "toleranceValueBox";
      this.toleranceValueBox.Size = new System.Drawing.Size(78, 22);
      this.toleranceValueBox.TabIndex = 49;
      this.toleranceValueBox.ValueChanged += new System.EventHandler(this.toleranceValueBox_ValueChanged);
      // 
      // buttonSIUnits
      // 
      this.buttonSIUnits.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.buttonSIUnits.Location = new System.Drawing.Point(475, 189);
      this.buttonSIUnits.Margin = new System.Windows.Forms.Padding(4);
      this.buttonSIUnits.Name = "buttonSIUnits";
      this.buttonSIUnits.Size = new System.Drawing.Size(72, 27);
      this.buttonSIUnits.TabIndex = 40;
      this.buttonSIUnits.Text = "SI";
      this.buttonSIUnits.UseVisualStyleBackColor = true;
      this.buttonSIUnits.Click += new System.EventHandler(this.buttonSI_Click);
      // 
      // buttonKNmUnits
      // 
      this.buttonKNmUnits.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.buttonKNmUnits.Location = new System.Drawing.Point(561, 189);
      this.buttonKNmUnits.Margin = new System.Windows.Forms.Padding(4);
      this.buttonKNmUnits.Name = "buttonKNmUnits";
      this.buttonKNmUnits.Size = new System.Drawing.Size(72, 27);
      this.buttonKNmUnits.TabIndex = 42;
      this.buttonKNmUnits.Text = "kN-m";
      this.buttonKNmUnits.UseVisualStyleBackColor = true;
      this.buttonKNmUnits.Click += new System.EventHandler(this.buttonkNm_Click);
      // 
      // buttonKipFtUnits
      // 
      this.buttonKipFtUnits.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.buttonKipFtUnits.Location = new System.Drawing.Point(645, 189);
      this.buttonKipFtUnits.Margin = new System.Windows.Forms.Padding(4);
      this.buttonKipFtUnits.Name = "buttonKipFtUnits";
      this.buttonKipFtUnits.Size = new System.Drawing.Size(72, 27);
      this.buttonKipFtUnits.TabIndex = 41;
      this.buttonKipFtUnits.Text = "kip-ft";
      this.buttonKipFtUnits.UseVisualStyleBackColor = true;
      this.buttonKipFtUnits.Click += new System.EventHandler(this.buttonkipFt_Click);
      // 
      // buttonKipInUnits
      // 
      this.tableLayoutPanel.SetColumnSpan(this.buttonKipInUnits, 2);
      this.buttonKipInUnits.Location = new System.Drawing.Point(728, 189);
      this.buttonKipInUnits.Margin = new System.Windows.Forms.Padding(4);
      this.buttonKipInUnits.Name = "buttonKipInUnits";
      this.buttonKipInUnits.Size = new System.Drawing.Size(72, 27);
      this.buttonKipInUnits.TabIndex = 43;
      this.buttonKipInUnits.Text = "kip-in";
      this.buttonKipInUnits.UseVisualStyleBackColor = true;
      this.buttonKipInUnits.Click += new System.EventHandler(this.buttonkipIn_Click);
      // 
      // UnitSettingsBox
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(842, 285);
      this.Controls.Add(this.tableLayoutPanel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Margin = new System.Windows.Forms.Padding(4);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "UnitSettingsBox";
      this.Padding = new System.Windows.Forms.Padding(12, 11, 12, 11);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Default GSA Units";
      this.Load += new System.EventHandler(this.UnitSettingsBox_Load);
      this.tableLayoutPanel.ResumeLayout(false);
      this.tableLayoutPanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.toleranceValueBox)).EndInit();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelForce;
        private System.Windows.Forms.Label labelMass;
        private System.Windows.Forms.Label labelStress;
        private System.Windows.Forms.Label labelStrain;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label labelMoment;
        private System.Windows.Forms.Label labelLengthGeometry;
        private System.Windows.Forms.Label labelLengthResult;
        private System.Windows.Forms.Label labelLengthSection;
        private System.Windows.Forms.ComboBox comboBoxForce;
        private System.Windows.Forms.ComboBox comboBoxMass;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox comboBoxMoment;
        private System.Windows.Forms.ComboBox comboBoxStress;
        private System.Windows.Forms.ComboBox comboBoxStrain;
        private System.Windows.Forms.ComboBox comboBoxLengthGeometry;
        private System.Windows.Forms.ComboBox comboBoxLengthSection;
        private System.Windows.Forms.ComboBox comboBoxLengthResult;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Button buttonSIUnits;
        private System.Windows.Forms.Button buttonKipFtUnits;
        private System.Windows.Forms.Button buttonKNmUnits;
        private System.Windows.Forms.Button buttonKipInUnits;
        private System.Windows.Forms.ComboBox comboBoxTemperature;
        private System.Windows.Forms.Label labelTemperature;
    private System.Windows.Forms.Label labelTolerance;
    private System.Windows.Forms.NumericUpDown toleranceValueBox;
    private System.Windows.Forms.CheckBox useRhinoToleranceCheckBox;
    private System.Windows.Forms.ComboBox comboBoxEnergy;
    private System.Windows.Forms.Label labelEnergy;
  }
}
