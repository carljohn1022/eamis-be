using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Report;
using EAMIS.Common.DTO.Report_Catalog;
using EAMIS.Core.ContractRepository.Report_Catalog;
//using EAMIS.Core.ContractRepository.Report;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Report
{
    public class EamisReportLinkRepository : IEamisReportLinkRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisReportLinkRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }


        public async Task<EamisUserReportLinkDTO> Delete(EamisUserReportLinkDTO item)
        {
            try
            {
                EAMISUSERREPORTLINK data = MapToEntity(item);
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

        public async Task<EamisUserReportLinkDTO> Insert(EamisUserReportLinkDTO item)
        {
            try
            {
                EAMISUSERREPORTLINK data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
                item.Id = data.ID;
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }

        public async Task<DataList<EamisUserReportLinkDTO>> List(EamisUserReportLinkDTO filter, PageConfig config)
        {
            IQueryable<EAMISUSERREPORTLINK> query = FilteredEntities(filter);

            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisUserReportLinkDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        public IQueryable<EAMISUSERREPORTLINK> PagedQuery(IQueryable<EAMISUSERREPORTLINK> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public async Task<EamisUserReportLinkDTO> Update(EamisUserReportLinkDTO item)
        {
            try
            {
                EAMISUSERREPORTLINK data = MapToEntity(item);
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
        private EamisUserReportLinkDTO MapToDTO(EAMISUSERREPORTLINK item)
        {
            if (item == null) return new EamisUserReportLinkDTO();
            return new EamisUserReportLinkDTO
            {
                Id = item.ID,
                CanView = item.CAN_VIEW,
                ReportId = item.REPORT_ID,
                UserId = item.USER_ID
            };

        }
        private EAMISUSERREPORTLINK MapToEntity(EamisUserReportLinkDTO item)
        {
            if (item == null) return new EAMISUSERREPORTLINK();
            return new EAMISUSERREPORTLINK
            {
                ID = item.Id,
                USER_ID = item.UserId,
                REPORT_ID = item.ReportId,
                CAN_VIEW = item.CanView
            };
        }
        private IQueryable<EamisUserReportLinkDTO> QueryToDTO(IQueryable<EAMISUSERREPORTLINK> query)
        {
            return query.Select(x => new EamisUserReportLinkDTO
            {
                Id = x.ID,
                CanView = x.CAN_VIEW,
                ReportId = x.REPORT_ID,
                UserId = x.USER_ID,
                UserProfile = _ctx.EAMIS_USERS.Select(u => new EamisUsersDTO
                {
                    User_Id = u.USER_ID,
                    Username = u.USERNAME
                }).Where(i => i.User_Id == x.USER_ID).FirstOrDefault(),
                ReportCatalog = _ctx.EAMIS_REPORT_CATALOG.Select(r => new EamisReportCatalogDTO
                {
                    Id = r.ID,
                    ReportName = r.REPORT_NAME,
                    ReportDescription = r.REPORT_DESCRIPTION
                }).Where(i => i.Id == x.REPORT_ID).FirstOrDefault()
            });
        }
        private IQueryable<EAMISUSERREPORTLINK> FilteredEntities(EamisUserReportLinkDTO filter, IQueryable<EAMISUSERREPORTLINK> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISUSERREPORTLINK>(true);
            if (filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);

            var query = custom_query ?? _ctx.EAMIS_USER_REPORT_LINK;
            return query.Where(predicate);
        }

    }
}
