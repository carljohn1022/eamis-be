using EAMIS.Common.DTO.Masterfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IRefreshTokenRepositories
    {
        Task<RefreshTokenDTO> GetByToken(string RefreshToken);
        Task Create(RefreshTokenDTO refreshTokenDTO);
        public RefreshTokenDTO refreshToken { get; set; }
    }
}
