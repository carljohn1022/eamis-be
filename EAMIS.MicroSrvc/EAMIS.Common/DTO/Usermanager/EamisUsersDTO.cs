using EAMIS.Common.DTO.Ais;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisUsersDTO
    {
        public int User_Id { get; set; }
        public int PersonnelId { get; set; }
        public string Username { get; set; }
        public byte[] Password_Hash { get; set; }
        public byte[] Password_Salt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBlocked { get; set; }
        public string AgencyEmployeeNumber { get; set; }
        public List<EamisUserRolesDTO> UserRoles {get;set;}
        public EamisUserloginDTO UserLogin { get; set; }
        public AisPersonnelDTO PersonnelInfo { get; set; }
    }
    public class RegisterDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AgencyEmployeeNumber { get; set; }
    }
   
}