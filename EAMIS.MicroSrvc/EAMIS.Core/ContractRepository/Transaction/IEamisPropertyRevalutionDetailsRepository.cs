using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisPropertyRevalutionDetailsRepository
    {
        string ErrorMessage { get; set; }
        bool HasError { get; set; }
        Task<DataList<EamisPropertyRevaluationDetailsDTO>> List(EamisPropertyRevaluationDetailsDTO filter, PageConfig config);
        EamisPropertyRevaluationDetailsDTO CalculateRevaluationDetails(EamisPropertyRevaluationDetailsDTO item, DateTime? newDepreciationDate);
        Task<EamisPropertyRevaluationDetailsDTO> Insert(EamisPropertyRevaluationDetailsDTO item);
        Task<EamisPropertyRevaluationDetailsDTO> Update(EamisPropertyRevaluationDetailsDTO item);
    }
}
