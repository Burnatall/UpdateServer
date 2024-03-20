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
        public long Id { get; set; }
        public string MainVersion { get; set; } 
        public string SubVersion { get; set; }
        public DateTime DateTime { get; set; }
        public string ParentBranch { get; set; }

        public VersionModel Clone()
        {
            VersionModel v = new()
            {
                Id = Id,
                MainVersion = MainVersion,
                SubVersion = SubVersion,
                DateTime = DateTime,
                ParentBranch = ParentBranch
            };
            return v;
        }
    }
}
