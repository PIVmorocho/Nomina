using Microsoft.EntityFrameworkCore;
using NominaApp.Models;

namespace NominaApp.Data
{
    /// <summary>Contexto de base de datos de la aplicación de nómina.</summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DeptEmp> DeptEmps { get; set; }
        public DbSet<DeptManager> DeptManagers { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LogAuditoriaSalarios> LogAuditoriaSalarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── DeptEmp: clave compuesta ──────────────────────────────────────
            modelBuilder.Entity<DeptEmp>(entity =>
            {
                entity.HasKey(e => new { e.EmpNo, e.DeptNo, e.FromDate });
                entity.Property(e => e.EmpNo).HasColumnName("emp_no");
                entity.Property(e => e.DeptNo).HasColumnName("dept_no");
                entity.Property(e => e.FromDate).HasColumnName("from_date");
                entity.Property(e => e.ToDate).HasColumnName("to_date");
                entity.HasOne(e => e.Employee)
                      .WithMany(e => e.DeptEmps)
                      .HasForeignKey(e => e.EmpNo);
                entity.HasOne(e => e.Department)
                      .WithMany(d => d.DeptEmps)
                      .HasForeignKey(e => e.DeptNo);
            });

            // ── DeptManager: clave compuesta ──────────────────────────────────
            modelBuilder.Entity<DeptManager>(entity =>
            {
                entity.HasKey(e => new { e.EmpNo, e.DeptNo, e.FromDate });
                entity.Property(e => e.EmpNo).HasColumnName("emp_no");
                entity.Property(e => e.DeptNo).HasColumnName("dept_no");
                entity.Property(e => e.FromDate).HasColumnName("from_date");
                entity.Property(e => e.ToDate).HasColumnName("to_date");
                entity.HasOne(e => e.Employee)
                      .WithMany(e => e.DeptManagers)
                      .HasForeignKey(e => e.EmpNo);
                entity.HasOne(e => e.Department)
                      .WithMany(d => d.DeptManagers)
                      .HasForeignKey(e => e.DeptNo);
            });

            // ── Title: clave compuesta ────────────────────────────────────────
            modelBuilder.Entity<Title>(entity =>
            {
                entity.HasKey(e => new { e.EmpNo, e.TitleName, e.FromDate });
                entity.Property(e => e.EmpNo).HasColumnName("emp_no");
                entity.Property(e => e.TitleName).HasColumnName("title");
                entity.Property(e => e.FromDate).HasColumnName("from_date");
                entity.Property(e => e.ToDate).HasColumnName("to_date");
                entity.HasOne(e => e.Employee)
                      .WithMany(e => e.Titles)
                      .HasForeignKey(e => e.EmpNo);
            });

            // ── Salary: clave compuesta ───────────────────────────────────────
            modelBuilder.Entity<Salary>(entity =>
            {
                entity.HasKey(e => new { e.EmpNo, e.FromDate });
                entity.Property(e => e.EmpNo).HasColumnName("emp_no");
                entity.Property(e => e.SalaryAmount).HasColumnName("salary");
                entity.Property(e => e.FromDate).HasColumnName("from_date");
                entity.Property(e => e.ToDate).HasColumnName("to_date");
                entity.HasOne(e => e.Employee)
                      .WithMany(e => e.Salaries)
                      .HasForeignKey(e => e.EmpNo);
            });

            // ── User: PK string ───────────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Usuario);
                entity.Property(e => e.EmpNo).HasColumnName("emp_no");
                entity.HasOne(e => e.Employee)
                      .WithOne(e => e.User)
                      .HasForeignKey<User>(e => e.EmpNo);
            });

            // ── Employee ──────────────────────────────────────────────────────
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.EmpNo).HasColumnName("emp_no");
                entity.Property(e => e.BirthDate).HasColumnName("birth_date");
                entity.Property(e => e.FirstName).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasColumnName("last_name");
                entity.Property(e => e.HireDate).HasColumnName("hire_date");
                entity.HasIndex(e => e.Ci).IsUnique();
                entity.HasIndex(e => e.Correo).IsUnique();
            });

            // ── Department ────────────────────────────────────────────────────
            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.DeptNo).HasColumnName("dept_no");
                entity.Property(e => e.DeptName).HasColumnName("dept_name");
            });

            // ── LogAuditoriaSalarios ──────────────────────────────────────────
            modelBuilder.Entity<LogAuditoriaSalarios>(entity =>
            {
                entity.Property(e => e.FechaActualizacion).HasColumnName("fechaActualizacion");
                entity.Property(e => e.DetalleCambio).HasColumnName("DetalleCambio");
                entity.Property(e => e.EmpNo).HasColumnName("emp_no");
                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey(e => e.EmpNo);
            });
        }
    }
}