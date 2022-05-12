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
    public class EamisPropertySuppliesController : ControllerBase
    {
        IEamisPropertySuppliesRepository _eamisPropertySuppliesRepository;
        public EamisPropertySuppliesController(IEamisPropertySuppliesRepository eamisPropertySuppliesRepository)
        {
            _eamisPropertySuppliesRepository = eamisPropertySuppliesRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EamisPropertySuppliesDTO>> List([FromQuery] EamisPropertySuppliesDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertySuppliesDTO();
            return Ok(await _eamisPropertySuppliesRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertySuppliesDTO>> Add([FromBody] EamisPropertySuppliesDTO item)
        {
            if (item == null)
                item = new EamisPropertySuppliesDTO();
            return Ok(await _eamisPropertySuppliesRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisPropertySuppliesDTO>> Edit([FromBody] EamisPropertySuppliesDTO item)
        {
            if (item == null)
                item = new EamisPropertySuppliesDTO();
            return Ok(await _eamisPropertySuppliesRepository.Update(item));
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisPropertySuppliesDTO>> Delete([FromBody] EamisPropertySuppliesDTO item)
        {
            if (item == null)
                item = new EamisPropertySuppliesDTO();
            return Ok(await _eamisPropertySuppliesRepository.Delete(item));
        }
    }
}
