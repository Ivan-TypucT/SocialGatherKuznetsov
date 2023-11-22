using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialGatherKuznetsov.Models;

namespace SocialGatherKuznetsov.Data
{
    public class SocialGatherKuznetsov2Context : DbContext
    {
        public SocialGatherKuznetsov2Context (DbContextOptions<SocialGatherKuznetsov2Context> options)
            : base(options)
        {
        }

        public DbSet<SocialGatherKuznetsov.Models.Card> Card { get; set; } = default!;
    }
}
