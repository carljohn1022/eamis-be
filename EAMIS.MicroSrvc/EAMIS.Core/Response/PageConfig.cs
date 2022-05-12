using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Response.DTO
{
    public class PageConfig
    {
        public PageConfig()
        {
        }

        public int? Size { get; set; }
        public int? Index { get; set; }
        public string SortBy { get; set; }
        public bool IsAscending { get; set; }
    }
}
