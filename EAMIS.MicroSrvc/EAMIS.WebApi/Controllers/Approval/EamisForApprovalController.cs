using EAMIS.Common.DTO.Approval;
using EAMIS.Core.ContractRepository.Approval;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Approval
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisForApprovalController : ControllerBase
    {
        IEamisForApprovalRepository _eamisForApprovalRepository;
        public EamisForApprovalController(IEamisForApprovalRepository eamisForApprovalRepository)
        {
            _eamisForApprovalRepository = eamisForApprovalRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EamisForApprovalDTO>> List([FromQuery] EamisForApprovalDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisForApprovalDTO();
            return Ok(await _eamisForApprovalRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisForApprovalDTO>> Add([FromBody] EamisForApprovalDTO item)
        {
            if (item == null)
                item = new EamisForApprovalDTO();

            var result = await _eamisForApprovalRepository.Insert(item);
            if (_eamisForApprovalRepository.HasError)
                return BadRequest(_eamisForApprovalRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisForApprovalDTO>> Edit([FromBody] EamisForApprovalDTO item)
        {
            if (item == null)
                item = new EamisForApprovalDTO();
            var result = await _eamisForApprovalRepository.Update(item);
            if (_eamisForApprovalRepository.HasError)
                return BadRequest(_eamisForApprovalRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisForApprovalDTO>> Delete([FromBody] EamisForApprovalDTO item)
        {
            if (item == null)
                item = new EamisForApprovalDTO();
            var result = await _eamisForApprovalRepository.Delete(item);
            if (_eamisForApprovalRepository.HasError)
                return BadRequest(_eamisForApprovalRepository.ErrorMessage);
            return Ok(result);
        }
    }
}
