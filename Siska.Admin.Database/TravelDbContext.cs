using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Siska.Admin.Model.Entities;
using Siska.Admin.Utility;

namespace Siska.Admin.Database
{
    public class SiskaDbContext : DbContext
    {
        private readonly List<AuditEntry> _auditEntries;
        private readonly IExtractUser _extractUser;

        public SiskaDbContext(DbContextOptions<SiskaDbContext> options, IExtractUser extractUser) : base(options)
        {
            _auditEntries = new List<AuditEntry>();
            _extractUser = extractUser;
        }

        #region System
        public DbSet<AuditEntry> auditEntries { get; set; }
        public DbSet<APIEndpoint> apiEndpoints { get; set; }
        public DbSet<Users> users { get; set; }
        public DbSet<Roles> roles { get; set; }
        #endregion System

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(new AuditInterceptor(_auditEntries, _extractUser));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("RolesUsers");

            modelBuilder.Entity<Users>()
                .HasMany(p => p.Roles)
                .WithMany(p => p.Users)
                .UsingEntity<IdentityUserRole<Guid>>(
                    l => l.HasOne<Roles>().WithMany().HasForeignKey(k => k.RoleId),
                    r => r.HasOne<Users>().WithMany().HasForeignKey(k => k.UserId)
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
