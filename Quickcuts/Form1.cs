using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Quickcuts
{
    public partial class Form1 : Form
    {
        #region Global Variables
        private const string SettingsFileName = "click.conf";
        private List<Shortcut> Shortcuts;
        private Size smallArrowSize = new Size(20, 20);
        private Size defaultArrowSize;
        private Point defaultArrowLocation;
        private Size OriginalSize;
        #endregion -----------------------------------------------------------------------


        public Form1()
        { //Constructor
            InitializeComponent();
            //Check and get path to be used for shortcuts
            if (Properties.Settings.Default.path == "")
            {
                string path = "";
                do {
                    path = Interaction.InputBox("Input the folder with projects", "Input Path", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Projects");
                    Properties.Settings.Default.path = path;
                    if (path == "")
                    {
                        Application.Exit();
                        return;
                    }
                    if (!System.IO.Directory.Exists(path))
                    {
                        MessageBox.Show("Didn't found the path");
                    }
                } while (!System.IO.Directory.Exists(path));
                Properties.Settings.Default.Save();
            }
            //Get folders in paths and create shortcuts
            Shortcut.defaultSize = arrow.Size;
            Shortcuts = new List<Shortcut>();
            GetShortcuts(Properties.Settings.Default.path);
        }


        #region UI Functionality
        private void ChangeArrow()
        {
            if (arrow.Tag.ToString() == ">")
            {
                arrow.Image = Properties.Resources.left;
                arrow.Tag = "<";
            }else
            {
                arrow.Image = Properties.Resources.right;
                arrow.Tag = ">";
            }
        }
        private void ShowProjets()
        {
            this.Size = new Size((this.Width + arrow.Width + 2 * Shortcut.Spacing) * Shortcuts.Count, this.Height);
        }
        private void HideProjects()
        {
            this.Size = OriginalSize;
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (!TopMost)
                this.SendToBack();
        }
        private void topMost(bool check)
        {
            if (check)
            {
                arrow.Size = smallArrowSize;
                arrow.Location = new Point(0, this.Height - arrow.Height);
            }
            else
            {
                arrow.Size = defaultArrowSize;
                arrow.Location = defaultArrowLocation;
            }
            this.TopMost = check;
        }
        #endregion -----------------------------------------------------------------------

        #region Shortcuts Functionality
        private void GetShortcuts(string Path)
        {
            int i = 0;
            string[] Directories = System.IO.Directory.GetDirectories(Path);
            if (Directories.Length != Shortcuts.Count())
            {
                //create shortcut for each newly detected folder
                foreach (string s in Directories)
                {
                    i++;
                    string name = (new System.IO.DirectoryInfo(s)).Name;
                    if (!Shortcuts.Exists(item => item.Name == name))
                    {
                        Shortcut shortcut = new Shortcut(this, i, name, Path, arrow);
                        Shortcuts.Add(shortcut);
                    }
                }
            }
            if (Directories.Length < Shortcuts.Count())
            {
                //remove deleted folders and fix indexes :
                int index = 0;
                List<Shortcut> tempList = new List<Shortcut>();
                foreach (Shortcut s in Shortcuts)
                {
                    if (!Directories.Contains(s.Path + @"\" + s.Name))
                    {
                        s.Destroy();
                    }
                    else
                    {
                        tempList.Add(s);
                        s.Index = index;
                        index++;
                    }
                }
                Shortcuts = tempList;
            }
        }
        private void ClickAction(PictureBox sender, string Click)
        {
            string ex = "";
            ex = GetClickActionPath(sender.Tag.ToString(), Click);
            if (!System.IO.File.Exists(ex) && !System.IO.Directory.Exists(ex))
            {
                do
                {
                    ex = Interaction.InputBox("Input a folder or program path to be executed on " + Click + " click", "Input Path", sender.Tag.ToString());
                    if (ex == "")
                    {
                        return;
                    }
                } while (!System.IO.File.Exists(ex) && !System.IO.Directory.Exists(ex));
                System.IO.File.WriteAllText(sender.Tag.ToString() + @"\" + Click + "." + SettingsFileName, ex);
            }
            System.Diagnostics.Process.Start(ex);
        }

        public string GetClickActionPath(string Path, string Click)
        {
            string ex = "";
            if (System.IO.File.Exists(Path + @"\" + Click + "." + SettingsFileName))
            {
                ex = System.IO.File.ReadAllText(Path + @"\" + Click + "." + SettingsFileName);
            }
            return ex;
        }
        #endregion -----------------------------------------------------------------------

        #region Events
        public void MouseClickEvent(object sender, MouseEventArgs e)
        {
            if (!TopMost)
                this.SendToBack();
            PictureBox S = (PictureBox)sender;

            //Handle arrow clicks :
            if (S.Tag.ToString() == ">")
            {
                if (e.Button == MouseButtons.Right)
                {
                    Application.Restart();
                    return;
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    topMost(
                        (arrow.Size == defaultArrowSize) ? true : false
                        );
                    return;
                }
                ChangeArrow();
                ShowProjets();
                GetShortcuts(Properties.Settings.Default.path);
            }
            else if (S.Tag.ToString() == "<")
            {
                if (e.Button == MouseButtons.Middle)
                {
                    Application.Exit();
                    return;
                }
                ChangeArrow();
                HideProjects();
                GetShortcuts(Properties.Settings.Default.path);
            }
            //Handle Click Event Actions :
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    ClickAction(S, "Left");
                }
                else if (e.Button == MouseButtons.Right)
                {
                    ClickAction(S, "Right");
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    ClickAction(S, "Middle");
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, Screen.PrimaryScreen.Bounds.Bottom - this.Height - 50);
            this.Size = new Size(arrow.Width + 2 * Shortcut.Spacing, arrow.Height + 2 * Shortcut.Spacing);
            OriginalSize = this.Size;
            
            defaultArrowSize = arrow.Size;
            defaultArrowLocation = arrow.Location;
            if (!TopMost)
                this.SendToBack();
        }
        #endregion -----------------------------------------------------------------------

    }
}
