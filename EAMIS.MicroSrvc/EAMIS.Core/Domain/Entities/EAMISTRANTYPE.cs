﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISTRANTYPE
    {
        public int ID { get; set; }
        public int ASSET_ID { get; set; }
        public string TRAN_TYPE { get; set; }
        public EAMISASSETCONDITIONTYPE ASSET_CONDITION_TYPE { get; set; }

    }
}