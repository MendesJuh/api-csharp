using DevIO.Api.Controllers;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.V2.Controllers
{
    
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteController : MainController

    {
        private readonly ILogger _logger;
        public TesteController(INotificador notificador, IUser appUser, ILogger logger) : base(notificador, appUser)
        {
            _logger = logger;
        }
        [HttpGet]
        public string Valor()
        {
            return "Sou a V2";
        }
    }
}
