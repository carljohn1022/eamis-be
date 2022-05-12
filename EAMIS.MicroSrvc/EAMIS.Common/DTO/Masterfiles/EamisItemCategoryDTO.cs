using EAMIS.Common.DTO.Ais;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisItemCategoryDTO
    {
        public int Id { get; set; } 
        public string ShortDesc { get; set; }
        public int ChartOfAccountId { get; set; }
        public string CategoryName { get; set; } 
        public bool IsStockable { get; set; } 
        public int StockQuantity { get; set; }
        public int EstimatedLife { get; set; }
        public string CostMethod{ get; set; } 
        public string DepreciationMethod{ get; set; } 
        public bool IsSerialized{ get; set; }
        public bool isActive { get; set; }
        public AisOfficeDTO OfficeInfo { get; set; }
        public EamisChartofAccountsDTO ChartOfAccounts { get; set; }
        public List<EamisItemSubCategoryDTO> SubCategory { get; set; }
       
    }
}
