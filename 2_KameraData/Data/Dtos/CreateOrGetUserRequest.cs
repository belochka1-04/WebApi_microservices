using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    public class CreateOrGetUserRequest
    {
        public long TelegramId { get; set; }
        public int? ReferralUserId { get; set; }
    }
}
