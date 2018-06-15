﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IASAqueue_Server
{
    public enum Status { Online, Offline, Paused }
    public partial class MainForm : Form
    {
        Monitor monitor;
        Screen screen;
        Screen[] screens;
        Server server;

        public MainForm()
        {
            InitializeComponent();    
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            monitor = new Monitor();
            Refresh_DisplayList();
            lbl_inQueue.Text = "0";
            lbl_Uptime.Text = "00:00:00";
        }

        private void btn_Monitor_Click(object sender, EventArgs e)
        {
            Rectangle bounds = screen.Bounds;
            monitor = new Monitor();
            monitor.StartPosition = FormStartPosition.Manual;
            monitor.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            monitor.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            screen = screens[cb_Displays.SelectedIndex];
        }

        private void Refresh_DisplayList()
        {
            screens = Screen.AllScreens;
            cb_Displays.Items.Clear();
            foreach (Screen scr in screens)
                cb_Displays.Items.Add(scr.DeviceName);
            cb_Displays.SelectedIndex = cb_Displays.Items.Count - 1;
        }

        private void Refresh_Status()
        {
            if (server.GetSetStatus == Status.Offline)
            {
                lbl_Status.ForeColor = Color.Red;
                lbl_Status.Text = "Offline";
            }
            else if (server.GetSetStatus == Status.Online)
            {
                lbl_Status.ForeColor = Color.Green;
                lbl_Status.Text = "Online";
            }
            else if (server.GetSetStatus == Status.Paused)
            {
                lbl_Status.ForeColor = Color.Yellow;
                lbl_Status.Text = "Paused";
            }
            else
            {
                lbl_Status.ForeColor = Color.Black;
                lbl_Status.Text = "None";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int port;
            if (int.TryParse(tb_Port.Text, out port))
            {
                server = new Server(port, rtb_Logs);
                server.StatusChanged += Server_StatusChanged;
                Thread thread = new Thread(server.Start);
                thread.Start();
            }
            else
            {
                rtb_Logs.Text += "Wrong port...\n";
            }
        }

        private void Server_StatusChanged(object sender, EventArgs e)
        {
            this.BeginInvoke((MethodInvoker)(() => this.Refresh_Status()));
        }
    }
}
