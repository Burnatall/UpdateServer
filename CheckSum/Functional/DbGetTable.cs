﻿
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
        private DataTable table = new DataTable();

        public DbGetTable(IConfiguration configuration)
        {
            _configuration = configuration;
            sqlConnection = _configuration.GetConnectionString("ITSGISAppCon");
        }

        /// <summary>
        /// Получает таблицу файлов и версий из базы
        /// </summary>
        /// <returns></returns>
        public List<FileTableModel> getTable(string tableName)
        {
            List<FileTableModel> ans = new List<FileTableModel>();
            //DataTable table = new DataTable();
            //string sqlConnection = _configuration.GetConnectionString("ITSGISAppCon");
            //NpgsqlDataReader reader;
            string query = @"Select * From "+tableName;
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlConnection))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    reader = command.ExecuteReader();
                    table.Load(reader);

                    List<DataRow> ldr = table.AsEnumerable().ToList();

                    reader.Close();
                    connection.Close();
                }
            }
            for(int i = 0; i < table.Rows.Count; i++)
            {
                ans.Add(new FileTableModel()
                {
                    id = (long)table.Rows[i]["id"],
                    name = (string)table.Rows[i]["name"],
                    version = (int)table.Rows[i]["version"],
                    date_time = (string)table.Rows[i]["date_time"]
                });
            }

            return ans;

        }

        public string getPathOfNeededFile(long id)
        {
            string query = @"Select name From " + Properties.Resources.TableName + " Where id = "+ id;
            string ans = "";
            using (NpgsqlConnection connection = new NpgsqlConnection(sqlConnection))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
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
            }

            return ans;
        }


    }
}
