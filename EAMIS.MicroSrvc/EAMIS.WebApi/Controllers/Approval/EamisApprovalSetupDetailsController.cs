using EAMIS.Common.DTO.Approval;
using EAMIS.Core.ContractRepository.Approval;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Approval
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisApprovalSetupDetailsController : ControllerBase
    {
        IEamisApprovalSetupDetailsRepository _eamisApporvalSetupDetailsRepository;
        public EamisApprovalSetupDetailsController(IEamisApprovalSetupDetailsRepository eamisApporvalSetupDetailsRepository)
        {
            _eamisApporvalSetupDetailsRepository = eamisApporvalSetupDetailsRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EamisApprovalSetupDetailsDTO>> List([FromQuery] EamisApprovalSetupDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisApprovalSetupDetailsDTO();
            return Ok(await _eamisApporvalSetupDetailsRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisApprovalSetupDetailsDTO>> Add([FromBody] EamisApprovalSetupDetailsDTO item)
        {
            if (item == null)
                item = new EamisApprovalSetupDetailsDTO();

            var result = await _eamisApporvalSetupDetailsRepository.Insert(item);
            if (_eamisApporvalSetupDetailsRepository.HasError)
                return BadRequest(_eamisApporvalSetupDetailsRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisApprovalSetupDetailsDTO>> Edit([FromBody] EamisApprovalSetupDetailsDTO item)
        {
            if (item == null)
                item = new EamisApprovalSetupDetailsDTO();
            var result = await _eamisApporvalSetupDetailsRepository.Update(item);
            if (_eamisApporvalSetupDetailsRepository.HasError)
                return BadRequest(_eamisApporvalSetupDetailsRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisApprovalSetupDetailsDTO>> Delete([FromBody] EamisApprovalSetupDetailsDTO item)
        {
            if (item == null)
                item = new EamisApprovalSetupDetailsDTO();
            var result = await _eamisApporvalSetupDetailsRepository.Delete(item);
            if (_eamisApporvalSetupDetailsRepository.HasError)
                return BadRequest(_eamisApporvalSetupDetailsRepository.ErrorMessage);
            return Ok(result);
        }
    }
}
