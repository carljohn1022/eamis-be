using EAMIS.Common.DTO.Masterfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Report_Catalog
{
   public class EamisUserReportLinkDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ReportId { get; set; }
        public bool CanView { get; set; }
        public EamisUsersDTO UserProfile { get; set; }
        public EamisReportCatalogDTO ReportCatalog { get; set; }
    }
}
