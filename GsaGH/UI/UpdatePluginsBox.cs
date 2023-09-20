using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.Kernel;

namespace GsaGH.Graphics {
  internal partial class UpdatePluginsBox : Form {
    internal string ProcessString { get; private set; }
    public UpdatePluginsBox(string header, string text, string processOnOkClick, Bitmap icon) {
      InitializeComponent();
      Text = header;
      ProcessString = processOnOkClick;
      textBox.Text = text;
      pictureBox.Image = icon;
    }

    private void AboutBox_Load(object sender, EventArgs e) { }


    private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      Process.Start(@"https://www.oasys-software.com/");
    }

    private void OkButton_Click(object sender, EventArgs e) {
      Process.Start(ProcessString);
      Close();
    }

    private void LabelProductName_Click(object sender, EventArgs e) {

    }
  }
}
