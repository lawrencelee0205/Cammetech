﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using v3x.Models;

namespace v3x.Data
{
    public class v3xContext : DbContext
    {
        public v3xContext(DbContextOptions<v3xContext> options)
            : base(options)
        {
        }

        public DbSet<People> People { get; set; }
    }
}
