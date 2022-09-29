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
    public class EamisServiceLogDetailsController : ControllerBase
    {
        private readonly IEamisServiceLogDetailsRepository _eamisServiceLogDetailsRepository;
        public EamisServiceLogDetailsController(IEamisServiceLogDetailsRepository eamisServiceLogDetailsRepository)
        {
            _eamisServiceLogDetailsRepository = eamisServiceLogDetailsRepository;
        }

        /// <summary>
        /// this will returns all the list of all property items with service log
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<ActionResult<EAMISSERVICELOGDETAILS>> List([FromQuery] EamisServiceLogDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisServiceLogDetailsDTO();
            return Ok(await _eamisServiceLogDetailsRepository.ListServiceLogDetails(filter, config));
        }

       

        /// <summary>
        /// this method will returns the list of all property items for service log creation, only transaction_type wit value of Property Receiving
        /// and no service log created yet
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpGet("listservicelogforcreation")]
        public async Task<ActionResult<EamisServiceLogDetailsCreationDTO>> ListServiceLogDetailsForCreation([FromQuery] EamisPropertyTransactionDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyTransactionDetailsDTO();
            var result = await _eamisServiceLogDetailsRepository.ListServiceLogDetailsForCreation(filter, config);
            return Ok(result);
        }

        /// <summary>
        /// important: before calling this method, make sure that the service log details is already created as
        /// we need the service log header id to be saved in the service log details table to be able to associate
        /// the service log header and details
        /// </summary>
        /// <param name="eamisServiceLogDetailsDTO"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<ActionResult<EAMISSERVICELOGDETAILS>> AddServiceLogDetails([FromBody] EamisServiceLogDetailsDTO eamisServiceLogDetailsDTO)
        {
            var result = await _eamisServiceLogDetailsRepository.InsertServiceLogDetails(eamisServiceLogDetailsDTO);
            if (result == null)
                return BadRequest();
            if (_eamisServiceLogDetailsRepository.HasError)
                return BadRequest(_eamisServiceLogDetailsRepository.ErrorMessage);

            return Ok(result); //return the item with ID, needed in constructing the payload of the Service log details
        }

        [HttpPost("update")]
        public async Task<ActionResult<EAMISSERVICELOGDETAILS>> UpdateServiceLogDetails([FromBody] EamisServiceLogDetailsDTO eamisServiceLogDetailsDTO)
        {
            var result = await _eamisServiceLogDetailsRepository.UpdateServiceLogDetails(eamisServiceLogDetailsDTO);
            if (result == null || result.ID == 0)
                return BadRequest();

            if (_eamisServiceLogDetailsRepository.HasError)
                return BadRequest(_eamisServiceLogDetailsRepository.ErrorMessage);

            return Ok(result);
        }

        [HttpGet("getAssetConditionType")]
        public async Task<ActionResult<EamisDeliveryReceiptDetailsDTO>> ListAssetCondition()
        {

            return Ok(await _eamisServiceLogDetailsRepository.GetAssetCondition());
        }

        [HttpGet("getTransactionType")]
        public async Task<ActionResult<EamisDeliveryReceiptDetailsDTO>> ListTransactionType()
        {
            return Ok(await _eamisServiceLogDetailsRepository.GetTranTypeList());
        }
    }
}
