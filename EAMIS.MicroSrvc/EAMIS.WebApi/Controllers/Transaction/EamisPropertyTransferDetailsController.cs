using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Transaction;
using EAMIS.Core.Domain.Entities;
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
    public class EamisPropertyTransferDetailsController : ControllerBase
    {
        IEamisPropertyTransferDetailsRepository _eamisPropertyTransferDetailsRepository;
        public EamisPropertyTransferDetailsController(IEamisPropertyTransferDetailsRepository eamisPropertyTransferDetailsRepository)
        {
            _eamisPropertyTransferDetailsRepository = eamisPropertyTransferDetailsRepository;
        }

        [HttpGet("GetPropertyTransferDetailsById")]
        public async Task<ActionResult<EamisPropertyTransferDetailsDTO>> GetPropertyTransferDetailsById([FromQuery] EamisPropertyTransferDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyTransferDetailsDTO();
            return Ok(await _eamisPropertyTransferDetailsRepository.List(filter, config));
        }


        /// <summary>
        /// this will create a new record/row in [EAMIS_PROPERTY_TRANSACTION_DETAILS]
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("AddPropertyTransferDetails")]
        public async Task<ActionResult<EamisPropertyTransferDetailsDTO>> AddPropertyTransferDetails([FromBody] EamisPropertyTransferDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransferDetailsDTO();
            return Ok(await _eamisPropertyTransferDetailsRepository.Insert(item));
        }
        //[HttpGet("listitemsfortransfer")]
        //public async Task<ActionResult<EamisPropertyTransferDetailsDTO>> List([FromBody] EamisPropertyTransferDetailsDTO item, [FromQuery] PageConfig config)
        //{
        //    if (item == null)
        //        item = new EamisPropertyTransferDetailsDTO();
        //    return Ok(await _eamisPropertyTransferDetailsRepository.ListItemsForTranser(item, config));
        //}
        [HttpGet("listitemsfortransfer")]
        public async Task<ActionResult<EamisPropertyTransferDetailsDTO>> List([FromQuery] EamisPropertyTransferDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyTransferDetailsDTO();
            return Ok(await _eamisPropertyTransferDetailsRepository.ListItemsForTranser(filter, config));
        }

        [HttpGet("getResponsibilityCenter")]
        public async Task<string> GetResponsibilityCenterByID(string newResponsibilityCode)
        {
            var response = await _eamisPropertyTransferDetailsRepository.GetResponsibilityCenterByID(newResponsibilityCode);
            return response;
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisPropertyTransferDetailsDTO>> Edit([FromBody] EamisPropertyTransferDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransferDetailsDTO();
            return Ok(await _eamisPropertyTransferDetailsRepository.Update(item));
        }
        [HttpGet("SearchIssuance")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTIONDETAILS>> SearchIssuance(string type, string searchValue, int assigneeCustodian)
        {
            return Ok(await _eamisPropertyTransferDetailsRepository.SearchIssuance(type, searchValue, assigneeCustodian));
        }
    }
}