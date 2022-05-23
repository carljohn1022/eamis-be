using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Masterfiles
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisSupplierController : ControllerBase
    {
        IEamisSupplierRepository _eamisSupplierRepository;
        public EamisSupplierController(IEamisSupplierRepository eamisSupplierRepository)
        {
            _eamisSupplierRepository = eamisSupplierRepository;
        }

        [HttpGet("SearchSupplier")]
        public async Task<ActionResult<EAMISSUPPLIER>> SearchSupplier(string type, string searchValue)
        {
            return Ok(await _eamisSupplierRepository.SearchSuppliers(type, searchValue));
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISSUPPLIER>> List([FromQuery] EamisSupplierDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisSupplierDTO();
            return Ok(await _eamisSupplierRepository.List(filter, config));

        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisSupplierDTO>> Add([FromBody] EamisSupplierDTO item)
        {
            if (await _eamisSupplierRepository.ValidateExistingCode(item.CompanyName))
            {
                return Unauthorized();
            }
            if (item == null)
                item = new EamisSupplierDTO();
            return Ok(await _eamisSupplierRepository.Insert(item));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisSupplierDTO>> Edit([FromBody] EamisSupplierDTO item, int id)
        {
            var data = new EamisSupplierDTO();
            if (await _eamisSupplierRepository.UpdateValidationCode(item.Id, item.CompanyName))
            {
                if (item == null)
                    item = new EamisSupplierDTO();
                return Ok(await _eamisSupplierRepository.Update(item, id));
            }
            else if (await _eamisSupplierRepository.ValidateExistingCode(item.CompanyName))
            {
                return Unauthorized();
            }
            else
            {
                return Ok(await _eamisSupplierRepository.Update(item, id));
            }
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisSupplierDTO>> Delete([FromBody] EamisSupplierDTO item)
        {
            if (item == null)
                item = new EamisSupplierDTO();
            return Ok(await _eamisSupplierRepository.Delete(item));
        }
    }
}
