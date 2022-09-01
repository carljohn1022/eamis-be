using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisProcurementCategoryRepository
    {
        Task<DataList<EamisProcurementCategoryDTO>> SearchProcurementCategory(string searchType, string searchValue);
        Task<DataList<EamisProcurementCategoryDTO>> List(EamisProcurementCategoryDTO filter, PageConfig config);
        Task<EamisProcurementCategoryDTO> Insert(EamisProcurementCategoryDTO item);
        Task<EamisProcurementCategoryDTO> Update(int Id, EamisProcurementCategoryDTO item);
        Task<EamisProcurementCategoryDTO> Delete(EamisProcurementCategoryDTO item);
        Task<bool> ValidateExistingDesc(string procurementDescription);
        Task<bool> ValidateExistingDescUpdate(int id, string procurementDescription);
        Task<EamisProcurementCategoryDTO> InsertFromExcel(EamisProcurementCategoryDTO item);
        Task<List<EAMISPROCUREMENTCATEGORY>> ListAllProcurements();

        Task<bool> InsertFromExcel(List<EamisProcurementCategoryDTO> item);
        string ErrorMessage { get; set; }

        bool HasError { get ; set; }

    }
}
