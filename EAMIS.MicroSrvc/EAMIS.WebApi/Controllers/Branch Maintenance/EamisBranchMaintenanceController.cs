using EAMIS.Common.DTO.Branch_Maintenance;
using EAMIS.Core.ContractRepository.Branch_Maintenance;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Branch_Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisBranchMaintenanceController : ControllerBase
    {
        IEamisBranchMaintenanceRepository _eamisBranchMaintenanceRepository;
        public EamisBranchMaintenanceController(IEamisBranchMaintenanceRepository eamisBranchMaintenanceRepository)
        {
            _eamisBranchMaintenanceRepository = eamisBranchMaintenanceRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EAMISBRANCHMAINTENANCE>> List([FromQuery] EamisBranchDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisBranchDTO();
            return Ok(await _eamisBranchMaintenanceRepository.List(filter, config));

        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisBranchDTO>> Add([FromBody] EamisBranchDTO item)
        {
            if (await _eamisBranchMaintenanceRepository.ValidateExistingBranchID(item.BranchID)) return Unauthorized();
            if (item == null)
                item = new EamisBranchDTO();
            return Ok(await _eamisBranchMaintenanceRepository.Insert(item));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisBranchDTO>> Edit([FromQuery] int seqID, [FromBody] EamisBranchDTO item)
        {
            if (await _eamisBranchMaintenanceRepository.ValidateExistingBranchUpdate(item.SeqID, item.BranchID))
            {
                if (item == null)
                    item = new EamisBranchDTO();
                return Ok(await _eamisBranchMaintenanceRepository.Update(seqID, item));
            }
            else if (await _eamisBranchMaintenanceRepository.ValidateExistingBranchID(item.BranchID))
            {
                return Unauthorized();
            }
            else
            {
                return Ok(await _eamisBranchMaintenanceRepository.Update(seqID, item));
            }
           
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisBranchDTO>> Delete([FromBody] EamisBranchDTO item)
        {
            if (item == null)
                item = new EamisBranchDTO();
            return Ok(await _eamisBranchMaintenanceRepository.Delete(item));
        }
    }
}
