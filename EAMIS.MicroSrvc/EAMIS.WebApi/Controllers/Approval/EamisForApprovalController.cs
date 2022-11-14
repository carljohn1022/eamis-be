using EAMIS.Common.DTO.Approval;
using EAMIS.Core.CommonSvc.Constant;
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

        /// <summary>
        /// call this method/endpoint to list all created approvals for all users/approver
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<ActionResult<EamisForApprovalDTO>> List([FromQuery] EamisForApprovalDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisForApprovalDTO();
            return Ok(await _eamisForApprovalRepository.List(filter, config));
        }

        /// <summary>
        /// call this method/endpoint to create/set approvals for specific user/approver
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<ActionResult<EamisForApprovalDTO>> Add([FromBody] EamisForApprovalDTO item, decimal totalAmount)
        {
            if (item == null || totalAmount <= 0)
                return BadRequest("Required parameter is missing.");

            var result = await _eamisForApprovalRepository.Insert(item, totalAmount);
            if (_eamisForApprovalRepository.HasError)
                return BadRequest(_eamisForApprovalRepository.ErrorMessage);
            return Ok(result);
        }

        /// <summary>
        /// call this method/endpoint to modify/update created approval for specific user/approver
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="config"></param>
        /// <returns></returns>
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

        /// <summary>
        /// call this method/endpoint to submit/save approver's approval details
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("SubmitApproval")]
        public async Task<ActionResult<MyApprovalListDTO>> SubmitApproval([FromBody] MyApprovalListDTO item)
        {
            if (item == null)
                return BadRequest("Required parameter is missing.");

            var result = await _eamisForApprovalRepository.SubmitApproval(item);
            if (_eamisForApprovalRepository.HasError)
                return BadRequest(_eamisForApprovalRepository.ErrorMessage);
            return Ok(result);
        }


        /// <summary>
        /// call this method/endpoint to retrieve all approvals based on the userId provided, 
        /// refer to EAMIS.Core.CommonSvc.Constant.DocStatus for the list of transaction types
        /// if transaction type is not provided by the calling third party program, the default value is DocStatus.AllStatus
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        [HttpGet("GetMyApprovalList")]
        public async Task<ActionResult<MyApprovalListDTO>> GetMyApprovalList(int userId, string transactionType = DocStatus.AllStatus)
        {
            if (userId == 0)
                return BadRequest("Required parameter is missing");

            var result = await _eamisForApprovalRepository.MyApprovalList(userId, transactionType);

            if (_eamisForApprovalRepository.HasError)
                return BadRequest(_eamisForApprovalRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpGet("getForApprovalStatus")]
        public async Task<ActionResult<EamisForApprovalDTO>> getForApprovalStatus(string transactionNumber)
        {
            return Ok(await _eamisForApprovalRepository.getForApprovalStatus(transactionNumber));
        }
    }
}
