using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quickcuts
{
    class Shortcut
    {
        #region Static Variables
        public static string[] IconPaths = { @"\logo.png", @"\logo.jpg" };
        public static System.Drawing.Size defaultSize = new System.Drawing.Size(90,90);
        public static int Spacing = 5;
        #endregion -----------------------------------------------------------------------

        #region Private Variables
        private string folderName;
        private string path;
        private int index;
        private System.Windows.Forms.PictureBox shortcut;
        private Form1 parent;
        private System.Drawing.Size ZoomIconSize = new System.Drawing.Size(7, 7);
        System.Windows.Forms.Timer animator; //Used for zoom effect
        #endregion -----------------------------------------------------------------------


        public Shortcut(Form1 Parent, int Index, string FolderName, string Path, System.Windows.Forms.PictureBox TemplateObject)
        { // Constructor \\
            parent = Parent;
            folderName = FolderName;
            path = Path;
            index = Index;
            ZoomIconSize = new System.Drawing.Size(TemplateObject.Width + ZoomIconSize.Width, TemplateObject.Height + ZoomIconSize.Height);
            CreateShortcut(TemplateObject);
        }

        #region Private Methodes
        //Create Shortcut with another Picturebox object as a template to copy it's properties
        private void CreateShortcut(System.Windows.Forms.PictureBox TemplateObject)
        {
            // Create shortcut object based on template object
            shortcut = new System.Windows.Forms.PictureBox();
            shortcut.Size = defaultSize;
            shortcut.Location = TemplateObject.Location;
            shortcut.Anchor = TemplateObject.Anchor;
            shortcut.BackColor = TemplateObject.BackColor;
            shortcut.BorderStyle = TemplateObject.BorderStyle;
            shortcut.SizeMode = TemplateObject.SizeMode;
            shortcut.MouseDown += parent.MouseClickEvent;
            shortcut.MouseEnter += ZoomEffect;
            shortcut.MouseLeave += DefaultState;
            shortcut.Cursor = TemplateObject.Cursor;
            shortcut.Tag = path + @"\" + folderName; //keep shortcut folder in tag for easy access if needed
            shortcut.Name = "_icon_" + folderName;
            shortcut.Location = new System.Drawing.Point((shortcut.Location.X + shortcut.Size.Width + Spacing) * index, shortcut.Location.Y);
            parent.Controls.Add(shortcut);

            //Get shortcut icon :
            shortcut.Image = Properties.Resources.default_icon;
            foreach (string s in IconPaths)
            {
                if (System.IO.File.Exists(path + @"\" + folderName + s))
                {
                    shortcut.ImageLocation = path + @"\" + folderName + s;
                }
            }
            // Create "open folder" button on the buttom right of the shortcut
            System.Windows.Forms.PictureBox B = new System.Windows.Forms.PictureBox();
            B.Name = "_btn_" + folderName;
            B.Text = "...";
            B.BackColor = System.Drawing.Color.Transparent;
            B.Cursor = TemplateObject.Cursor;
            B.Size = new System.Drawing.Size(15, 15);
            B.Image = Properties.Resources.open_icon;
            B.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            B.BorderStyle = System.Windows.Forms.BorderStyle.None;
            B.Tag = path + @"\" + folderName;
            B.Click += OpenFolder;
            shortcut.Controls.Add(B);
            B.Location = new System.Drawing.Point(shortcut.Width - B.Width, shortcut.Height - B.Height);

            // Create ToolTip for shortcuts with event actions
            System.Windows.Forms.ToolTip T = new System.Windows.Forms.ToolTip();
            T.InitialDelay = 1000;
            T.IsBalloon = true;
            string[] action = { //possible events
                        "Left",
                        "Right",
                        "Middle"
                    };
            //Get actions of each event :
            for (int j = 0; j < action.Length; j++)
            {
                try
                {
                    action[j] = (new System.IO.FileInfo(parent.GetClickActionPath(shortcut.Tag.ToString(), action[j]))).Name;
                }
                catch
                {
                    action[j] = "-";
                }
            }
            //format ToolTip text :
            string options = "Left\t: " + action[0] + Environment.NewLine +
                             "Right\t: " + action[1] + Environment.NewLine +
                             "Middle\t: " + action[2];
            //Set ToolTip to shortcut
            T.SetToolTip(shortcut, options);
        }
        private void OpenFolder(object sender, EventArgs e) //Open shortcut folder
        {
            System.Diagnostics.Process.Start(((System.Windows.Forms.PictureBox)sender).Tag.ToString());
        }
        private void ZoomEffect(object sender, EventArgs e) //Zoom effect on MouseOver event
        {
            Animate();
        }
        private void DefaultState(object sender, EventArgs e)
        {
            animator.Stop();
            ((System.Windows.Forms.PictureBox)sender).Size = defaultSize;
        }
        private void Animate()
        {
            animator = new System.Windows.Forms.Timer();
            animator.Interval = 1;
            animator.Tick += Animation_Tick;
            animator.Start();
        }
        private void Animation_Tick(object sender, EventArgs e)
        {
            if (shortcut != null)
            {
                if (
                    (shortcut.Size.Width >= ZoomIconSize.Width) ||
                    (shortcut.Size.Height >= ZoomIconSize.Height)
                   )
                {
                    ((System.Windows.Forms.Timer)sender).Stop();
                    ((System.Windows.Forms.Timer)sender).Dispose();
                }
                else
                {
                    shortcut.Width += 1;
                    shortcut.Height += 1;
                }
            }
        }
        #endregion -----------------------------------------------------------------------

        #region Public Methodes
        public string Name
        {
            get { return folderName; }
            set { folderName = value; }
        }
        public string Path
        {
            get { return path; }
            set { path = value; }
        }
        public System.Windows.Forms.PictureBox GetShortcut()
        {
            return shortcut;
        }
        public int Index
        {
            get { return index  ; }
            set { index = value ; }
        }
        public void Destroy()
        {
            parent.Controls.Remove(shortcut);
            shortcut.Dispose();
        }
        #endregion -----------------------------------------------------------------------
    }
}
