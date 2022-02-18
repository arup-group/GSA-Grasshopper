using Grasshopper.Kernel;
using System;
using System.Timers;
using System.Windows.Forms;

namespace GsaGH.UI.Menu
{
    public class Loader
    {
        System.Timers.Timer loadTimer;
        System.Timers.Timer trimTimer;
        static bool MenuHasBeenAdded = false;

        public Loader() { }

        public void CreateMainMenuItem()
        {
            loadTimer = new System.Timers.Timer(500);
            loadTimer.Start();
            loadTimer.Elapsed += AddMenuItem;
            trimTimer = new System.Timers.Timer(500);
            trimTimer.Elapsed += TrimMenuItem;
        }
        
        private void AddMenuItem(object sender, ElapsedEventArgs e)
        {
            if (Grasshopper.Instances.DocumentEditor == null) return;
            GH_AssemblyInfo thisPlugin = Grasshopper.Instances.ComponentServer.FindAssembly(new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f"));
            if (thisPlugin == null) return;


            if (MenuHasBeenAdded)
            {
                loadTimer.Stop();
                trimTimer.Start();
                return;
            }

            // get main menu
            var mainMenu = Grasshopper.Instances.DocumentEditor.MainMenuStrip;

            // create menu
            ToolStripMenuItem oasysMenu = new ToolStripMenuItem("Oasys");

            // add units
            oasysMenu.DropDown.Items.Add("GSA Units", Properties.Resources.Units, (s, a) =>
            {
                UnitSettingsBox unitBox = new UnitSettingsBox();
                unitBox.Show();
            });
            // add info
            oasysMenu.DropDown.Items.Add("GSA Info", Properties.Resources.GSAInfo, (s, a) =>
            {
                AboutGsaBox aboutBox = new AboutGsaBox();
                aboutBox.Show();
            });
            
            try
            {
                mainMenu.Items.Insert(mainMenu.Items.Count - 2, oasysMenu);
                MenuHasBeenAdded = true;
                loadTimer.Stop();
                trimTimer.Start();
            }
            catch (Exception)
            {
            }
        }
        private void TrimMenuItem(object sender, ElapsedEventArgs e)
        {
            var mainMenu = Grasshopper.Instances.DocumentEditor.MainMenuStrip;
            bool removeNext = false;
            for (int i = 0; i < mainMenu.Items.Count; i++)
            {
                if (mainMenu.Items[i].ToString() == "Oasys")
                {
                    if (!removeNext)
                    {
                        removeNext = true;
                    }
                    else
                    {
                        mainMenu.Items.RemoveAt(i);
                        i--;
                    }
                }
            }
            trimTimer.Stop();
        }
    }
}