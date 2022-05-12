﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Ais
{
    public class AisCodeListValueDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CodeListType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
