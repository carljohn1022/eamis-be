using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Masterfiles
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisFinancingSourceController : ControllerBase
    {
        IEamisFinancingSourceRepository _eamisFinancingSourceRepository;
        public EamisFinancingSourceController(IEamisFinancingSourceRepository eamisFinancingSourceRepository)
        {
            _eamisFinancingSourceRepository = eamisFinancingSourceRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EamisFinancingSourceDTO>> List([FromQuery] EamisFinancingSourceDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisFinancingSourceDTO();
            return Ok(await _eamisFinancingSourceRepository.List(filter, config));
        }
    }
}
