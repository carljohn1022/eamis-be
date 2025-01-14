﻿using EAMIS.Common.DTO;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository
{
    public interface IEamisProvinceRepository
    {
        Task<DataList<EamisProvinceDTO>> List(EamisProvinceDTO filter,PageConfig config);
        Task<List<EAMISPROVINCE>> ListProvince(string searchValue);
        Task<List<EAMISPROVINCE>> ListProvinceById(int provinceId);
    }
}
