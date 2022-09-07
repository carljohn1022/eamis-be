using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisSupplierDTO
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDescription { get; set; }
        public int RegionCode { get; set; }
        public int ProvinceCode { get; set; }
        public int CityMunicipalityCode { get; set; }
        public int BrgyCode { get; set; }
        public string Street { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonNumber { get; set; }
        public string Bank {get; set;}
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Branch { get; set; }
        public bool IsActive { get; set; }
        public EamisBarangayDTO Barangay { get; set; }
    }
}
