using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HA_Updater
{
    public class Updater
    {
        public int version = 1;
        public bool[] updateAvailable = new bool[4] { false, false, false, false };
        public int[] versions = new int[4];
        readonly string updateURL = "https://raw.githubusercontent.com/Patti4832/SmartHome/master/Update/";

        WebClient client = new WebClient();

        public void CheckForUpdate()
        {

            string updateFile = client.DownloadString(updateURL + "versions.txt");

            int[] fileVersions = new int[4];

            foreach (string s in updateFile.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (s.StartsWith("Main="))
                    fileVersions[0] = int.Parse(s.Replace("Main=", ""));
                else if (s.StartsWith("Updater="))
                    fileVersions[1] = int.Parse(s.Replace("Updater=", ""));
                else if (s.StartsWith("Settings="))
                    fileVersions[2] = int.Parse(s.Replace("Settings=", ""));
                else if (s.StartsWith("IO="))
                    fileVersions[3] = int.Parse(s.Replace("IO=", ""));
            }

            for (int i = 0; i < 4; i++)
            {
                if (fileVersions[i] > versions[i])
                    updateAvailable[i] = true;
            }
        }

        public void InstallUpdate()
        {
            if (!Directory.Exists("Update"))
                Directory.CreateDirectory("Update");

            if (updateAvailable[0]) //SmartHome.exe
            {
                client.DownloadFile(updateURL + "SmartHome.exe", "Update/" + "SmartHome.exe");
            }
            if (updateAvailable[1]) //Updater.dll
            {
                client.DownloadFile(updateURL + "HA_Updater.dll", "Update/" + "HA_Updater.dll");
            }
            if (updateAvailable[2]) //Settings.dll
            {
                client.DownloadFile(updateURL + "HA_Settings.dll", "Update/" + "HA_Settings.dll");
            }
            if (updateAvailable[3]) //IO.dll
            {
                client.DownloadFile(updateURL + "HA_IO.dll", "Update/" + "HA_IO.dll");
            }

            ExecuteScript(CreateScript());
        }

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        private string CreateScript()
        {
            string updateFile = "update";
            if (!IsLinux)
                updateFile += ".bat";

            client.DownloadFile(updateURL + updateFile, "Update/" + updateFile);
            while (!File.Exists("Update/" + updateFile)) ;
            return System.Environment.CurrentDirectory + "/Update/" + updateFile;
        }

        private void ExecuteScript(string script)
        {
            while (!File.Exists(script)) ;
            Task t = new Task(delegate { System.Diagnostics.Process.Start(script); });
            t.Start();
        }
    }
}
