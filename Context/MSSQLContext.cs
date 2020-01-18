using Microsoft.EntityFrameworkCore;

namespace EFConsole {
    public class MSSQLContext : DbContext
    {
        public DbSet<Pessoa> Pessoa { get; set; }

        // Método de configuração
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        {
            // String de conexão
            optionsBuilder
                .UseSqlServer(@"Server=localhost;Database=dbmodel;User Id=sa;Password=sa@12345;");
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Pessoa>(p => {
                // Tabela
                p.ToTable("pessoa");

                p
                    .Property(v => v.nome)
                    .HasColumnType("varchar(50)");

                // Chave Primaria
                p.HasKey(k => k.id);
            });
        }
    }
}