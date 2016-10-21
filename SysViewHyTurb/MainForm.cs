﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysViewHyTurb
{
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class MainForm : Form
    {
        private AppDriverContainer container;

        /// <summary>
        /// Gets the config file path.
        /// </summary>
        /// <value>
        /// The config file path.
        /// </value>
        public static string ConfigFilePath
        {
            get
            {
                var assemblyPath = Path.GetDirectoryName(typeof(Program).GetTypeInfo().Assembly.Location);

#if DEBUG
                // This lets you edit the files without restarting the server.
                return Directory.GetParent(assemblyPath).Parent.FullName + "\\SysViewCp.xml";
#else
                // This is when you have deployed the server.
                return Path.Combine(assemblyPath, "html");
#endif
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.container = new AppDriverContainer(ConfigFilePath);
               
            }
            catch (Exception err)
            {
                MessageBox.Show("程序初始化失败，请检查配置文件SysView.xml格式  " + err.Message);
                Application.Exit();
            }


            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            if (!WBEmulator.IsBrowserEmulationSet())
            {
                WBEmulator.SetBrowserEmulationVersion();
            }
            this.MainwebBrowser.AllowWebBrowserDrop = false;
            //this.MainwebBrowser.IsWebBrowserContextMenuEnabled = false;
            this.MainwebBrowser.Navigate(Program.Url);
            this.MainwebBrowser.ObjectForScripting = this;
            


        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.EmbedWebServer.Dispose();
        }

        private void MainwebBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //MessageBox.Show(e.KeyCode.ToString());
            if (this.skipPreview)
            {
                this.skipPreview = false;
            }
            else
            {
                if (e.KeyCode == Keys.F11)
                {
                    if (this.FormBorderStyle == FormBorderStyle.None)
                    {
                        this.FormBorderStyle = FormBorderStyle.Sizable;
                        this.WindowState = FormWindowState.Normal;
                        this.TopMost = false;
                    }
                    else
                    {
                        this.FormBorderStyle = FormBorderStyle.None;
                        this.WindowState = FormWindowState.Maximized;
                        this.TopMost = true;
                    }
                }
                this.skipPreview = true;
            }
              
        }

        private bool skipPreview;
    }
}
