using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Globalization;
using Microsoft.Win32;

namespace SetLocalTime
{

    public partial class Form1 : Form
    {
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);

        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMiliseconds;
        }
        public static bool SetLocalTimeByStr(string timestr)
        {
            bool successful = false;
            SystemTime sysTime = new SystemTime();
            try
            {
                DateTime dt = Convert.ToDateTime(timestr);   // 將字串轉成DateTime格式

                sysTime.wYear = Convert.ToUInt16(dt.Year);
                sysTime.wMonth = Convert.ToUInt16(dt.Month);
                sysTime.wDay = Convert.ToUInt16(dt.Day);
                sysTime.wHour = Convert.ToUInt16(dt.Hour);
                sysTime.wMinute = Convert.ToUInt16(dt.Minute);
                sysTime.wSecond = Convert.ToUInt16(dt.Second);
                successful = SetLocalTime(ref sysTime);
            }
            catch (Exception e)
            {
                MessageBox.Show("SetLocalTimeByStr Error" + e.Message);
            }
            return successful;
        }
        public Form1()
        {
            
            InitializeComponent();
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            //dateTimePicker1.CustomFormat = "'Today is:' hh:mm:ss dddd MMMM dd, yyyy";
            dateTimePicker1.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            Get_WeekOfYear();
        }
        private void Get_WeekOfYear()
        {
            // Gets the Calendar instance associated with a CultureInfo.
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;
            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            lb_Week.Text = String.Format("{0}", myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW));
        }

        private void btGetTime_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now;
            Get_WeekOfYear();
        }

        private void btSetTime_Click(object sender, EventArgs e)
        {
            SetLocalTimeByStr(dateTimePicker1.Value.ToString());
            Get_WeekOfYear();
        }

        //開機自動啟動
        public static void RegisterInStartup(string name, string path, bool onStartOpen)
        {
            try
            {
                var registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (onStartOpen)
                    registryKey.SetValue(name, path);//註冊打開
                else
                    registryKey.DeleteValue(name);//刪除註冊
            }
            catch (Exception e)
            {
                //https://www.ruyut.com/
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            notifyIcon1.Text = "Week:" + lb_Week.Text;
        }
    }
}
