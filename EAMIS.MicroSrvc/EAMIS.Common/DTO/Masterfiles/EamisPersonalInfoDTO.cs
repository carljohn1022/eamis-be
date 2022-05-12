using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisPersonalInfoDTO
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int DirectoryId { get; set; }
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Postion { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Region { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Barangay { get; set; }
        public string Street { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public EamisUsersDTO Users { get; set; }
    }
}
