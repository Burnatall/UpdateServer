﻿
using CheckSumTerminal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CheckSumTerminal.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<FileTableModel> files { get; set; }
        public DbSet<VersionModel> versions { get; set; }
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

        public DbContext GetNew()
        {
            return new ApplicationContext();
        }

    }
}