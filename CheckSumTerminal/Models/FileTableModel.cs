using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckSumTerminal.Models
{
    public class FileTableModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Description("Id")]
        public long id { get; set; }
        [Description("Название файла")]
        public string name { get; set; }
        [Description("Версия")]
        public int version { get; set; }
        [Description("Дата изменения")]
        public string date_time { get; set; }

        public FileTableModel clone()
        {
            FileTableModel f = new FileTableModel();
            f.id = id;
            f.name = name;
            f.version = version;
            f.date_time = date_time;

            return f;
        }
    }
}
