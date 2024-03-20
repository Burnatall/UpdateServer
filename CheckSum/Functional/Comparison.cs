
using CheckSumTerminal.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace CheckSumServer.Functional
{
    public class Comparison
    {
        /// <summary>
        /// Сравнивает лист полученный из request с листом из базы, порядок элементов также должен совпадать.
        /// </summary>
        /// <param name="fileTableModels">Лист полученный из request</param>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <returns>id несовпавших элементов</returns>
        public List<long> Compare(List<FileTableModel> fileTableModels, IConfiguration configuration)
        {
            DbGetTable db = new(configuration);
            var fileTableModels2 = db.GetTable(Properties.Resources.TableName);
            return BadCompare(fileTableModels, fileTableModels2);
        }

        public List<long> DeepListCompare(List<FileTableModel> fileTableModels, List<FileTableModel> fileTableModels2)
        {
            var l = new List<long>();

            //Если запрос будет передан в случайном порядке
            List<FileTableModel> table2 = fileTableModels2.OrderBy(x => x.Id).ToList();
            List<FileTableModel> table1 = fileTableModels.OrderBy(x => x.Id).ToList();

            //Начинаем с начала сортированного списка
            for (int i = table2.IndexOf(table2.Where(x=>x.Id== table1.First().Id).First()); i < fileTableModels.Count; i++)
            {
                var k = table1[i].Version;
                if (table1[i].Version != table2[i].Version ||
                    table1[i].Name != table2[i].Name ||
                    table1[i].Id != table2[i].Id) 
                    l.Add(table1[i].Id);
            }
            return l;
        }

        public List<long> BadCompare(List<FileTableModel> fileTableModels, List<FileTableModel> fileTableModels2)
        {
            var l = new List<long>();
            foreach (var i in fileTableModels)
            {
                var g = fileTableModels2.Where(x => x.Name == i.Name).FirstOrDefault();
                if (g != null)
                {
                    if (g.Version != i.Version || g.Id != i.Id)
                        l.Add(i.Id);
                }
                else
                    l.Add(i.Id);
            }

            return l.Distinct().ToList();
        }
    }
}
