using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO
{
    public class EamisMunicipalityDTO
    {
        public string Psgcode { get; set; }
        public string CityMunicipalityDescription { get; set; }
        public int RegionCode { get; set; }
        public int ProvinceCode { get; set; }
        public int CityMunicipalityCode { get; set; }
    }
}
