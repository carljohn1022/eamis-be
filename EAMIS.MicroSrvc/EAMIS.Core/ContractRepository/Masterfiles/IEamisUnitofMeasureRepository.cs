using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisUnitofMeasureRepository
    {
        Task<DataList<EamisUnitofMeasureDTO>> List(EamisUnitofMeasureDTO filter, PageConfig config);
        Task<EamisUnitofMeasureDTO> Insert(EamisUnitofMeasureDTO item);
        Task<EamisUnitofMeasureDTO> Update(EamisUnitofMeasureDTO item);
        Task<EamisUnitofMeasureDTO> Delete(EamisUnitofMeasureDTO item);
        Task<DataList<EamisUnitofMeasureDTO>> SearchMeasure(string type, string searchValue);
        //Task<bool> ValidateExistingDescUpdate(string ShortDesc, string UomDesc);
        //Task<bool> ValidateExistingUomDesc(string UomDesc);
        //Task<bool> ValidateExistingShortDesc(string ShortDesc);
        //Task<bool> ValidateExistingShortDesc1(string ShortDesc);
        //Task<bool> ValidateExistingUomDesc1(string UomDesc);
        //Task<bool> Validation(string shortDesc, string UomDesc);
        Task<bool> ValidationForUomExistShortDescNotExist(string ShortDesc, string UomDesc);
        Task<bool> ValidationForShortDescExistUomNotExist(string ShortDesc, string UomDesc);
        Task<bool> ValidateExistDesc(string ShortDesc, string UomDesc);
        Task<EamisUnitofMeasureDTO> InsertFromExcel(EamisUnitofMeasureDTO item);
        Task<List<EAMISUNITOFMEASURE>> ListAllIUnitOfMeasurement();
    }
}
