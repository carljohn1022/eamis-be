using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisResponsibilityCenterDTO
    {
        public int Id { get; set; }
        public string mainGroupCode { get; set; }
        public string mainGroupDesc { get; set; }
        public string subGroupCode { get; set; }
        public string subGroupDesc { get; set; }
        public string officeCode { get; set; }
        public string officeDesc { get; set; }
        public string unitCode { get; set; }
        public string unitDesc { get; set; }
        public string locationCode { get; set; }
        public string locationDescription { get; set; }
        public bool isActive { get; set; }
        public string responsibilityCenter { get; set; }
        public string UserStamp { get; set; }
    }
}
