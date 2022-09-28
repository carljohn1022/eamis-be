using EAMIS.Common.DTO.Approval;
using EAMIS.Core.ContractRepository.Approval;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Approval
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisApprovalSetupController : ControllerBase
    {
        IEamisApprovalSetupRepository _eamisApporvalSetupRepository;
        public EamisApprovalSetupController(IEamisApprovalSetupRepository eamisApporvalSetupRepository)
        {
            _eamisApporvalSetupRepository = eamisApporvalSetupRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EamisApprovalSetupDTO>> List([FromQuery] EamisApprovalSetupDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisApprovalSetupDTO();
            return Ok(await _eamisApporvalSetupRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisApprovalSetupDTO>> Add([FromBody] EamisApprovalSetupDTO item)
        {
            if (item == null)
                item = new EamisApprovalSetupDTO();

            var result = await _eamisApporvalSetupRepository.Insert(item);
            if (_eamisApporvalSetupRepository.HasError)
                return BadRequest(_eamisApporvalSetupRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisApprovalSetupDTO>> Edit([FromBody] EamisApprovalSetupDTO item)
        {
            if (item == null)
                item = new EamisApprovalSetupDTO();
            var result = await _eamisApporvalSetupRepository.Update(item);
            if (_eamisApporvalSetupRepository.HasError)
                return BadRequest(_eamisApporvalSetupRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisApprovalSetupDTO>> Delete([FromBody] EamisApprovalSetupDTO item)
        {
            if (item == null)
                item = new EamisApprovalSetupDTO();
            var result = await _eamisApporvalSetupRepository.Delete(item);
            if (_eamisApporvalSetupRepository.HasError)
                return BadRequest(_eamisApporvalSetupRepository.ErrorMessage);
            return Ok(result);
        }
    }
}
