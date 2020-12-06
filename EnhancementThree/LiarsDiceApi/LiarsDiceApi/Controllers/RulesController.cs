using LiarsDiceApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiarsDiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulesController : ControllerBase
    {
        private LiarsDiceDbContext _ldContext;
        public RulesController(LiarsDiceDbContext ldContext)
        {
            _ldContext = ldContext;
        }
        
        // GET: api/<RulesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Rules rules = new Rules();
            return new string[] { rules.RulesText };
        }
    }
}
