using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Report_Catalog;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Report_Catalog
{
    public interface IEamisReportLinkRepository
    {
        string ErrorMessage { get; set; }
        bool HasError { get; set; }

        Task<EamisUserReportLinkDTO> Delete(EamisUserReportLinkDTO item);
        Task<EamisUserReportLinkDTO> Insert(EamisUserReportLinkDTO item);
        Task<EamisUsersDTO> GetUserIdList(int userId);
        Task<DataList<EamisUserReportLinkDTO>> List(EamisUserReportLinkDTO filter, PageConfig config);
        IQueryable<EAMISUSERREPORTLINK> PagedQuery(IQueryable<EAMISUSERREPORTLINK> query, int resolved_size, int resolved_index);
        Task<EamisUserReportLinkDTO> Update(EamisUserReportLinkDTO item);
    }
}
