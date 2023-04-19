using EAMIS.Common.DTO.Inventory_Taking;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Inventory_Taking;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Inventory_Taking
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisInventoryTakingController : ControllerBase
    {
        IEamisInventoryTakingRepository _eamisInventoryTakingRepository;
        public EamisInventoryTakingController(IEamisInventoryTakingRepository eamisInventoryTakingRepository)
        {
            _eamisInventoryTakingRepository = eamisInventoryTakingRepository;
        }
        [HttpGet("getScannedItemList")]
        public async Task<ActionResult<EamisInventoryTakingDTO>> getScannedPropertyNumberList(string propertyNumber, string office, string department, string userStamp)
        {
            //if (await _eamisInventoryTakingRepository.CheckPropertyNumberExistIfScanned(propertyNumber))
            //{
            //    return Unauthorized("Item is already Scanned");
            //}
            if (await _eamisInventoryTakingRepository.CheckPropertyNumberExist(propertyNumber))
            {
                return Ok(await _eamisInventoryTakingRepository.getScannedPropertyNumberList(propertyNumber, office, department, userStamp));
            }
            else {
                return Unauthorized("Property Number Doesn't Exist");
            }
        }
        [HttpGet("list")]
        public async Task<ActionResult<EAMISINVENTORYTAKING>> List([FromQuery] EamisInventoryTakingDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisInventoryTakingDTO();
            return Ok(await _eamisInventoryTakingRepository.List(filter, config));
        }
        [HttpGet("getUnitDesc")]
        public async Task<ActionResult<EamisResponsibilityCenterDTO>> GetUnitDesc(bool isActive,string officeDesc)
        {
            return Ok(await _eamisInventoryTakingRepository.GetUnitDesc(isActive, officeDesc));
        }
        [HttpGet("getOfficeDesc")]
        public async Task<ActionResult<EamisResponsibilityCenterDTO>> GetOfficeDesc(bool isActive)
        {
            return Ok(await _eamisInventoryTakingRepository.GetOfficeDesc(isActive));
        }
        [HttpGet("getPropertyAccountability")]
        public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> GetPropertyNumberAccountability(string assigneeCustodianName)
        {
            return Ok(await _eamisInventoryTakingRepository.GetPropertyNumberAccountability(assigneeCustodianName));
        }
    }
}
