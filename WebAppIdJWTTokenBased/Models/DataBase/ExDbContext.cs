using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppIdJWTTokenBased.Models.DataBase
{
    public class ExDbContext : IdentityDbContext<IdentityUser>
    {
        public ExDbContext(DbContextOptions<ExDbContext> options) : base(options)
        { }

        public DbSet<Car> Cars { get; set; }
    }
}
