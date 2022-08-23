using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Masterfiles
{
    public class EamisAttachedFilesRepository : IEamisAttachedFilesRepository
    {
        private readonly EAMISContext _ctx;
        public EamisAttachedFilesRepository(EAMISContext ctx)
        {
            _ctx = ctx;
        }

        private EAMISATTACHEDFILES MapToEntity(EamisAttachedFilesDTO item)
        {
            if (item == null) return new EAMISATTACHEDFILES();
            return new EAMISATTACHEDFILES
            {
                ID = item.Id,
                ATTACHED_FILENAME = item.FileName,
                MODULE_NAME = item.ModuleName,
                TRANID = item.TransactionNumber,
                USERSTAMP = item.UserStamp,
                TIIMESTAMP = item.TimeStamp.ToString()
            };
        }
        public async Task<bool> Delete(EamisAttachedFilesDTO file)
        {
            try
            {

                EAMISATTACHEDFILES data = MapToEntity(file);
                _ctx.Entry(data).State = EntityState.Deleted;
                await _ctx.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            return true;
        }

        public async Task<bool> Insert(List<EamisAttachedFilesDTO> files)
        {
            try
            {
                foreach (var file in files)
                {
                    EAMISATTACHEDFILES data = MapToEntity(file);
                    _ctx.Entry(data).State = EntityState.Added;
                    await _ctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            return true;
        }

    }
}