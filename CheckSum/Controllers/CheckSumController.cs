using CheckSumServer.Functional;
using CheckSumTerminal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CheckSumServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckSumController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CheckSumController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Возвращает список id несовпавших элементов, сравнение по version,id,name и порядку элементов
        /// </summary>
        /// <param name="version">Версия клиента</param>
        /// <param name="sum">Полученная от клиента сумма пакетов</param>
        /// <returns></returns>
        [HttpPost]
        public List<long> GetSum()
        {
            var cc = ControllerContext.HttpContext;
            StreamReader s = new StreamReader(cc.Request.Body);
            List<FileTableModel> files = JsonConvert.DeserializeObject<List<FileTableModel>>(s.ReadToEndAsync().Result);

            comparison c = new comparison();
            var ans = c.compare(files, _configuration);

            var ip = cc.Connection.RemoteIpAddress.ToString();
            Logger.createLogWarning("Remote IP: " + ip + " Connected " + DateTime.Now.ToString() + Environment.NewLine 
                +"Action: "+ ToString() + Environment.NewLine + "Answer: " + Parser<long,bool>.getListForMessage(ans)+ Environment.NewLine);

            return ans;
        }

        public override string ToString()
        {
            return "Получение списка несовпавших элементов";
        }
    }
}
