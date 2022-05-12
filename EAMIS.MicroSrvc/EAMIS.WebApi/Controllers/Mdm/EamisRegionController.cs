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
    public class EamisRegionController : ControllerBase
    {
        IEamisRegionRepository _eamisRegionRepository;
        public EamisRegionController(IEamisRegionRepository eamisRegionRepository)
        {
            _eamisRegionRepository = eamisRegionRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EAMISREGION>> List([FromQuery] EamisRegionDTO filter, [FromQuery]PageConfig config)
        {
            if (config == null)
                config = new PageConfig();
            if (filter == null)
                filter = new EamisRegionDTO();
            return Ok(await _eamisRegionRepository.List(filter,config));
        }
    }
}
