using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CheckSumServer.Functional
{
    public static class Logger
    {
        public async static void createLogWarning(string message)
        {
            try
            {
                await createLogInformation(message, "Warning:");
            }
            catch { }
        }

        public static Task createLogInformation(string message, string header) 
        {
            using (StreamWriter sw = new StreamWriter(new FileStream(Environment.CurrentDirectory + @"\" + Properties.Resources.LogConnectionFile, FileMode.Append)))
            {
                sw.WriteLine("Warning:");
                sw.WriteLine(message);
                sw.Close();
            }
            return Task.CompletedTask;
        }

        public async static void createLogError(string message)
        {
            try
            {
                await createLogInformation(message, "Error:");
            }
            catch { }
        }
    }
}
