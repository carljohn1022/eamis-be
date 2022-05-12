using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisGeneralFundSourceRepository
    {
        Task<DataList<EamisGeneralFundSourceDTO>> List(EamisGeneralFundSourceDTO filter, PageConfig config);
        Task<EamisGeneralFundSourceDTO> Insert(EamisGeneralFundSourceDTO item);
        Task<EamisGeneralFundSourceDTO> Update(EamisGeneralFundSourceDTO item,int Id);
        Task<EamisGeneralFundSourceDTO> Delete(EamisGeneralFundSourceDTO item,int Id);
    }
}
