using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisGeneralChartofAccountsDTO
    {
        public int Id { get; set; }
        public string Classification { get; set; }
        public string SubClassification { get; set; }
        public string ClassificationGroup { get; set; }
        public List<EamisChartofAccountsDTO> ChartofAccountList { get; set; }
    }
}
