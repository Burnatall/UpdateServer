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
        public static int GetCountOfVersion()
        {
            return mainModel.GetAllVersions().Count;
        }

        public static Dictionary<string, DateTime> GetRange()
        {
            return mainModel.GetAllVersionWithDates();
        }

        public static string GetByNumber(string number)
        {
            if (number != mainModel.GetLastFullVersion())
                return File.ReadAllText(Environment.CurrentDirectory + @"\" + Properties.Resources.VersionFolderName + @"\" + number + @"\" + Properties.Resources.ChangesDocument);
            else
                return File.ReadAllText(Environment.CurrentDirectory + @"\" + Properties.Resources.ChangesDocument);
        }
    }
}
