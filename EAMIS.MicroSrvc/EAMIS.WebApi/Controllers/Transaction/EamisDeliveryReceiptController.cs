using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Transaction;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Transaction
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisDeliveryReceiptController : ControllerBase
    {
        IEamisDeliveryReceiptRepository _eamisDeliveryReceiptRepository;
        public EamisDeliveryReceiptController(IEamisDeliveryReceiptRepository eamisDeliveryReceiptRepository)
        {
            _eamisDeliveryReceiptRepository = eamisDeliveryReceiptRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EAMISDELIVERYRECEIPT>> List([FromQuery] EamisDeliveryReceiptDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisDeliveryReceiptDTO();
            return Ok(await _eamisDeliveryReceiptRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisDeliveryReceiptDTO>> Add([FromBody] EamisDeliveryReceiptDTO item)
        {
            if (item == null)
                item = new EamisDeliveryReceiptDTO();
            return Ok(await _eamisDeliveryReceiptRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisDeliveryReceiptDTO>> Edit([FromBody] EamisDeliveryReceiptDTO item)
        {
            if (item == null)
                item = new EamisDeliveryReceiptDTO();
            return Ok(await _eamisDeliveryReceiptRepository.Update(item));
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisDeliveryReceiptDTO>> Delete([FromBody] EamisDeliveryReceiptDTO item)
        {
            if (item == null)
                item = new EamisDeliveryReceiptDTO();
            return Ok(await _eamisDeliveryReceiptRepository.Delete(item));
        }
        [HttpGet("getNextSequence")]
        public async Task<string> GetNextSequenceAsync()
        {
            var nextId = await _eamisDeliveryReceiptRepository.GetNextSequenceNumber();
            return nextId;
        }

        [HttpGet("Search")]
        public async Task<ActionResult<EAMISRESPONSIBILITYCENTER>> Search(string type, string searchValue)
        {
            return Ok(await _eamisDeliveryReceiptRepository.SearchDeliveryReceipt(type, searchValue));
        }

    }
}
