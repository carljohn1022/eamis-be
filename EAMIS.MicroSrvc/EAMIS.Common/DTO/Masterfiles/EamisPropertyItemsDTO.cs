
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisPropertyItemsDTO
    {
        public int Id { get; set; }
        public string AppNo { get; set; }
        public string PropertyNo { get; set; }
        public string PropertyName { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Brand { get; set; }
        public int UomId { get; set; }
        public int WarehouseId { get; set; }
        public string PropertyType { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }
        public int SupplierId { get; set; }
        public bool IsActive { get; set; }
        public string ImageURL { get; set; }
        public IFormFile Photo { get; set; }
        public EamisItemCategoryDTO ItemCategory { get; set; }
        public EamisUnitofMeasureDTO UnitOfMeasure { get; set; }
        public EamisWarehouseDTO Warehouse { get; set; }
        public EamisSupplierDTO Supplier { get; set; }

    }
}
