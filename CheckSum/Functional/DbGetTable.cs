
using CheckSumTerminal.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Documents;

namespace CheckSumServer.Functional
{
    public class DbGetTable
    {
        private readonly IConfiguration _configuration;

        private string sqlConnection;
        private NpgsqlDataReader reader;
        private DataTable table = new();

        public DbGetTable(IConfiguration configuration)
        {
            _configuration = configuration;
            sqlConnection = _configuration.GetConnectionString("ITSGISAppCon");
        }

        /// <summary>
        /// Получает таблицу файлов и версий из базы
        /// </summary>
        /// <returns></returns>
        public List<FileTableModel> GetTable(string tableName)
        {
            List<FileTableModel> ans = new();
            //DataTable table = new DataTable();
            //string sqlConnection = _configuration.GetConnectionString("ITSGISAppCon");
            //NpgsqlDataReader reader;
            string query = @"Select * From "+tableName;
            using (NpgsqlConnection connection = new(sqlConnection))
            {
                connection.Open();
                using NpgsqlCommand command = new(query, connection);
                reader = command.ExecuteReader();
                table.Load(reader);

                List<DataRow> ldr = table.AsEnumerable().ToList();

                reader.Close();
                connection.Close();
            }
            for(int i = 0; i < table.Rows.Count; i++)
            {
                ans.Add(new FileTableModel()
                {
                    Id = (long)table.Rows[i]["id"],
                    Name = (string)table.Rows[i]["name"],
                    Version = (int)table.Rows[i]["version"],
                    DateTime = (string)table.Rows[i]["dateTime"]
                });
            }

            return ans;

        }

        public string GetPathOfNeededFile(long id)
        {
            string query = @"Select name From " + Properties.Resources.TableName + " Where id = "+ id;
            string ans = "";
            using (NpgsqlConnection connection = new(sqlConnection))
            {
                connection.Open();
                using NpgsqlCommand command = new(query, connection);
                reader = command.ExecuteReader();
                table.Load(reader);
                try
                {
                    ans = (string)table.AsEnumerable().ToList()[0].ItemArray[0];
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return "No such number of file";
                }

                reader.Close();
                connection.Close();
            }

            return ans;
        }


    }
}
