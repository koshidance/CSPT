using Microsoft.Win32;
using System.Diagnostics;

namespace CSPT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            SetTextBox1();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (TestToDonbass(GetTime(textBox2.Text)))
            {
                DialogResult a = MessageBox.Show($"Do you want to set the \"Instant Replay\" time to {textBox2.Text} minutes", "", MessageBoxButtons.OKCancel);
                if (a == DialogResult.OK)
                {
                    SetTimeNVIDIA(GetTime(textBox2.Text));
                    if (checkBox1.Checked) ReloadPC();
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            SetTextBox1();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult a = MessageBox.Show($"Do you want to set the \"Instant Replay\" time to 20 minutes(default)", "", MessageBoxButtons.OKCancel);
            if (a == DialogResult.OK)
            {
                SetTimeNVIDIA(20);
                if (checkBox1.Checked) ReloadPC();
            }
        }
        private void label3_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.github.com/koshidance",
                UseShellExecute = true
            });
        }
        private int HexToInt(string hex)
        {
            int res = 0;
            hex = hex.Replace(" ", "");
            for(int i = 0; i < hex.Length; i++)
            {
                int d = 0;
                string h = hex[i].ToString().ToLower();
                if (h == "a") d = 10;
                else if (h == "b") d = 11;
                else if (h == "c") d = 12;
                else if (h == "d") d = 13;
                else if (h == "e") d = 14;
                else if (h == "f") d = 15;
                else if (h == "9") d = 9;
                else if (h == "8") d = 8;
                else if (h == "7") d = 7;
                else if (h == "6") d = 6;
                else if (h == "5") d = 5;
                else if (h == "4") d = 4;
                else if (h == "3") d = 3;
                else if (h == "2") d = 2;
                else if (h == "1") d = 1;
                else d = 0;

                int ff = (int)(Math.Pow(16, hex.Length-i-1) * d);
                if (ff == -2147483648) ff = 0;
                res += ff;
            }
            return res;
        }
        private string ByteTo(byte[] array)
        {
            string a = "";
            for(int i= array.Length-1; i >= 0;i--)
            {
                string ss = array[i].ToString("X");
                if (ss.Length == 1) ss = "0" + ss;
                a += $"{ss} ";
            }
            return a;
        }
        private void SetTextBox1()
        {
            byte[] time = (byte[])GetObjectNVIDIA();
            string hex = ByteTo(time);
            textBox1.Text = (HexToInt(hex)/60) + "min";
        }
        private object GetObjectNVIDIA()
        {
            RegistryKey cuk = Registry.CurrentUser;
            RegistryKey s = cuk!.OpenSubKey("Software")!.OpenSubKey("NVIDIA Corporation")!.OpenSubKey("Global")!.OpenSubKey("ShadowPlay")!.OpenSubKey("NVSPCAPS")!;
            return s!.GetValue("DVRBufferLen")!;
        }
        private void SetObjectNVIDIA(object time)
        {
            RegistryKey cuk = Registry.CurrentUser;
            RegistryKey s = cuk!.OpenSubKey("Software")!.OpenSubKey("NVIDIA Corporation")!.OpenSubKey("Global")!.OpenSubKey("ShadowPlay")!.OpenSubKey("NVSPCAPS", true)!;
            s.SetValue("DVRBufferLen", time);
        }
        private int GetTime(string textbox)
        {
            try
            {
                return Convert.ToInt32(textbox);
            }
            catch
            {
                return -1;
            }
        }
        private bool TestToDonbass(int time)
        {
            if(time == -1)
            {
                MessageBox.Show("Incorrect number entered!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if(time < 5 || time > 1440)
            {
                MessageBox.Show("The time cannot be less than 5 minutes and more than 24 hours!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
            
        }
        private void SetTimeNVIDIA(int time)
        {
            time = time * 60;
            string dex = time.ToString("X");
            while(dex.Length < 8)
            {
                dex = "0" + dex;
            }
            byte[] hex = { 00, 00, 00, 00 };
            hex[3] = (byte)int.Parse($"{dex[0]}{dex[1]}", System.Globalization.NumberStyles.HexNumber);
            hex[2] = (byte)int.Parse($"{dex[2]}{dex[3]}", System.Globalization.NumberStyles.HexNumber);
            hex[1] = (byte)int.Parse($"{dex[4]}{dex[5]}", System.Globalization.NumberStyles.HexNumber);
            hex[0] = (byte)int.Parse($"{dex[6]}{dex[7]}", System.Globalization.NumberStyles.HexNumber);

            SetObjectNVIDIA(hex);

            MessageBox.Show($"The time has been successfully set to {time/60} minutes\n\n\n{(checkBox1.Checked ? ("After closing this window, the computer will restart in 15 seconds") : ("To make the settings take effect, restart the PC"))}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SetTextBox1();
        }
        private void ReloadPC()
        {
            Process.Start("shutdown", "/r /t 15");
        }
    }
}