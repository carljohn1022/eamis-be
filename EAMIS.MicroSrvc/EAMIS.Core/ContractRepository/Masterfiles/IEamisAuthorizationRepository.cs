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
    public interface IEamisAuthorizationRepository
    {
        Task<DataList<EamisAuthorizationDTO>> List(EamisAuthorizationDTO filter, PageConfig config);
        Task<List<EAMISAUTHORIZATION>> ListAuthorization(string searchValue);
        Task<List<EAMISAUTHORIZATION>> ListAuthorizationById(int authorizationId);
    }
}
