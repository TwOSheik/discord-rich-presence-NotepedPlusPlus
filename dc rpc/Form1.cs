using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscordRPC;
using DiscordRPC.Logging;

namespace dc_rpc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public struct ProcessEntry32
        {
            // Token: 0x04000037 RID: 55
            public uint dwSize;

            // Token: 0x04000038 RID: 56
            public uint cntUsage;

            // Token: 0x04000039 RID: 57
            public uint th32ProcessID;

            // Token: 0x0400003A RID: 58
            public IntPtr th32DefaultHeapID;

            // Token: 0x0400003B RID: 59
            public uint th32ModuleID;

            // Token: 0x0400003C RID: 60
            public uint cntThreads;

            // Token: 0x0400003D RID: 61
            public uint th32ParentProcessID;

            // Token: 0x0400003E RID: 62
            public int pcPriClassBase;

            // Token: 0x0400003F RID: 63
            public uint dwFlags;

            // Token: 0x04000040 RID: 64
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }
        string procPID;
        // Token: 0x0600002A RID: 42
        [DllImport("KERNEL32.DLL", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processid);

        // Token: 0x0600002B RID: 43
        [DllImport("KERNEL32.DLL", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern int Process32First(IntPtr handle, ref ProcessEntry32 pe);

        // Token: 0x0600002C RID: 44
        [DllImport("KERNEL32.DLL", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern int Process32Next(IntPtr handle, ref ProcessEntry32 pe);
        public string GetProcID(int index)
        {
            string result = "";
            checked
            {
                if (index == 1 || index == 0)
                {
                    IntPtr intPtr = IntPtr.Zero;
                    uint num = 0U;
                    IntPtr intPtr2 = CreateToolhelp32Snapshot(2U, 0U);
                    if ((int)intPtr2 > 0)
                    {
                        ProcessEntry32 processEntry = default(ProcessEntry32);
                        processEntry.dwSize = (uint)Marshal.SizeOf<ProcessEntry32>(processEntry);
                        for (int num2 = Process32First(intPtr2, ref processEntry); num2 == 1; num2 = Process32Next(intPtr2, ref processEntry))
                        {
                            IntPtr intPtr3 = Marshal.AllocHGlobal((int)processEntry.dwSize);
                            Marshal.StructureToPtr<ProcessEntry32>(processEntry, intPtr3, true);
                            object obj = Marshal.PtrToStructure(intPtr3, typeof(ProcessEntry32));
                            ProcessEntry32 processEntry2 = (obj != null) ? ((ProcessEntry32)obj) : default(ProcessEntry32);
                            Marshal.FreeHGlobal(intPtr3);

                            if (processEntry2.szExeFile.Contains("notepad++") && processEntry2.cntThreads > num)
                            {
                                num = processEntry2.cntThreads;
                                intPtr = (IntPtr)((long)(unchecked((ulong)processEntry2.th32ProcessID)));
                            }
                        }
                    }
                    result = Convert.ToString(intPtr);
                    procPID = Convert.ToString(intPtr);
                }
                return result;
            }


        }

        string phpact = "";
        public DiscordRpcClient client;
        bool initalized = false;
        private void button1_Click(object sender, EventArgs e)
        {
            initalized = true;
            client = new DiscordRpcClient($"{textBox3.Text}");
            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
            client.Initialize();
            update();
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            client.Deinitialize();
            client.Dispose();
            initalized = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        bool n = false;
        private void function1()
        {
            GetProcID(0);
            int procId = -1;
            var processes = from p in Process.GetProcessesByName("notepad++")
                            select p;

            foreach (var process in processes)
            {
                string titlee = process.MainWindowTitle;
                string resultado = titlee.Substring(titlee.LastIndexOf(@"\") + 1);

                string[] resultado2 = resultado.Split('-');

                if(n == false)
                {
                    phpact = resultado2[0];
                    n = true;
                    break;
                }
                else
                {
                    if (phpact == resultado2[0])
                    {

                    }
                    else
                    {
                        phpact = resultado2[0];
                        update1();
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            function1();
        }
        public void update()
        {
            if (initalized == false)
            {
                MessageBox.Show("É preciso inicializar primeiro a aplicação!");
            }
            else
            {
                function1();
                client.SetPresence(new DiscordRPC.RichPresence()
                {
                    Details = $"{phpact}",
                    State = $"{textBox1.Text}",
                    Timestamps = Timestamps.Now,
                    Assets = new Assets()
                    {
                        LargeImageKey = $"{textBox4.Text}",
                        LargeImageText = "PHP",
                        SmallImageKey = $"{textBox5.Text}",
                        SmallImageText = "Visual Studio Code"
                    }
                });
            }
        }

        public void update1()
        {
            if (procPID == "0")
            {
                if (initalized == false)
                {
                    MessageBox.Show("É preciso inicializar primeiro a aplicação!");
                }
                else
                {
                    function1();
                    client.SetPresence(new DiscordRPC.RichPresence()
                    {
                        Details = $"N/A",
                        State = $"{textBox1.Text}",
                        Timestamps = Timestamps.Now,
                        Assets = new Assets()
                        {
                            LargeImageKey = $"{textBox4.Text}",
                            LargeImageText = "PHP",
                            SmallImageKey = $"{textBox5.Text}",
                            SmallImageText = "Visual Studio Code"
                        }
                    });
                }
            }
            else
            {
                if (initalized == false)
                {
                    MessageBox.Show("É preciso inicializar primeiro a aplicação!");
                }
                else
                {
                    function1();
                    client.SetPresence(new DiscordRPC.RichPresence()
                    {
                        Details = $"{phpact}",
                        State = $"{textBox1.Text}",
                        Timestamps = Timestamps.Now,
                        Assets = new Assets()
                        {
                            LargeImageKey = $"{textBox4.Text}",
                            LargeImageText = "PHP",
                            SmallImageKey = $"{textBox5.Text}",
                            SmallImageText = "Visual Studio Code"
                        }
                    });
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            function1();
        }
    }
}
