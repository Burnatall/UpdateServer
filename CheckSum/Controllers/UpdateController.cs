﻿using CheckSumServer.Functional;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Windows.Shapes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace CheckSumServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UpdateController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// возвращает актуальные пакеты
        /// </summary>
        /// <param name="id">Номер возвращаемого пакета</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUpdate(long id)
        {

            DbGetTable db = new(_configuration);

            var filesPath = db.GetPathOfNeededFile(id);
            try
            {
                var tst = System.IO.Path.GetFullPath(Properties.Resources.FolderName);
                Response.Headers.Add("Content-Disposition", "attachment");
                Response.ContentType = System.IO.Path.GetFileNameWithoutExtension(filesPath) + "/dll";
                if (filesPath == "No such number of file")
                {
                    Response.ContentType = System.IO.Path.GetFileNameWithoutExtension(filesPath);
                    return new NotFoundResult();
                }

                var ip = ControllerContext.HttpContext.Connection.RemoteIpAddress.ToString();
                Logger.CreateLogWarning("Remote IP: " + ip + " Connected " + DateTime.Now.ToString() + Environment.NewLine
                    + "Action: " + ToString() +Environment.NewLine+"Answer: "+ filesPath + Environment.NewLine);

                return PhysicalFile(tst + @"\" + filesPath, "file/dll", filesPath);

            }
            catch 
            {
                return new BadRequestResult();
            }
        }

        public override string ToString()
        {
            return "Получение файла по id";
        }
    }
}
