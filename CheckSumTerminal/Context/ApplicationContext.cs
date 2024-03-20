
using CheckSumTerminal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CheckSumTerminal.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<FileTableModel> Files { get; set; }
        public DbSet<VersionModel> Versions { get; set; }
        public ApplicationContext()
        {
            //Database.EnsureCreated();
        }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
            // создаем базу данных при первом обращении
        }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                Properties.Resources.ConnectionStringTest);
        }

        public static DbContext GetNew()
        {
            return new ApplicationContext();
        }

    }
}
