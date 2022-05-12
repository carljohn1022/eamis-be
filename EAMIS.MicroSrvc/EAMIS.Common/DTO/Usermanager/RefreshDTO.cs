using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class RefreshDTO
    {
        [Required]
        public string RefreshToken { get; set; }
    }
    public class RefreshTokenDTO
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
