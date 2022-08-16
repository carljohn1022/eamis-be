using EAMIS.Common.DTO;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository
{
    public interface IEamisMunicipalityRepository
    {
        Task<DataList<EamisMunicipalityDTO>> List(EamisMunicipalityDTO filter, PageConfig config);
        Task<List<EAMISMUNICIPALITY>> ListMunicipality(string searchValue);
        Task<List<EAMISMUNICIPALITY>> ListMunicipalityById(int municipalityId);
    }
}
