using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisUserRolesDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public bool IsDeleted { get; set; }
        public EamisRolesDTO Roles { get; set; }
       
    }
}
