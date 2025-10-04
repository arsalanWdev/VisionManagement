using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VisionManagement.Models;

public partial class VisionManagementContext : DbContext
{
    public VisionManagementContext()
    {
    }

    public VisionManagementContext(DbContextOptions<VisionManagementContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Project> Projects { get; set; }
    public DbSet<ProjectAssignment> ProjectAssignments { get; set; }
    public DbSet<Evaluation> Evaluations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.WebsiteLink).HasMaxLength(250);
            entity.Property(e => e.MobileAppLink).HasMaxLength(250);
            entity.Property(e => e.StartupDescription).HasMaxLength(1000);
            entity.Property(e => e.StartupStatus).HasMaxLength(100);
            entity.Property(e => e.StartupLogo).HasMaxLength(250);
            entity.Property(e => e.ProjectDemoVideoLink).HasMaxLength(250);
            entity.Property(e => e.FounderPhoto).HasMaxLength(250);
            entity.Property(e => e.SpotlightReason).HasMaxLength(1000);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AFCF3F722");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160C4069CA1").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

       
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C01C7909E");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4C2ADC1BA").IsUnique();

            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
