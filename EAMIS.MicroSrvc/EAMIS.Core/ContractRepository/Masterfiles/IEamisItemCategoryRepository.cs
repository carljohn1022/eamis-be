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
    public interface IEamisItemCategoryRepository
    {
        //Task<DataList<EamisItemCategoryDTO>> MapToDTOList(string searchKey, PageConfig config);
        Task<DataList<EamisItemCategoryDTO>> SearchItemCategory(string type, string searchValue);
        Task<DataList<EamisItemCategoryDTO>> List(EamisItemCategoryDTO filter, PageConfig config);
        Task<EamisItemCategoryDTO> Insert(EamisItemCategoryDTO item);
        Task<EamisItemCategoryDTO> Update(EamisItemCategoryDTO item);
        Task<EamisItemCategoryDTO> Delete(EamisItemCategoryDTO item);
        Task<bool> ValidateExistingShortDesc(string shortDesc, string categoryName);
        Task<bool> EditValidateExistingShortDesc(int id, string shortDesc, string categoryName);
        Task<bool> EditValidateExistingShortDesc(int id, string shortDesc);
        Task<bool> EditValidateExistingCategory(int id, string categoryName);
        Task<List<EAMISITEMCATEGORY>> ListAllItemCategories();
        Task<List<EAMISITEMCATEGORY>> ListCategories(string searchValue);
        Task<EamisItemCategoryDTO> InsertFromExcel(EamisItemCategoryDTO item);
        Task<string> GetPropertyNo (int categoryId);
        Task<List<EAMISITEMCATEGORY>> GetAllItemCategories();

        Task<List<EAMISITEMCATEGORY>> ListCategoriesById(int categoryId);

        string ErrorMessage { get; set; }

        bool HasError { get; set; }
        Task<bool> InsertFromExcel(List<EamisItemCategoryDTO> Items);
    }
}
