﻿using CheckSumServer.Functional;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace CheckSumServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetPicturesZip : Controller
    {
        private readonly IConfiguration _configuration;

        public GetPicturesZip(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Возвращает рахив с картинками - заставками из текущей дирректории
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetArchive()
        {
            try
            {
                var tst = Environment.CurrentDirectory +@"\"+Properties.Resources.PicturesZipName;

                var ip = ControllerContext.HttpContext.Connection.RemoteIpAddress.ToString();
                Logger.createLogWarning("Remote IP: " + ip + " Connected " + DateTime.Now.ToString() + Environment.NewLine
                    + "Action: " + ToString() + Environment.NewLine);

                return PhysicalFile(tst, "archive/zip", Properties.Resources.PicturesZipName);
            }
            catch
            {
                return new BadRequestResult();
            }
        }

        public override string ToString()
        {
            return "Получение архива с картинками";
        }
    }
}