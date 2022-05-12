using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO
{
    public class EamisBarangayDTO
    {
        public int BrgyCode { get; set; }
        public string BrgyDescription { get; set; }
        public int RegionCode { get; set; }
        public EamisRegionDTO Region { get; set; }
        public int ProvinceCode { get; set; }
        public EamisProvinceDTO Province { get; set; }
        public int MunicipalityCode { get; set; }
        public EamisMunicipalityDTO Municipality { get; set; }
    }
}
