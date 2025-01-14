﻿using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisSupplierRepository
    {
        Task<DataList<EamisSupplierDTO>> SearchSuppliers(string type, string searchValue);
        Task<DataList<EamisSupplierDTO>> List(EamisSupplierDTO filter, PageConfig config);
        Task<EamisSupplierDTO> Insert(EamisSupplierDTO item);
        Task<List<EAMISSUPPLIER>> GetAllSuppliers();
        Task<EamisSupplierDTO> InsertFromExcel(EamisSupplierDTO item);
        Task<bool> InsertFromExcel(List<EamisSupplierDTO> Items);
        Task<EamisSupplierDTO> Update(EamisSupplierDTO item, int id);
        Task<EamisSupplierDTO> Delete(EamisSupplierDTO item);
        Task<bool> ValidateExistingCode(string companynanme);
        Task<bool> UpdateValidationCode(int id, string companyname);
        Task<List<EAMISSUPPLIER>> ListAllSuppliers();
        Task<string> GetSupplieryById(int supplierId);
        string ErrorMessage { get; set; }

        bool HasError { get; set; }
    }
}
