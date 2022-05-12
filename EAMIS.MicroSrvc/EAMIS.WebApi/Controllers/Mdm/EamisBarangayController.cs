using EAMIS.Common.DTO;
using EAMIS.Core.ContractRepository;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisBarangayController : ControllerBase
    {
        IEamisBarangayReporsitory _eamisBarangayReporsitory;
        public EamisBarangayController(IEamisBarangayReporsitory eamisBarangayReporsitory)
        {
            _eamisBarangayReporsitory = eamisBarangayReporsitory;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EAMISBARANGAY>> List([FromQuery] EamisBarangayDTO filter,[FromQuery]PageConfig config)
        {
            if (config == null)
                 config = new PageConfig();
            if (filter == null)
                filter = new EamisBarangayDTO();
            return Ok(await _eamisBarangayReporsitory.List(filter,config));
        }
        [HttpGet("PublicSearchBarangay")]
        public async Task<ActionResult<EAMISBARANGAY>> PublicSearchBarangay(string SearchType, string SearchValue,[FromQuery] PageConfig config)
        {
            if (config == null)
                config = new PageConfig();
            return Ok(await _eamisBarangayReporsitory.PublicSearch(SearchType,SearchValue, config));
        }
    }
}
