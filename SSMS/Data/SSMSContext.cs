using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SSMS.Models;

namespace SSMS.Data;

public partial class SSMSContext : DbContext
{
    public SSMSContext()
    {
    }

    public SSMSContext(DbContextOptions<SSMSContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Mark> Marks { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Mohammad\\SQLEXPRESS;Database=SSMS;Trusted_Connection=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927A0B4B0C05C");

            entity.Property(e => e.ClassId)
                .ValueGeneratedNever()
                .HasColumnName("ClassID");
            entity.Property(e => e.ClassNameArabic).HasMaxLength(50);
            entity.Property(e => e.ClassNameEnglish).HasMaxLength(50);
        });

        modelBuilder.Entity<Mark>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.ClassId, e.MaterialId }).HasName("PK__Marks__38B1BE101BEA22C5");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.Mark1)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("Mark");

            entity.HasOne(d => d.Class).WithMany(p => p.Marks)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Marks__ClassID__5812160E");

            entity.HasOne(d => d.Material).WithMany(p => p.Marks)
                .HasForeignKey(d => d.MaterialId)
                .HasConstraintName("FK__Marks__MaterialI__59063A47");

            entity.HasOne(d => d.Student).WithMany(p => p.Marks)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Marks__StudentID__571DF1D5");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Material__C506131783370C7D");

            entity.Property(e => e.MaterialId)
                .ValueGeneratedNever()
                .HasColumnName("MaterialID");
            entity.Property(e => e.MaterialNameArabic).HasMaxLength(50);
            entity.Property(e => e.MaterialNameEnglish).HasMaxLength(50);

            entity.HasMany(d => d.Classes).WithMany(p => p.Materials)
                .UsingEntity<Dictionary<string, object>>(
                    "ClassMaterial",
                    r => r.HasOne<Class>().WithMany()
                        .HasForeignKey("ClassId")
                        .HasConstraintName("FK__ClassMate__Class__47DBAE45"),
                    l => l.HasOne<Material>().WithMany()
                        .HasForeignKey("MaterialId")
                        .HasConstraintName("FK__ClassMate__Mater__46E78A0C"),
                    j =>
                    {
                        j.HasKey("MaterialId", "ClassId").HasName("PK__ClassMat__D9B7816D0C48C695");
                        j.ToTable("ClassMaterials");
                        j.IndexerProperty<int>("MaterialId").HasColumnName("MaterialID");
                        j.IndexerProperty<int>("ClassId").HasColumnName("ClassID");
                    });
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A797BEFCD0F");

            entity.HasIndex(e => e.UserId, "UQ__Students__1788CCAD10129B91").IsUnique();

            entity.Property(e => e.StudentId)
                .ValueGeneratedNever()
                .HasColumnName("StudentID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.FullNameArabic).HasMaxLength(50);
            entity.Property(e => e.FullNameEnglish).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Class).WithMany(p => p.Students)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Students__ClassI__4316F928");

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Students__UserID__440B1D61");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teachers__EDF2594478F4A71C");

            entity.HasIndex(e => e.UserId, "UQ__Teachers__1788CCADD8C65CBA").IsUnique();

            entity.Property(e => e.TeacherId)
                .ValueGeneratedNever()
                .HasColumnName("TeacherID");
            entity.Property(e => e.FullNameArabic).HasMaxLength(50);
            entity.Property(e => e.FullNameEnglish).HasMaxLength(50);
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Material).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.MaterialId)
                .HasConstraintName("FK__Teachers__Materi__3F466844");

            entity.HasOne(d => d.User).WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Teachers__UserID__3E52440B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACC0EC1719");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(20);
            entity.Property(e => e.UserName).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
