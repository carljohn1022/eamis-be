using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Transaction;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Hosting;
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
    public class EamisPropertyIssuanceController : ControllerBase
    {
        IEamisPropertyIssuanceRepository _eamisPropertyIssuanceRepository;
        IEamisPropertyTransactionRepository _eamisPropertyTransactionRepository;
        public EamisPropertyIssuanceController(IEamisPropertyIssuanceRepository eamisPropertyIssuanceRepository, IEamisPropertyTransactionRepository eamisPropertyTransactionRepository)
        {
            _eamisPropertyIssuanceRepository = eamisPropertyIssuanceRepository;
            _eamisPropertyTransactionRepository = eamisPropertyTransactionRepository;
        }

        [HttpGet("getNextSequence")]
        public async Task<string> GetNextSequenceAsync()
        {
            var nextId = await _eamisPropertyIssuanceRepository.GetNextSequenceNumber();
            return nextId;
        }
        [HttpGet("Search")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTION>> Search(string type, string searchValue)
        {
            return Ok(await _eamisPropertyTransactionRepository.SearchReceivingforIssuance(type, searchValue));
        }
        //[HttpGet("list")]
        //public async Task<ActionResult<EAMISPROPERTYDETAILS>> List([FromQuery] EamisPropertyItemsDTO filter, [FromQuery] PageConfig config)
        //{
        //    if (filter == null)
        //        filter = new EamisPropertyItemsDTO();
        //    return Ok(await _eamisPropertyIssuanceRepository.List(filter, config));
        //}

        //[HttpGet("list")]
        //public async Task<ActionResult<EAMISPROPERTYTRANSACTIONDETAILS>> List([FromQuery] EamisPropertyTransactionDetailsDTO filter, [FromQuery] PageConfig config)
        //{
        //    if (filter == null)
        //        filter = new EamisPropertyTransactionDetailsDTO();
        //    return Ok(await _eamisPropertyIssuanceRepository.List(filter, config));
        //}


        [HttpPost("AddPropertyTransaction")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTION>> AddPropertyTransaction([FromBody] EamisPropertyTransactionDTO eamisPropertyTransactionDTO)
        {
            //Steps
            //1. Create Property Transaction
            var result = await _eamisPropertyIssuanceRepository.InsertProperty(eamisPropertyTransactionDTO);
            if (result == null)
                return BadRequest();
            //2. Create Property Transaction Details
            //3. Update Property Items In Stock Quantity
            return Ok(result); //return the item with ID, needed in constructing the payload of the Property Transaction details
        }

        [HttpPost("UpdatePropertyTransaction")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> Edit([FromBody] EamisPropertyTransactionDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyIssuanceRepository.UpdateProperty(item));
        }

        [HttpPut("UpdatePropertyTransactionDetails")]
        public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> Edit([FromBody] EamisPropertyTransactionDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDetailsDTO();
            return Ok(await _eamisPropertyIssuanceRepository.UpdateDetails(item));
        }
        //[HttpGet("editbyid")]
        //public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> getPropertyItemById(int itemID)
        //{
        //    return Ok(await _eamisPropertyIssuanceRepository.getPropertyItemById(itemID));
        //}
        [HttpGet("editbyid")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> getPropertyItemById(int itemID)
        {
            return Ok(await _eamisPropertyIssuanceRepository.getPropertyItemById(itemID));
        }
        [HttpPost("AddPropertyTransactionDetails")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTION>> AddPropertyTransactionDetails([FromBody] EamisPropertyTransactionDetailsDTO eamisPropertyTransactionDetailsDTO)
        {
            //Steps
            //2. Create Property Transaction Details
            //Note: make sure that Property Transaction ID generated from  AddPropertyTransaction method is assigned to  PropertyTransactionDetails.PropertyTransactionID
            //and other required fields are properly filled-up before calling this method
            var result = await _eamisPropertyIssuanceRepository.InsertPropertyTransaction(eamisPropertyTransactionDetailsDTO);
            if (result == null)
                return BadRequest();
            //3. Update Property Items In Stock Quantity
            return Ok(result);
        }

        [HttpPost("UpdateItemQtyInStock")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTION>> AddPropertyTransactionDetails([FromBody] EamisDeliveryReceiptDetailsDTO eamisDeliveryReceiptDetailsDTO)
        {
            //Steps
            //2. Create Property Transaction Details
            //3. Update Property Items In Stock Quantity
            var result = await _eamisPropertyIssuanceRepository.UpdatePropertyItemQty(eamisDeliveryReceiptDetailsDTO);
            if (result == null)
                return BadRequest();

            return Ok();//use "return Ok(result) if the result is needed to return.
        }

        //[HttpGet("listForReceivingItems")]
        //public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> ListItemsForReceivingItems()
        //{
        //    return Ok(await _eamisPropertyIssuanceRepository.ListItemsForReceivingItems());
        //}
        [HttpGet("listitemsforreceiving")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTIONDETAILS>> ListItemsForReceiving([FromQuery] EamisPropertyTransactionDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyTransactionDetailsDTO();
            return Ok(await _eamisPropertyIssuanceRepository.ListItemsForReceiving(filter, config));
        }
        [HttpGet("SearchReceiving")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTIONDETAILS>> SearchReceiving(string type, string searchValue)
        {
            return Ok(await _eamisPropertyIssuanceRepository.SearchReceiving(type, searchValue));
        }
        //[HttpDelete("Delete")]
        //public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> Delete([FromBody] EamisPropertyTransactionDetailsDTO item)
        //{
        //    if (item == null)
        //        item = new EamisPropertyTransactionDetailsDTO();
        //    return Ok(await _eamisPropertyIssuanceRepository.Delete(item));
        //}
    }
}
