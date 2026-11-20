using System;
using System.Threading.Tasks;
using OnibusExpress.Application.DTOs;
using OnibusExpress.Application.Interfaces;
using OnibusExpress.Application.Mappings;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Exceptions;
using OnibusExpress.Domain.Interfaces;
using OnibusExpress.Domain.ValueObjects;

namespace OnibusExpress.Application.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IViagemRepository _viagemRepository;
        private readonly IPassageiroRepository _passageiroRepository;

        public ReservaService(IReservaRepository reservaRepository, IViagemRepository viagemRepository, IPassageiroRepository passageiroRepository)
        {
            _reservaRepository = reservaRepository;
            _viagemRepository = viagemRepository;
            _passageiroRepository = passageiroRepository;
        }

        public async Task<ReservaCriadaDto> CriarReservaAsync(CriarReservaDto dto)
        {
            var cpf = new Cpf(dto.Cpf);

            var viagem = await _viagemRepository.GetByIdAsync(dto.ViagemId);
            if (viagem == null) throw new NotFoundException("Viagem não encontrada.");

            if (viagem.IsPassada()) 
                throw new InvalidOperationException("Não é possível reservar passagens para uma viagem que já ocorreu.");

            if (dto.NumeroAssento <= 0 || dto.NumeroAssento > viagem.TotalAssentos)
                throw new ArgumentException("Número de assento inválido para esta viagem.");

            if (await _reservaRepository.ExisteAssentoConfirmadoAsync(dto.ViagemId, dto.NumeroAssento))
                throw new InvalidOperationException("Este assento já está ocupado.");

            var passageiro = await _passageiroRepository.GetByCpfAsync(cpf.Numero);

            if (passageiro == null)
                passageiro = new Passageiro(dto.Nome, cpf, dto.Email, dto.DataNascimento);
            else
                // Passageiro já cadastrado pelo CPF: mantém o cadastro e atualiza os dados de contato informados
                passageiro.AtualizarContato(dto.Nome, dto.Email);

            var reserva = new Reserva(viagem, passageiro, dto.NumeroAssento);

            // Grava reserva e passageiro (novo ou atualizado) na mesma transação
            await _reservaRepository.AddAsync(reserva);

            return reserva.ToCriadaDto();
        }

        public async Task<ReservaDetalheDto> ConsultarPorCodigoAsync(string codigo)
        {
            var reserva = await ObterReservaAsync(codigo);
            return reserva.ToDetalheDto();
        }

        public async Task CancelarAsync(string codigo)
        {
            var reserva = await ObterReservaAsync(codigo);

            // A regra de prazo fica no domínio; a reserva é mantida com status Cancelada (histórico preservado)
            reserva.Cancelar();

            await _reservaRepository.UpdateAsync(reserva);
        }

        private async Task<Reserva> ObterReservaAsync(string codigo)
        {
            return await _reservaRepository.GetByCodigoAsync(codigo)
                ?? throw new NotFoundException("Reserva não encontrada.");
        }
    }
}
