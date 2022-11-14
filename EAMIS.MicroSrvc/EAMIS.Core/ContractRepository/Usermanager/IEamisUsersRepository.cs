using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisUsersRepository
    {
        Task<EAMISUSERS> Register(RegisterDTO item);
        Task<bool> UserExists(string Username,string EmployeeAgencyNumber);
        Task<EAMISUSERS> GetUserName(string Username);
        Task<DataList<EamisUsersDTO>> PublicSearch(string searchType, string searchKey, PageConfig config);
        Task<DataList<EamisUsersDTO>> List(EamisUsersDTO filter, PageConfig config);
        Task<bool> Validate(string EmployeeAgencyNumber);
        Task<bool> ValidateExistAgency(string EmployeeAgencyNumber);
        Task<string> GetAgencyName(string AgencyEmployeeNumber);
        //Task<EamisUsersDTO> ChangePassword(EamisUsersDTO item);
        Task<string> GetId(string AgencyEmployeeNumber);
        Task<EamisUsersDTO> ChangePassword(EamisUsersDTO item, string newPassword);


    }
}
