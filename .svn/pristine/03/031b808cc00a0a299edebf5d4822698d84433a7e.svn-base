
using CheckSumTerminal.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace CheckSumServer.Functional
{
    public class comparison
    {
        /// <summary>
        /// Сравнивает лист полученный из request с листом из базы, порядок элементов также должен совпадать.
        /// </summary>
        /// <param name="fileTableModels">Лист полученный из request</param>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <returns>id несовпавших элементов</returns>
        public List<long> compare(List<FileTableModel> fileTableModels, IConfiguration configuration)
        {
            DbGetTable db = new DbGetTable(configuration);
            var fileTableModels2 = db.getTable(Properties.Resources.TableName);
            return badCompare(fileTableModels, fileTableModels2);
        }

        public List<long> deepListCompare(List<FileTableModel> fileTableModels, List<FileTableModel> fileTableModels2)
        {
            var l = new List<long>();

            //Если запрос будет передан в случайном порядке
            List<FileTableModel> table2 = fileTableModels2.OrderBy(x => x.id).ToList();
            List<FileTableModel> table1 = fileTableModels.OrderBy(x => x.id).ToList();

            //Начинаем с начала сортированного списка
            for (int i = table2.IndexOf(table2.Where(x=>x.id== table1.First().id).First()); i < fileTableModels.Count; i++)
            {
                var k = table1[i].version;
                if (table1[i].version != table2[i].version ||
                    table1[i].name != table2[i].name ||
                    table1[i].id != table2[i].id) 
                    l.Add(table1[i].id);
            }
            return l;
        }

        public List<long> badCompare(List<FileTableModel> fileTableModels, List<FileTableModel> fileTableModels2)
        {
            var l = new List<long>();
            foreach (var i in fileTableModels)
            {
                var g = fileTableModels2.Where(x => x.name == i.name).FirstOrDefault();
                if (g != null)
                {
                    if (g.version != i.version || g.id != i.id)
                        l.Add(i.id);
                }
                else
                    l.Add(i.id);
            }

            return l.Distinct().ToList();
        }
    }
}
