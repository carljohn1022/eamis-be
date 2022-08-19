using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Transaction;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Transaction
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisPropertyTransactionController : ControllerBase
    {
        IEamisPropertyTransactionRepository _eamisPropertyTransactionRepository;
        IEamisDeliveryReceiptDetailsRepository _eamisDeliveryReceiptDetailsRepository;
        public EamisPropertyTransactionController(IEamisPropertyTransactionRepository eamisPropertyTransactionRepository, IEamisDeliveryReceiptDetailsRepository eamisDeliveryReceiptDetailsRepository)
        {
            _eamisPropertyTransactionRepository = eamisPropertyTransactionRepository;
            _eamisDeliveryReceiptDetailsRepository = eamisDeliveryReceiptDetailsRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> List([FromQuery] EamisPropertyTransactionDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyTransactionRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> Add([FromBody] EamisPropertyTransactionDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyTransactionRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> Edit([FromBody] EamisPropertyTransactionDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyTransactionRepository.Update(item));
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> Delete([FromBody] EamisPropertyTransactionDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyTransactionRepository.Delete(item));
        }
        [HttpGet("getNextSequence")]
        public async Task<string> GetNextSequenceAsync()
        {
            var nextId = await _eamisPropertyTransactionRepository.GetNextSequenceNumberPR();
            return nextId;
        }
        [HttpGet("editbyid")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> getPropertyItemById(int itemID)
        {
            return Ok(await _eamisPropertyTransactionRepository.getPropertyItemById(itemID));
        }
        [HttpGet("Search")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTION>> Search(string type, string searchValue)
        {
            return Ok(await _eamisPropertyTransactionRepository.SearchReceivingforList(type, searchValue));
        }
        [HttpGet("SearchDeliveryReceiptDetails")]
        public async Task<ActionResult<EAMISDELIVERYRECEIPTDETAILS>> SearchDR(string type, string searchValue)
        {
            return Ok(await _eamisDeliveryReceiptDetailsRepository.SearchDeliveryDetailsforReceiving(type, searchValue));
        }
    }
}
