using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Data
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // ↴ IMPORTANTES: dizem ao EF para usar os nomes reais das tabelas no MySQL
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<Project>().ToTable("project");
            modelBuilder.Entity<TaskItem>().ToTable("taskitem");
            modelBuilder.Entity<TaskComment>().ToTable("taskcomment");
            modelBuilder.Entity<TaskHistory>().ToTable("taskhistory");


            modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();

            //modelBuilder.Entity<Project>()
            //    .HasOne(p => p.Owner)
            //    .WithMany(u => u.Projects)
            //    .HasForeignKey(p => p.OwnerId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<TaskItem>()
            //    .HasMany(t => t.Comments)
            //    .WithOne(c => c.Task)
            //    .HasForeignKey(c => c.TaskItemId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<TaskItem>()
            //    .HasMany(t => t.History)
            //    .WithOne(h => h.Task)
            //    .HasForeignKey(h => h.TaskItemId)
            //    .OnDelete(DeleteBehavior.Cascade);


            //// Configuração para a Entidade Project
            //modelBuilder.Entity<Project>(entity =>
            //{
            //    entity.ToTable("Projects"); // Mapeia para a tabela 'Projects'
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.Title).HasMaxLength(200).IsRequired();

            //    // Configuração da Chave Estrangeira FK_Project_User
            //    entity.HasOne<User>()
            //        .WithMany()
            //        .HasForeignKey(e => e.OwnerId)
            //        .OnDelete(DeleteBehavior.Restrict); // ON DELETE RESTRICT
            //});

            //// Configuração para a Entidade User
            //modelBuilder.Entity<User>(entity =>
            //{
            //    entity.ToTable("User");
            //    entity.HasIndex(e => e.Email).IsUnique(); // UNIQUE (Email)
            //});

            //// Configuração da TaskHistory (Mapeamento para a tabela 'TaskHistory')
            //modelBuilder.Entity<TaskHistory>(entity =>
            //{
            //    entity.ToTable("TaskHistory");
            //    entity.HasKey(e => e.Id);
            //    // Configurar chave estrangeira FK_TaskHistory_Task
            //    entity.HasOne<TaskItem>()
            //          .WithMany()
            //          .HasForeignKey(e => e.TaskItemId);
            //});

            base.OnModelCreating(modelBuilder);
        }
    }
}