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
    public interface IEamisPropertyItemsRepository
    {
        Task<DataList<EamisPropertyItemsDTO>> PublicSearch(string type, string SearchValue);
        Task<DataList<EamisPropertyItemsDTO>> List(EamisPropertyItemsDTO filter, PageConfig config);
        Task<EamisPropertyItemsDTO> Insert(EamisPropertyItemsDTO item);
        Task<EamisPropertyItemsDTO> InsertFromExcel(EamisPropertyItemsDTO item);
        Task<bool> InsertFromExcel(List<EamisPropertyItemsDTO> Items);
        Task<List<EAMISPROPERTYITEMS>> GetAllPropertyItems();
        Task<EamisPropertyItemsDTO> Update(EamisPropertyItemsDTO item);
        Task<EamisPropertyItemsDTO> Delete(EamisPropertyItemsDTO item);
        Task<EamisPropertyItemsDTO> GeneratedProperty();
        Task<bool> ValidateExistingItem(string propertyNo);
        Task<bool> ValidateExistingPropertyName(string propertyName);
        Task<bool> UpdateValidateExistingItem(string propertyNo, int id);
        Task<bool> UpdateValidateExistingItemPropertyName(string propertyName, int id);
        string GetPropertyImageFileName(int propertyItemId);
        string ErrorMessage { get; set; }

        bool HasError { get; set; }
    }
}
