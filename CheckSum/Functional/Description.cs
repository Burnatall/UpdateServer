using CheckSumTerminal.IModels;
using CheckSumTerminal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CheckSumServer.Functional
{
    public static class Description
    {
        private static IMainModel mainModel = new MainModel();
        public static int getCountOfVersion()
        {
            return mainModel.GetAllVersions().Count;
        }

        public static Dictionary<string, DateTime> getRange()
        {
            return mainModel.GetAllVersionWithDates();
        }

        public static string getByNumber(string number)
        {
            if (number != mainModel.GetLastFullVersion())
                return File.ReadAllText(Environment.CurrentDirectory + @"\" + Properties.Resources.VersionFolderName + @"\" + number + @"\" + Properties.Resources.ChangesDocument);
            else
                return File.ReadAllText(Environment.CurrentDirectory + @"\" + Properties.Resources.ChangesDocument);
        }
    }
}
