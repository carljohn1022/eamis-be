using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Ais
{
    public class AisOfficeDTO
    {
        public int Id { get; set; }
        public int OfficeTypeId { get; set; }
        public int? ParentOfficeId { get; set; }
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public string? OrgCode { get; set; }
        public string? Address { get; set; }
    }
}
