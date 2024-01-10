﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CheckSumTerminal.Models
{
    public class VersionModelPretty
    {
        public long Id { get; set; }
        public string Версия { get; set; }
        public DateTime Дата { get; set; }

        public string Ответвление { get; set; }

        public VersionModelPretty(long id, string mainVersion, string subVersion,DateTime dateTime, string parent)
        {
            Id = id;
            Версия = mainVersion + "."+ subVersion;
            Дата = dateTime;
            Ответвление = parent;
        }
    }
}