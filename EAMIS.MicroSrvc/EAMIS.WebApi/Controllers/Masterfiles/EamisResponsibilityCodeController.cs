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
    public class EamisResponsibilityCodeController : ControllerBase
    {
        IEamisResponsibilityCodeRepository _eamisResponsibilityCodeRepository;
        public EamisResponsibilityCodeController(IEamisResponsibilityCodeRepository eamisResponsibilityCodeRepository)
        {
            _eamisResponsibilityCodeRepository = eamisResponsibilityCodeRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EAMISRESPONSIBILITYCODE>> List([FromQuery] EamisResponsibilityCodeDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisResponsibilityCodeDTO();
            return Ok(await _eamisResponsibilityCodeRepository.List(filter, config));
        }
        [HttpGet("Search")]
        public async Task<ActionResult<EAMISRESPONSIBILITYCODE>> Search(string type, string searchValue)
        {
            return Ok(await _eamisResponsibilityCodeRepository.SearchResCenter(type, searchValue));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisResponsibilityCodeDTO>> Add([FromBody] EamisResponsibilityCodeDTO item)
        {
            if (await _eamisResponsibilityCodeRepository.ValidateExistingCode(item.Office, item.Department))
            {
                return Unauthorized();
            }
            if (item == null)
                item = new EamisResponsibilityCodeDTO();
            return Ok(await _eamisResponsibilityCodeRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisResponsibilityCodeDTO>> Edit([FromBody] EamisResponsibilityCodeDTO item, int id)
        {
            var data = new EamisResponsibilityCodeDTO();
            if (await _eamisResponsibilityCodeRepository.UpdateValidateExistingCode(item.Office, item.Department, item.Id))
            {
                if (item == null)
                    item = new EamisResponsibilityCodeDTO();
                return Ok(await _eamisResponsibilityCodeRepository.Update(item, id));
            }
            else if (await _eamisResponsibilityCodeRepository.ValidateExistingCode(item.Office, item.Department))
            {
                return Unauthorized();
            }
            else
            {
                return Ok(await _eamisResponsibilityCodeRepository.Update(item, id));
            }
        }


    }
}
