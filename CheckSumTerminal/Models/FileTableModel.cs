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
        public long Id { get; set; }
        [Description("Название файла")]
        public string Name { get; set; }
        [Description("Версия")]
        public int Version { get; set; }
        [Description("Дата изменения")]
        public string DateTime { get; set; }

        public FileTableModel Clone()
        {
            FileTableModel f = new()
            {
                Id = Id,
                Name = Name,
                Version = Version,
                DateTime = DateTime
            };

            return f;
        }
    }
}
