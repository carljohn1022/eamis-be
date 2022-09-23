using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Transaction;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Transaction
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisDeliveryReceiptDetailsController : ControllerBase
    {
        IEamisDeliveryReceiptDetailsRepository _eamisDeliveryReceiptDetailsRepository;
        public EamisDeliveryReceiptDetailsController(IEamisDeliveryReceiptDetailsRepository eamisDeliveryReceiptDetailsRepository)
        {
            _eamisDeliveryReceiptDetailsRepository = eamisDeliveryReceiptDetailsRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EamisDeliveryReceiptDetailsDTO>> List([FromQuery] EamisDeliveryReceiptDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisDeliveryReceiptDetailsDTO();
            return Ok(await _eamisDeliveryReceiptDetailsRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisDeliveryReceiptDetailsDTO>> Add([FromBody] EamisDeliveryReceiptDetailsDTO item)
        {
            if (item == null)
                item = new EamisDeliveryReceiptDetailsDTO();
            return Ok(await _eamisDeliveryReceiptDetailsRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisDeliveryReceiptDetailsDTO>> Edit([FromBody] EamisDeliveryReceiptDetailsDTO item)
        {
            if (item == null)
                item = new EamisDeliveryReceiptDetailsDTO();
            return Ok(await _eamisDeliveryReceiptDetailsRepository.Update(item));
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisDeliveryReceiptDetailsDTO>> Delete([FromBody] EamisDeliveryReceiptDetailsDTO item)
        {
            if (item == null)
                item = new EamisDeliveryReceiptDetailsDTO();
            return Ok(await _eamisDeliveryReceiptDetailsRepository.Delete(item));
        }
        [HttpPost("PostSerialTranByRecord")]
        public async Task<ActionResult<EamisSerialTranDTO>> PostSerialTran([FromBody] EamisSerialTranDTO item)
        {
            if (item == null)
                return BadRequest("At least one serial row is required");

            var result = await _eamisDeliveryReceiptDetailsRepository.PostSerialTranByItem(item);

            if (_eamisDeliveryReceiptDetailsRepository.HasError)
                return BadRequest(_eamisDeliveryReceiptDetailsRepository.ErrorMessage);

            return Ok(result);
        }

    }
}
