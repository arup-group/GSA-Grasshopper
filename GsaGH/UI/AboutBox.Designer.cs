using GsaGH;
namespace GsaGH.Graphics
{
  partial class AboutBox
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
      this.components = new System.ComponentModel.Container();
      this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.logoPictureBox = new System.Windows.Forms.PictureBox();
      this.labelProductName = new System.Windows.Forms.Label();
      this.labelVersion = new System.Windows.Forms.Label();
      this.labelApiVersion = new System.Windows.Forms.Label();
      this.labelCompanyName = new System.Windows.Forms.Label();
      this.labelContact = new System.Windows.Forms.Label();
      this.linkEmail = new System.Windows.Forms.LinkLabel();
      this.Check = new System.Windows.Forms.Button();
      this.linkWebsite = new System.Windows.Forms.LinkLabel();
      this.okButton = new System.Windows.Forms.Button();
      this.disclaimer = new System.Windows.Forms.Label();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.tableLayoutPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
      this.SuspendLayout();
      //
      // tableLayoutPanel
      //
      this.tableLayoutPanel.ColumnCount = 3;
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.65128F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.48005F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.86868F));
      this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
      this.tableLayoutPanel.Controls.Add(this.labelProductName, 1, 0);
      this.tableLayoutPanel.Controls.Add(this.labelVersion, 1, 1);
      this.tableLayoutPanel.Controls.Add(this.labelApiVersion, 1, 2);
      this.tableLayoutPanel.Controls.Add(this.labelCompanyName, 1, 3);
      this.tableLayoutPanel.Controls.Add(this.labelContact, 1, 4);
      this.tableLayoutPanel.Controls.Add(this.linkEmail, 2, 4);
      this.tableLayoutPanel.Controls.Add(this.Check, 2, 1);
      this.tableLayoutPanel.Controls.Add(this.linkWebsite, 0, 6);
      this.tableLayoutPanel.Controls.Add(this.okButton, 2, 6);
      this.tableLayoutPanel.Controls.Add(this.disclaimer, 0, 5);
      this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel.Location = new System.Drawing.Point(9, 9);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 7;
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel.Size = new System.Drawing.Size(417, 256);
      this.tableLayoutPanel.TabIndex = 0;
      //
      // logoPictureBox
      //
      this.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.logoPictureBox.Image = global::GsaGH.Properties.Resources.GSALogo128;
      this.logoPictureBox.Location = new System.Drawing.Point(3, 3);
      this.logoPictureBox.Name = "logoPictureBox";
      this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 5);
      this.logoPictureBox.Size = new System.Drawing.Size(130, 129);
      this.logoPictureBox.TabIndex = 12;
      this.logoPictureBox.TabStop = false;
      //
      // labelProductName
      //
      this.tableLayoutPanel.SetColumnSpan(this.labelProductName, 2);
      this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelProductName.Location = new System.Drawing.Point(142, 0);
      this.labelProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
      this.labelProductName.MaximumSize = new System.Drawing.Size(0, 17);
      this.labelProductName.Name = "labelProductName";
      this.labelProductName.Size = new System.Drawing.Size(272, 17);
      this.labelProductName.TabIndex = 19;
      this.labelProductName.Text = "GSA Grasshopper Plugin";
      this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      //
      // labelVersion
      //
      this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelVersion.Location = new System.Drawing.Point(142, 27);
      this.labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
      this.labelVersion.MaximumSize = new System.Drawing.Size(0, 17);
      this.labelVersion.Name = "labelVersion";
      this.labelVersion.Size = new System.Drawing.Size(113, 17);
      this.labelVersion.TabIndex = 0;
      this.labelVersion.Text = "Plugin Version";
      this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      //
      // labelApiVersion
      //
      this.tableLayoutPanel.SetColumnSpan(this.labelApiVersion, 2);
      this.labelApiVersion.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelApiVersion.Location = new System.Drawing.Point(142, 54);
      this.labelApiVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
      this.labelApiVersion.MaximumSize = new System.Drawing.Size(0, 17);
      this.labelApiVersion.Name = "labelApiVersion";
      this.labelApiVersion.Size = new System.Drawing.Size(272, 17);
      this.labelApiVersion.TabIndex = 21;
      this.labelApiVersion.Text = "API Version";
      this.labelApiVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      //
      // labelCompanyName
      //
      this.tableLayoutPanel.SetColumnSpan(this.labelCompanyName, 2);
      this.labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.labelCompanyName.Location = new System.Drawing.Point(142, 81);
      this.labelCompanyName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
      this.labelCompanyName.MaximumSize = new System.Drawing.Size(0, 17);
      this.labelCompanyName.Name = "labelCompanyName";
      this.labelCompanyName.Size = new System.Drawing.Size(272, 17);
      this.labelCompanyName.TabIndex = 22;
      this.labelCompanyName.Text = "Company";
      this.labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      //
      // labelContact
      //
      this.labelContact.Location = new System.Drawing.Point(142, 108);
      this.labelContact.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
      this.labelContact.Name = "labelContact";
      this.labelContact.Size = new System.Drawing.Size(113, 17);
      this.labelContact.TabIndex = 22;
      this.labelContact.Text = "Contact and support:";
      this.labelContact.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      //
      // linkEmail
      //
      this.linkEmail.Location = new System.Drawing.Point(264, 108);
      this.linkEmail.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
      this.linkEmail.Name = "linkEmail";
      this.linkEmail.Size = new System.Drawing.Size(107, 17);
      this.linkEmail.TabIndex = 26;
      this.linkEmail.TabStop = true;
      this.linkEmail.Text = "oasys@arup.com";
      this.linkEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.linkEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkEmail_LinkClicked);
      //
      // Check
      //
      this.Check.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.Check.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.Check.Location = new System.Drawing.Point(302, 30);
      this.Check.Name = "Check";
      this.Check.Size = new System.Drawing.Size(112, 20);
      this.Check.TabIndex = 27;
      this.Check.Text = "&Check for Updates";
      this.Check.UseVisualStyleBackColor = true;
      this.Check.Click += new System.EventHandler(this.Button1_Click);
      //
      // linkWebsite
      //
      this.linkWebsite.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.linkWebsite.AutoSize = true;
      this.tableLayoutPanel.SetColumnSpan(this.linkWebsite, 2);
      this.linkWebsite.Location = new System.Drawing.Point(6, 229);
      this.linkWebsite.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
      this.linkWebsite.Name = "linkWebsite";
      this.linkWebsite.Size = new System.Drawing.Size(127, 13);
      this.linkWebsite.TabIndex = 25;
      this.linkWebsite.TabStop = true;
      this.linkWebsite.Text = "www.oasys-software.com";
      this.linkWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel1_LinkClicked);
      //
      // okButton
      //
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.okButton.Location = new System.Drawing.Point(339, 232);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 21);
      this.okButton.TabIndex = 24;
      this.okButton.Text = "&OK";
      this.okButton.Click += new System.EventHandler(this.OkButton_Click);
      //
      // disclaimer
      //
      this.disclaimer.AccessibleName = "";
      this.tableLayoutPanel.SetColumnSpan(this.disclaimer, 3);
      this.disclaimer.Location = new System.Drawing.Point(6, 135);
      this.disclaimer.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
      this.disclaimer.Name = "disclaimer";
      this.disclaimer.Size = new System.Drawing.Size(408, 81);
      this.disclaimer.TabIndex = 28;
      this.disclaimer.Text = "disclaimer";
      //
      // imageList1
      //
      this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      //
      // AboutBox
      //
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(435, 274);
      this.Controls.Add(this.tableLayoutPanel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AboutBox";
      this.Padding = new System.Windows.Forms.Padding(9);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "About GSA";
      this.Load += new System.EventHandler(this.AboutBox_Load);
      this.tableLayoutPanel.ResumeLayout(false);
      this.tableLayoutPanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    private System.Windows.Forms.PictureBox logoPictureBox;
    private System.Windows.Forms.Label labelProductName;
    private System.Windows.Forms.Label labelVersion;
    private System.Windows.Forms.Label labelApiVersion;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Label labelCompanyName;
    private System.Windows.Forms.LinkLabel linkWebsite;
    private System.Windows.Forms.Label labelContact;
    private System.Windows.Forms.LinkLabel linkEmail;
    private System.Windows.Forms.Button Check;
    private System.Windows.Forms.ImageList imageList1;
    private System.Windows.Forms.Label disclaimer;
  }
}
