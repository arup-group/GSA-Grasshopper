using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Grasshopper;
using Grasshopper.Kernel;

namespace GsaGH.Graphics {
  internal partial class AboutBox : Form {
    public string AssemblyCompany => "Copyright © Oasys 1985 - 2025";
    public string AssemblyDescription
      => "GSA is the software of choice for advanced analysis and design of buildings, bridges and tensile structures. "
        + "It provides a complete toolkit, with a comprehensive suite of analysis options. Designed by engineers for engineers.";
    public string AssemblyTitle {
      get {
        object[] attributes = Assembly.GetExecutingAssembly()
         .GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        if (attributes.Length <= 0) {
          return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
        }

        var titleAttribute = (AssemblyTitleAttribute)attributes[0];
        return titleAttribute.Title != string.Empty ? titleAttribute.Title :
          Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }

    public AboutBox() {
      GH_AssemblyInfo gsaPlugin
        = Instances.ComponentServer.FindAssembly(new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f"));

      const string api = "GSA 10.2 .NET API beta";
      string pluginvers = gsaPlugin.Version;
      string pluginloc = gsaPlugin.Location;

      InitializeComponent();
      Text = $"About GSA Grasshopper plugin";
      labelProductName.Text = "GSA Grasshopper Plugin";
      labelVersion.Text = $"Version {pluginvers}";
      labelApiVersion.Text = $"API Version {api}";
      labelCompanyName.Text = AssemblyCompany;
      linkWebsite.Text = @"www.oasys-software.com";
      labelContact.Text = "Contact and support:";
      linkEmail.Text = @"oasys@arup.com";
      disclaimer.Text = GsaGhInfo.isBeta ? GsaGhInfo.disclaimer : string.Empty;
    }

    private void AboutBox_Load(object sender, EventArgs e) { }

    private void Button1_Click(object sender, EventArgs e) {
      Process.Start(@"rhino://package/search?name=guid:a3b08c32-f7de-4b00-b415-f8b466f05e9f");
    }

    private void LinkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      GH_AssemblyInfo gsaPlugin
        = Instances.ComponentServer.FindAssembly(new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f"));
      string pluginvers = gsaPlugin.Version;
      Process.Start(@"mailto:oasys@arup.com?subject=Oasys GsaGH version " + pluginvers);
    }

    private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      Process.Start(@"https://www.oasys-software.com/");
    }

    private void OkButton_Click(object sender, EventArgs e) {
      Close();
    }
  }
}
