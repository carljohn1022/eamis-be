using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Ais
{
    public class AisPersonnelDTO
    {
        public int Id { get; set; }
        public int OfficeId { get; set; }
        public string AgencyEmployeeNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string ExtensionName { get; set; }
        public string NickName { get; set; }
        public int SexId { get; set; }
        public string ProfilePhoto { get; set; }
        public bool isDeleted { get; set; }
        public string CurrentPosition { get; set; }
        public AisCodeListValueDTO CodeValue { get; set; }
        public AisOfficeDTO OfficeInfo { get; set; }
    }
}
