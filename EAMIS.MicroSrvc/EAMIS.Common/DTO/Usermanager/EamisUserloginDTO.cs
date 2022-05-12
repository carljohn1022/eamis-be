using EAMIS.Common.DTO.Ais;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisUserloginDTO
    {
        public class LoginDTO
        {
            public int Id { get; set; }
            public string ComputerName { get; set; }
            public UserDTO UsersToken { get; set; }

        }
        public class UserDTO
        {
            public int Id { get; set; }
            public int User_Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public bool IsActive { get; set; }
            public bool isDeleted { get; set; }
            public bool isBlocked { get; set; }
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public List<EamisUserRolesDTO> userRole { get; set; }
            public AisPersonnelDTO PersonnelInfo {get;set;}
        }
        public class UserLoginDTO
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        public class NewToken
        {
            public int UserId { get; set; }
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }
    }
}
