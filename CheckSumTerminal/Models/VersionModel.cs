using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CheckSumTerminal.Models
{
    public class VersionModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string main_version { get; set; } 
        public string sub_version { get; set; }
        public DateTime date_time { get; set; }
        public string parent_branch { get; set; }

        public VersionModel clone()
        {
            VersionModel v = new VersionModel();
            v.id = id;
            v.main_version = main_version;
            v.sub_version = sub_version;
            v.date_time = date_time;
            v.parent_branch = parent_branch;
            return v;
        }
    }
}
