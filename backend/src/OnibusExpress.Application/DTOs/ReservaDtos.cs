using System;
using System.ComponentModel.DataAnnotations;

namespace OnibusExpress.Application.DTOs
{
    public class CriarReservaDto
    {
        [Required]
        public Guid ViagemId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DateTime DataNascimento { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Número de assento inválido.")]
        public int NumeroAssento { get; set; }
    }

    public record ReservaCriadaDto(string CodigoReserva, string Status);

    public record ReservaDetalheDto(string Codigo, string Status, int Assento, ReservaViagemDto Viagem, ReservaPassageiroDto Passageiro);

    public record ReservaViagemDto(string Origem, string Destino, DateTime Partida);

    public record ReservaPassageiroDto(string Nome, string Cpf);
}
