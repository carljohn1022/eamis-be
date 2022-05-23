using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.LogicRepository.Masterfiles.EAMIS.Core.LogicRepository.Masterfiles;
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
    public class EAMISResponsibilityCenterController : ControllerBase
    {
        IEamisResponsibilityCenterRepository _eamisResponsibilityCenterRepository;
        public EAMISResponsibilityCenterController(IEamisResponsibilityCenterRepository eamisResponsibilityCenterRepository)
        {
            _eamisResponsibilityCenterRepository = eamisResponsibilityCenterRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EAMISRESPONSIBILITYCENTER>> List([FromQuery] EamisResponsibilityCenterDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisResponsibilityCenterDTO();
            return Ok(await _eamisResponsibilityCenterRepository.List(filter, config));
        }
        [HttpGet("Search")]
        public async Task<ActionResult<EAMISRESPONSIBILITYCENTER>> Search(string type, string searchValue)
        {
            return Ok(await _eamisResponsibilityCenterRepository.SearchResCenter(type, searchValue));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisResponsibilityCenterDTO>> Add([FromBody] EamisResponsibilityCenterDTO item)
        {
            item.responsibilityCenter = item.mainGroupCode + item.subGroupCode + item.officeCode + item.unitCode;
            if (await _eamisResponsibilityCenterRepository.ValidateExistingCode(item.responsibilityCenter))
            {
                return Unauthorized();
            }
            if (item == null)
                item = new EamisResponsibilityCenterDTO();
            return Ok(await _eamisResponsibilityCenterRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisResponsibilityCenterDTO>> Edit([FromBody] EamisResponsibilityCenterDTO item, int id)
        {
            item.responsibilityCenter = item.mainGroupCode + item.subGroupCode + item.officeCode + item.unitCode;
            var data = new EamisResponsibilityCenterDTO();
            if (await _eamisResponsibilityCenterRepository.ValidateExistingCode(item.responsibilityCenter))
            {
                return Unauthorized();
            }
            if (item == null)
                item = new EamisResponsibilityCenterDTO();
            return Ok(await _eamisResponsibilityCenterRepository.Update(item,id));
        }


    }
}

