using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisPropertyDetailsDTO
    {
        public int Id { get; set; }
        public int PropertyTypeId { get; set; }
        public string PropertyName { get; set; }
        public string Brand { get; set; }
        public string ModelNo { get; set; }
        public string SerialNo { get; set; }
        public decimal UnitCost { get; set; }
        public int UomId { get; set; }
        public double StockNo { get; set; }
        public bool IsStockable { get; set; }
        public double QtyInStock { get; set; }
        public int SourceId { get; set; }
        public bool IsProperty { get; set; }
    }
}
