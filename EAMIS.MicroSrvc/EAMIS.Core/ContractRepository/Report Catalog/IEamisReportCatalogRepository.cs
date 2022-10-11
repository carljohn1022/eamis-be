using EAMIS.Common.DTO.LookUp;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Report_Catalog
{
    public interface IEamisReportCatalogRepository
    {
        Task<List<LookupDTO>> FundSourceList();
        Task<List<LookupDTO>> TransactionNumberICS();
        Task<List<LookupDTO>> TransactionNumberPAR();
        Task<List<LookupDTO>> PropertyNumberList();
        Task<List<LookupDTO>> ItemCodeList();
        Task<List<LookupDTO>> TransactionNumberIssuance();
        Task<List<LookupDTO>> OfficeList();

    }
}
