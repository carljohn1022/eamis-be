using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisGeneralChartofAccountsRepository
    {
        Task<DataList<EamisGeneralChartofAccountsDTO>> List(EamisGeneralChartofAccountsDTO filter, PageConfig config);
        Task<EamisGeneralChartofAccountsDTO> Insert(EamisGeneralChartofAccountsDTO item);
        Task<EamisGeneralChartofAccountsDTO> Update(EamisGeneralChartofAccountsDTO item, int Id);
        Task<EamisGeneralChartofAccountsDTO> Delete(EamisGeneralChartofAccountsDTO item, int Id);
    }
}
