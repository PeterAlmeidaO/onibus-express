using Microsoft.EntityFrameworkCore;
using OnibusExpress.Domain.Entities;

namespace OnibusExpress.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public const string IndiceAssentoUnico = "IX_Reservas_ViagemId_NumeroAssento_Confirmada";

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Rota> Rotas { get; set; }
        public DbSet<Viagem> Viagens { get; set; }
        public DbSet<Passageiro> Passageiros { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeamento da Reserva
            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.CodigoReserva).IsUnique(); // Garante código único no banco
                entity.Property(e => e.CodigoReserva).HasMaxLength(20).IsRequired();

                // Restrict: apagar viagem ou passageiro com reservas deve falhar, não apagar o histórico junto
                entity.HasOne(e => e.Viagem).WithMany().HasForeignKey(e => e.ViagemId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Passageiro).WithMany().HasForeignKey(e => e.PassageiroId).OnDelete(DeleteBehavior.Restrict);

                // Garante no banco que um assento só tenha uma reserva confirmada por viagem (evita overbooking concorrente)
                entity.HasIndex(e => new { e.ViagemId, e.NumeroAssento })
                      .IsUnique()
                      .HasFilter($"\"Status\" = {(int)ReservaStatus.Confirmada}")
                      .HasDatabaseName(IndiceAssentoUnico);
            });

            // Mapeamento do Passageiro e do Value Object (CPF)
            modelBuilder.Entity<Passageiro>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(254).IsRequired();

                // Ensina o EF Core a salvar o Value Object Cpf na mesma tabela do Passageiro
                entity.OwnsOne(e => e.Cpf, cpf =>
                {
                    cpf.Property(c => c.Numero)
                       .HasColumnName("Cpf")
                       .HasMaxLength(11)
                       .IsRequired();
                });
            });

            modelBuilder.Entity<Viagem>(entity =>
            {
                entity.HasOne(e => e.Rota).WithMany().HasForeignKey(e => e.RotaId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Rota>(entity =>
            {
                entity.Property(e => e.Origem).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Destino).HasMaxLength(100).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}