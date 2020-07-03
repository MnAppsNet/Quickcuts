using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Quickcuts
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                if ( args[0] == "reset")
                {
                    if (Properties.Settings.Default.path != "")
                    {
                        Properties.Settings.Default.Reset();
                        Properties.Settings.Default.Save();
                        Application.Restart();
                    }
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
