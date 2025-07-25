﻿using Microsoft.EntityFrameworkCore;
using MultiAuthAPI.Models;

namespace MultiAuthAPI.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<OtpEntry> OtpEntries { get; set; }

    }
}
