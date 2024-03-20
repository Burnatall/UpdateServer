using CheckSumServer.Functional;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace CheckSumServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetDescriptionController : Controller
    {

        private readonly IConfiguration _configuration;

        public GetDescriptionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public string GetDesription(string number)
        {
            var d = Description.GetByNumber(number);

            var ip = ControllerContext.HttpContext.Connection.RemoteIpAddress.ToString();
            Logger.CreateLogWarning("Remote IP: " + ip + " Connected " + DateTime.Now.ToString() + Environment.NewLine
                + "Action: " + ToString() + Environment.NewLine + "Answer: " + d + Environment.NewLine);

            return d;
        }

        public override string ToString()
        {
            return "Получение описания выбранной версии";
        }
    }
}
