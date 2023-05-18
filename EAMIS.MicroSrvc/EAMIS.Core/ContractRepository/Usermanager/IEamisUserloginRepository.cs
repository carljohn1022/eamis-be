using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EAMIS.Common.DTO.Masterfiles.EamisUserloginDTO;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisUserloginRepository
    {
        Task<LoginDTO> Login(UserLoginDTO item);
        Task<UserLoginDTO> DirectBlockedUser(UserLoginDTO item);
        Task<bool> userBlocked(string userName);
        Task<LoginDTO> GetById(LoginDTO loginDTO);
        public LoginDTO UserId { get; set; }
        Task<bool> UsernameExist(string Username);
        Task<bool> UserLoginExists(string Username);
        Task<UserDTO> Logout(int Id,UserDTO item);
        Task<EAMISUSERLOGIN> LogoutSession(int Id, int UserId);
        Task<string> GetSessionIsLogout(int id);
    }
}
