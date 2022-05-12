using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain.Entities;
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
    public class EamisUserRolesController : ControllerBase
    {
        private readonly IEamisUserRolesRepository _eamisUserRolesRepository;
        public EamisUserRolesController(IEamisUserRolesRepository eamisUserRolesRepository)
        {
            _eamisUserRolesRepository = eamisUserRolesRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISUSERROLES>> List([FromQuery] EamisUserRolesDTO filter)
        {
            if (filter == null)
                filter = new EamisUserRolesDTO();
            return Ok(await _eamisUserRolesRepository.List(filter));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisUserRolesDTO>> Add([FromBody] EamisUserRolesDTO item)
        {
            if (item == null)
                item = new EamisUserRolesDTO();
            return Ok(await _eamisUserRolesRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisUserRolesDTO>> Edit([FromBody] EamisUserRolesDTO item)
        {
            if (item == null)
                item = new EamisUserRolesDTO();
            return Ok(await _eamisUserRolesRepository.Update(item));
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisUserRolesDTO>> Delete([FromBody] EamisUserRolesDTO item)
        {
            if (item == null)
                item = new EamisUserRolesDTO();
            return Ok(await _eamisUserRolesRepository.Delete(item));
        }
    }
}
