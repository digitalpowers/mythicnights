using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MythicNights.DataContext
{
    public class GuildConfig
    {
        [ Key ]
        public ulong GuildId { get; set; }
        public ulong? ChannelId { get; set; }

    }
}
