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

namespace EAMIS.WebApi.Controllers.Rolemanager
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisRolesController : ControllerBase
    {
        private readonly IEamisRolesRepository _eamisRolesRepository;
        public EamisRolesController(IEamisRolesRepository eamisRolesRepository)
        {
            _eamisRolesRepository = eamisRolesRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISROLES>> List([FromQuery] EamisRolesDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisRolesDTO();
            return Ok(await _eamisRolesRepository.List(filter,config));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisRolesDTO>> Add([FromBody] EamisRolesDTO item)
        {
            if (item == null)
                item = new EamisRolesDTO();
            return Ok(await _eamisRolesRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisRolesDTO>> Edit([FromBody] EamisRolesDTO item)
        {
            if (item == null)
                item = new EamisRolesDTO();
            return Ok(await _eamisRolesRepository.Update(item));
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisRolesDTO>> Delete([FromBody] EamisRolesDTO item)
        {
            if (item == null)
                item = new EamisRolesDTO();
            return Ok(await _eamisRolesRepository.Delete(item));
        }
    }
}
