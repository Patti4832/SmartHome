using System;

using HA_Settings;
using HA_IO;
using HA_Updater;

namespace Hausautomation
{
    class Program
    {
        public static int version = 1;

        static Updater class_Updater = new Updater();
        static IO class_IO = new IO();
        static Settings class_Settings = new Settings();
        static Server class_Server;

        static void Main(string[] args)
        {
            class_IO.Log("Starting ...");
            class_IO.Log("Updating ...");
            GetVersions();
            class_Updater.CheckForUpdate();
            if (class_Updater.updateAvailable[0] || class_Updater.updateAvailable[1] || class_Updater.updateAvailable[2] || class_Updater.updateAvailable[3])
            {
                class_Updater.InstallUpdate();
            }
            else
            {
                class_Settings.Init();
                class_IO.Init();
                StartCompleted();
            }

            Console.ReadLine();
        }

        static void GetVersions()
        {
            class_Updater.versions[0] = version;
            class_Updater.versions[1] = class_Updater.version;
            class_Updater.versions[2] = class_IO.version;
            class_Updater.versions[3] = class_Settings.version;
        }

        static void StartCompleted()
        {
            class_IO.DebugLog("Versions:\r\n\t"
                + "[Main] V" + class_Updater.versions[0] + "\r\n\t"
                + "[Updater] V" + class_Updater.versions[1] + "\r\n\t"
                + "[IO] V" + class_Updater.versions[2] + "\r\n\t"
                + "[Settings] V" + class_Updater.versions[3]);
            class_IO.Log("Libraries updated and loaded.");
            class_IO.Log("Starting Server (Port " + class_Settings.serverPort + ")...");
            class_Server = new Server(class_Settings.serverPort, class_Settings.token);
            class_Server.Loop();
        }
    }
}
