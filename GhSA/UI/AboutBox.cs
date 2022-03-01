using Grasshopper.Kernel;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace GsaGH.UI
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            GH_AssemblyInfo gsaPlugin = Grasshopper.Instances.ComponentServer.FindAssembly(new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f"));

            string api = "GSA 10.1 .NET API beta"; // IVersion.Api();
            string pluginvers = gsaPlugin.Version;
            string pluginloc = gsaPlugin.Location;

            InitializeComponent();
            this.Text = String.Format("About {0}", "GsaGH");
            this.labelProductName.Text = "GSA Grasshopper Plugin";
            this.labelVersion.Text = String.Format("Version {0}", pluginvers);
            this.labelApiVersion.Text = String.Format("API Version {0}", api);
            this.labelCompanyName.Text = AssemblyCompany;
            this.linkWebsite.Text = @"www.oasys-software.com";
            this.labelContact.Text = "Contact and support:"; 
            this.linkEmail.Text = @"oasys@arup.com";
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyDescription
        {
            get
            {
                return "GSA is the software of choice for advanced analysis and design of buildings, bridges and tensile structures. " +
                    "It provides a complete toolkit, with a comprehensive suite of analysis options. Designed by engineers for engineers.";
            }
        }

        public string AssemblyCompany
        {
            get
            {
                return "Copyright © Oasys 1985 - 2022";
            }
        }
        #endregion

        private void labelProductName_Click(object sender, EventArgs e)
        {

        }

        private void labelVersion_Click(object sender, EventArgs e)
        {

        }

        private void AboutBox_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(@"https://www.oasys-software.com/");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(@"rhino://package/search?name=gsa");
        }

        private void linkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GH_AssemblyInfo gsaPlugin = Grasshopper.Instances.ComponentServer.FindAssembly(new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f"));
            string pluginvers = gsaPlugin.Version;
            Process.Start(@"mailto:oasys@arup.com?subject=Oasys GsaGH version " + pluginvers);
        }

        private void labelApiVersion_Click(object sender, EventArgs e)
        {

        }
    }
}
