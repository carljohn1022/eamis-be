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
    public class EamisProvinceController : ControllerBase
    {
        private IEamisProvinceRepository _eamisProvinceRepository;
        public EamisProvinceController(IEamisProvinceRepository eamisProvinceRepository)
        {
            _eamisProvinceRepository = eamisProvinceRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EAMISPROVINCE>> List([FromQuery] EamisProvinceDTO filter, [FromQuery]PageConfig config)
        {
            if (config == null)
                config = new PageConfig();
            if (filter == null)
                filter = new EamisProvinceDTO();
            return Ok(await _eamisProvinceRepository.List(filter,config));
        }
    }
}
