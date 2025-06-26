using CeiliApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CeiliApi.Data
{
    public class CeiliDbContext : DbContext
    {
        public CeiliDbContext(DbContextOptions<CeiliDbContext> options) : base(options) { }

        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Evaluacion> Evaluaciones { get; set; }
        public DbSet<RetroalimentacionIA> Retroalimentaciones { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Evaluacion>()
                .HasOne(e => e.Alumno)
                .WithMany(a => a.Evaluaciones)
                .HasForeignKey(e => e.AlumnoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evaluacion>()
                .HasOne(e => e.Docente)
                .WithMany(d => d.Evaluaciones)
                .HasForeignKey(e => e.DocenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación uno a uno Evaluacion <-> RetroalimentacionIA
            modelBuilder.Entity<Evaluacion>()
                .HasOne(e => e.Retroalimentacion)
                .WithOne(r => r.Evaluacion)
                .HasForeignKey<RetroalimentacionIA>(r => r.EvaluacionId)
                .OnDelete(DeleteBehavior.Cascade); // Borra la retro si se borra la evaluación

            base.OnModelCreating(modelBuilder);
        }
    }
}
