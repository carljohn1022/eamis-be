using EAMIS.Common.DTO;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository
{
    public interface IEamisBarangayReporsitory
    {
        Task<DataList<EamisBarangayDTO>> List(EamisBarangayDTO filter,PageConfig config);
        Task<DataList<EamisBarangayDTO>> PublicSearch(string SearchType, string SearchValue, PageConfig config);
    }
}
