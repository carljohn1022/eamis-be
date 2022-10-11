﻿using EAMIS.Common.DTO.Rolemanager;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisRoleModuleLinkRepository
    {
        Task<DataList<EamisRoleModuleLinkDTO>> List(EamisRoleModuleLinkDTO filter, PageConfig config);
        Task<EamisRoleModuleLinkDTO> Insert(EamisRoleModuleLinkDTO item);
        Task<EamisRoleModuleLinkDTO> Update(EamisRoleModuleLinkDTO item);
        Task<EamisRoleModuleLinkDTO> Delete(EamisRoleModuleLinkDTO item);
        string ErrorMessage { get; set; }
        public bool HasError { get; set; }
    }
}