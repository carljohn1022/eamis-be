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
    public class EamisFundSourceController : ControllerBase
    {
        IEamisFundSourceRepository _eamisFundSourceRepository;
        public EamisFundSourceController(IEamisFundSourceRepository eamisFundSourceRepository)
        {
            _eamisFundSourceRepository = eamisFundSourceRepository;
        }

        [HttpGet("Search")]
        public async Task<ActionResult<EAMISFUNDSOURCE>> Search(string type, string searchValue)
        {
            return Ok(await _eamisFundSourceRepository.SearchFunds(type, searchValue));
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISFUNDSOURCE>> List([FromQuery] EamisFundSourceDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisFundSourceDTO();
            return Ok(await _eamisFundSourceRepository.List(filter, config));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisFundSourceDTO>> Add([FromBody] EamisFundSourceDTO item)
        {
            if (await _eamisFundSourceRepository.ValidateExistingCode(item.Code)) return Unauthorized();
            if (item == null)
                item = new EamisFundSourceDTO();
            return Ok(await _eamisFundSourceRepository.Insert(item));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisFundSourceDTO>> Edit([FromBody] EamisFundSourceDTO item,int id)
        {
            var data = new EamisFundSourceDTO();
            if (await _eamisFundSourceRepository.UpdateValidateExistingCode(item.Code, item.Id))
            {
                if (item == null)
                    item = new EamisFundSourceDTO();
                return Ok(await _eamisFundSourceRepository.Update(item, id));
            }
            else if(await _eamisFundSourceRepository.ValidateExistingCode(item.Code))
            {
                return Unauthorized();
            }
            else
            {
                return Ok(await _eamisFundSourceRepository.Update(item, id));
            }
            
            
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisFundSourceDTO>> Delete([FromBody] EamisFundSourceDTO item,int Id)
        {
            if (item == null)
                item = new EamisFundSourceDTO();
            return Ok(await _eamisFundSourceRepository.Delete(item,Id));
        }
    }
}
