﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISREPORTCATALOG
    {
        public int ID { get; set; }
        public string REPORT_NAME { get; set; }
        public string REPORT_DESCRIPTION { get; set; }
        public bool ACTIVE { get; set; }
    }
}
