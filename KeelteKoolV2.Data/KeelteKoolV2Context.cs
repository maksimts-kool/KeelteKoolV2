using KeelteKoolV2.Core.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeelteKoolV2.Data
{
    public class KeelteKoolV2Context : IdentityDbContext<ApplicationUser>
    {
        public KeelteKoolV2Context(DbContextOptions<KeelteKoolV2Context> options):base (options) 
        {
            //set tables here
        }
    }
}
