using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository
{
    public class RefreshTokenRepository : IRefreshTokenRepositories
    {
        public readonly List<RefreshTokenDTO> _refreshTokenDTOs = new List<RefreshTokenDTO>();
        public RefreshTokenDTO refreshToken { get; set; }
        public readonly EAMISContext _ctx;

        public async Task Create(RefreshTokenDTO refreshTokenDTO)
        {
            refreshTokenDTO.Id = Guid.NewGuid();
            _refreshTokenDTOs.Add(refreshTokenDTO);
            await Task.CompletedTask;
        }

        public  Task<RefreshTokenDTO> GetByToken(string RefreshToken)
        {
            _refreshTokenDTOs.Add(new RefreshTokenDTO
            {
                Id = Guid.NewGuid(),
                RefreshToken = RefreshToken,
            });
            refreshToken = _refreshTokenDTOs.FirstOrDefault(x => x.RefreshToken == RefreshToken);
            return  Task.FromResult(refreshToken);
        }
    }
}
