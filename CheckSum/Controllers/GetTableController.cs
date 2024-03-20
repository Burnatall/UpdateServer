using CheckSumServer.Functional;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace CheckSumServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetTableController : Controller
    {
        private readonly IConfiguration _configuration;

        public GetTableController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetTable()
        {
            try
            {
                var tst = System.IO.Path.GetFullPath(Properties.Resources.CSVTableName);

                var ip = ControllerContext.HttpContext.Connection.RemoteIpAddress.ToString();
                Logger.CreateLogWarning("Remote IP: " + ip + " Connected " + DateTime.Now.ToString() + Environment.NewLine
                    + "Action: " + ToString() + Environment.NewLine);

                return PhysicalFile(tst, "table/csv", Properties.Resources.CSVTableName);
            }
            catch 
            {
                return new BadRequestResult();
            }
        }

        public override string ToString()
        {
            return "Получение таблицы файлов";
        }
    }
}
