using CafeDunyasi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CafeDunyasi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BusinessInfo> BusinessInfo { get; set; }
        public DbSet<FollowingAccounts> FollowingAccounts { get; set; }
        public DbSet<PostComments> PostComments { get; set; }
        public DbSet<PostLikes> PostLikes { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<Stories> Stories { get; set; }
        //public DbSet<Users> Users { get; set; }
    }
}
