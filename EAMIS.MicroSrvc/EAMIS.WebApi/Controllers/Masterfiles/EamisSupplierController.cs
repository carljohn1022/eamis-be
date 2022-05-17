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
        public async Task<ActionResult<EAMISSUPPLIER>> SearchSupplier(string searchValue)
        {
            return Ok(await _eamisSupplierRepository.SearchSupplier(searchValue));
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
            else
            { 
            }
            if (item == null)
                item = new EamisSupplierDTO();
            return Ok(await _eamisSupplierRepository.Insert(item));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisSupplierDTO>> Edit([FromBody] EamisSupplierDTO item)
        {
            if (item == null)
                item = new EamisSupplierDTO();
            return Ok(await _eamisSupplierRepository.Update(item));
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
