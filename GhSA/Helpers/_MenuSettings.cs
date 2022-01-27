using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using Grasshopper.Kernel;


namespace GhSA.UI.Menu
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
        private void AddMenuItem(object sender, ElapsedEventArgs e)
        {
            if (Grasshopper.Instances.DocumentEditor == null) return;

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
                //GhSA.UI.UnitSettingsBox unitBox = new UI.UnitSettingsBox();
                //unitBox.Show();
            });
            // add info
            oasysMenu.DropDown.Items.Add("GSA Info", Properties.Resources.GSAInfo, (s, a) =>
            {
                GhSA.UI.AboutGsaBox aboutBox = new UI.AboutGsaBox();
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
    }
}