using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain.Entities;
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
    public class EamisGeneralFundSourceController : ControllerBase
    {
        private IEamisGeneralFundSourceRepository _eamisGeneralFundSource;
        public EamisGeneralFundSourceController(IEamisGeneralFundSourceRepository eamisGeneralFundSource)
        {
            _eamisGeneralFundSource = eamisGeneralFundSource;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISGENERALFUNDSOURCE>> List([FromQuery] EamisGeneralFundSourceDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisGeneralFundSourceDTO();
            return Ok(await _eamisGeneralFundSource.List(filter, config));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisEnvironmentalAspectsDTO>> Add([FromBody] EamisGeneralFundSourceDTO item)
        {
            if (item == null)
                item = new EamisGeneralFundSourceDTO();
            return Ok(await _eamisGeneralFundSource.Insert(item));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisGeneralFundSourceDTO>> Edit([FromBody] EamisGeneralFundSourceDTO item,int Id)
        {
            if (item == null)
                item = new EamisGeneralFundSourceDTO();
            return Ok(await _eamisGeneralFundSource.Update(item,Id));
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisGeneralFundSourceDTO>> Delete([FromBody] EamisGeneralFundSourceDTO item,int Id)
        {
            if (item == null)
                item = new EamisGeneralFundSourceDTO();
            return Ok(await _eamisGeneralFundSource.Delete(item,Id));
        }
    }
}
