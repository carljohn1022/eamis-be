using EAMIS.Common.DTO.LookUp;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Report_Catalog;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Report_Catalog;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Report_Catalog
{
    public class EamisReportCatalogRepository : IEamisReportCatalogRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisReportCatalogRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<List<LookupDTO>> FundSourceList()
        {
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION
                                          .Where(f => !(f.FUND_SOURCE == null || f.FUND_SOURCE == string.Empty))
                                          .GroupBy(x => x.FUND_SOURCE)
                                          .Select(i => new LookupDTO
                                          { 
                                              LookUpValue = i.Key })
                                          .ToList();

            return result;
        }
        public async Task<List<LookupDTO>> TransactionNumberICS()
        {
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION
                                          .Where(f => f.TRAN_TYPE == "ICS")
                                          .GroupBy(x => x.TRANSACTION_NUMBER)
                                          .Select(i => new LookupDTO
                                          {
                                              LookUpValue = i.Key
                                          })
                                          .ToList();

            return result;
        }

        public async Task<List<LookupDTO>> TransactionNumberPAR()
        {
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION
                                          .Where(f => f.TRAN_TYPE == "PAR")
                                          .GroupBy(x => x.TRANSACTION_NUMBER)
                                          .Select(i => new LookupDTO
                                          {
                                              LookUpValue = i.Key
                                          })
                                          .ToList();

            return result;
        }
        public async Task<List<LookupDTO>> TransactionNumberIssuance()
        {
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION
                                          .Where(f => f.TRANSACTION_TYPE == "Issuance/Releasing")
                                          .GroupBy(x => x.TRANSACTION_NUMBER)
                                          .Select(i => new LookupDTO
                                          {
                                              LookUpValue = i.Key
                                          })
                                          .ToList();

            return result;
        }
        public async Task<List<LookupDTO>> PropertyNumberList()
        {
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                          .Where(f => !(f.PROPERTY_NUMBER == null || f.PROPERTY_NUMBER == string.Empty))
                                          .GroupBy(x => x.PROPERTY_NUMBER)
                                          .Select(i => new LookupDTO
                                          {
                                              LookUpValue = i.Key
                                          })
                                          .ToList();

            return result;
        }
        public async Task<List<LookupDTO>> ItemCodeList()
        {
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                          .Where(f => !(f.ITEM_CODE == null || f.ITEM_CODE == string.Empty))
                                          .GroupBy(x => x.ITEM_CODE)
                                          .Select(i => new LookupDTO
                                          {
                                              LookUpValue = i.Key
                                          })
                                          .ToList();

            return result;
        }
        public async Task<List<LookupDTO>> OfficeList()
        {
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                          .Where(f => !(f.OFFICE == null || f.OFFICE == string.Empty))
                                          .GroupBy(x => x.OFFICE)
                                          .Select(i => new LookupDTO
                                          {
                                              LookUpValue = i.Key
                                          })
                                          .ToList();

            return result;
        }
        public async Task<List<LookupDTO>> ResponsibilityCodeList()
        {
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                          .Where(f => !(f.RESPONSIBILITY_CODE == null || f.RESPONSIBILITY_CODE == string.Empty))
                                          .GroupBy(x => x.RESPONSIBILITY_CODE)
                                          .Select(i => new LookupDTO
                                          {
                                              LookUpValue = i.Key,
                                          })
                                          .ToList();
            return result;
        }
        public async Task<List<LookupDTO>> PropertyType()
        {
            var result = _ctx.EAMIS_PROPERTYITEMS
                              .Where(f => !(f.PROPERTY_TYPE == null || f.PROPERTY_TYPE == string.Empty))
                              .Where(f => f.PROPERTY_TYPE == "High Value Semi-Expendable" || f.PROPERTY_TYPE == "Low Value Semi-Expendable")
                              .GroupBy(x => x.PROPERTY_TYPE)
                              .Select(i => new LookupDTO
                              {
                                  LookUpValue = i.Key,
                              })
                              .ToList();
            return result;
        }
        public async Task<List<LookupDTO>> CategoryName()
        {
            var result = _ctx.EAMIS_ITEM_CATEGORY
                                          .Where(f => !(f.CATEGORY_NAME == null || f.CATEGORY_NAME == string.Empty))
                                          .GroupBy(x => x.CATEGORY_NAME)
                                          .Select(i => new LookupDTO
                                          {
                                              LookUpValue = i.Key,
                                          })
                                          .ToList();
            return result;
        }
        public async Task<List<LookupDTO>> TransactionNumberMaterial()
        {
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION
                                          .Where(f => f.TRANSACTION_TYPE == "Issuance/Releasing - Materials")
                                          .GroupBy(x => x.TRANSACTION_NUMBER)
                                          .Select(i => new LookupDTO
                                          {
                                              LookUpValue = i.Key
                                          })
                                          .ToList();

            return result;
        }


        public async Task<EamisReportCatalogDTO> Delete(EamisReportCatalogDTO item)
        {
            try
            {
                EAMISREPORTCATALOG data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Deleted;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }

        public async Task<EamisReportCatalogDTO> Insert(EamisReportCatalogDTO item)
        {
            try
            {
                EAMISREPORTCATALOG data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }

        public async Task<DataList<EamisReportCatalogDTO>> List(EamisReportCatalogDTO filter, PageConfig config)
        {
            IQueryable<EAMISREPORTCATALOG> query = FilteredEntities(filter);

            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisReportCatalogDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        public IQueryable<EAMISREPORTCATALOG> PagedQuery(IQueryable<EAMISREPORTCATALOG> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public async Task<EamisReportCatalogDTO> Update(EamisReportCatalogDTO item)
        {
            try
            {
                EAMISREPORTCATALOG data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Modified;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }
        private EamisReportCatalogDTO MapToDTO(EAMISREPORTCATALOG item)
        {
            if (item == null) return new EamisReportCatalogDTO();
            return new EamisReportCatalogDTO
            {
                Id = item.ID,
                ReportName = item.REPORT_NAME,
                ReportDescription = item.REPORT_DESCRIPTION
            };

        }
        private EAMISREPORTCATALOG MapToEntity(EamisReportCatalogDTO item)
        {
            if (item == null) return new EAMISREPORTCATALOG();
            return new EAMISREPORTCATALOG
            {
                ID = item.Id,
                REPORT_DESCRIPTION = item.ReportDescription,
                REPORT_NAME = item.ReportName
            };
        }
        private IQueryable<EamisReportCatalogDTO> QueryToDTO(IQueryable<EAMISREPORTCATALOG> query)
        {
            return query.Select(x => new EamisReportCatalogDTO
            {
                Id = x.ID,
                ReportName = x.REPORT_NAME,
                ReportDescription = x.REPORT_DESCRIPTION
            });
        }
        private IQueryable<EAMISREPORTCATALOG> FilteredEntities(EamisReportCatalogDTO filter, IQueryable<EAMISREPORTCATALOG> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISREPORTCATALOG>(true);
            if (filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.ReportName))
                predicate.And(x => x.REPORT_NAME.Contains(filter.ReportName.ToLower()));
            if (!string.IsNullOrEmpty(filter.ReportDescription))
                predicate.And(x => x.REPORT_DESCRIPTION.Contains(filter.ReportDescription.ToLower()));
            var query = custom_query ?? _ctx.EAMIS_REPORT_CATALOG;
            return query.Where(predicate);
        }

    }
}
