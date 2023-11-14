using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialGatherKuznetsov.Models;

namespace SocialGatherKuznetsov.Data
{
    public class SocialGatherKuznetsovContext : DbContext
    {
        public SocialGatherKuznetsovContext (DbContextOptions<SocialGatherKuznetsovContext> options)
            : base(options)
        {
        }

        public DbSet<SocialGatherKuznetsov.Models.RegistreationData> RegistreationData { get; set; } = default!;
    }
}
