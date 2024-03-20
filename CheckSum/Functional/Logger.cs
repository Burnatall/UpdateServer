using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CheckSumServer.Functional
{
    public static class Logger
    {
        public async static void CreateLogWarning(string message)
        {
            try
            {
                await CreateLogInformation(message, "Warning:");
            }
            catch { }
        }

        public static Task CreateLogInformation(string message, string header) 
        {
            using (StreamWriter sw = new(new FileStream(Environment.CurrentDirectory + @"\" + Properties.Resources.LogConnectionFile, FileMode.Append)))
            {
                sw.WriteLine("Warning:");
                sw.WriteLine(message);
                sw.Close();
            }
            return Task.CompletedTask;
        }

        public async static void CreateLogError(string message)
        {
            try
            {
                await CreateLogInformation(message, "Error:");
            }
            catch { }
        }
    }
}
