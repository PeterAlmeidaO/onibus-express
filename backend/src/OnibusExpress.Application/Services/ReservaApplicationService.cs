using System;
using System.Threading.Tasks;
using OnibusExpress.Domain.Entities;
using OnibusExpress.Domain.Exceptions;
using OnibusExpress.Domain.Interfaces;
using OnibusExpress.Domain.ValueObjects;

namespace OnibusExpress.Application.Services
{
    public class ReservaApplicationService
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IViagemRepository _viagemRepository;
        private readonly IPassageiroRepository _passageiroRepository;

        public ReservaApplicationService(IReservaRepository reservaRepository, IViagemRepository viagemRepository, IPassageiroRepository passageiroRepository)
        {
            _reservaRepository = reservaRepository;
            _viagemRepository = viagemRepository;
            _passageiroRepository = passageiroRepository;
        }

        public async Task<Reserva> CriarReservaAsync(Guid viagemId, string nome, string cpfInput, string email, DateTime dataNascimento, int numeroAssento)
        {
            var cpf = new Cpf(cpfInput);

            var viagem = await _viagemRepository.GetByIdAsync(viagemId);
            if (viagem == null) throw new NotFoundException("Viagem não encontrada.");

            if (viagem.IsPassada()) 
                throw new InvalidOperationException("Não é possível reservar passagens para uma viagem que já ocorreu.");

            if (numeroAssento <= 0 || numeroAssento > viagem.TotalAssentos)
                throw new ArgumentException("Número de assento inválido para esta viagem.");

            if (await _reservaRepository.ExisteAssentoConfirmadoAsync(viagemId, numeroAssento))
                throw new InvalidOperationException("Este assento já está ocupado.");

            var passageiro = await _passageiroRepository.GetByCpfAsync(cpf.Numero);

            if (passageiro == null)
                passageiro = new Passageiro(nome, cpf, email, dataNascimento);
            else
                // Passageiro já cadastrado pelo CPF: mantém o cadastro e atualiza os dados de contato informados
                passageiro.AtualizarContato(nome, email);

            var reserva = new Reserva(viagem, passageiro, numeroAssento);

            // Grava reserva e passageiro (novo ou atualizado) na mesma transação
            await _reservaRepository.AddAsync(reserva);

            return reserva;
        }
    }
}
