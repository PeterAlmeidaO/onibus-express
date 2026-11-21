# OniBus Express - API de Reservas

[![CI](https://github.com/PeterAlmeidaO/onibus-express/actions/workflows/ci.yml/badge.svg)](https://github.com/PeterAlmeidaO/onibus-express/actions/workflows/ci.yml)

Este repositório contém a solução de backend para o desafio técnico do OniBus Express. A API foi projetada para gerenciar rotas, viagens, passageiros e reservas de assentos, respeitando regras de negócio e garantindo a consistência dos dados.

---

## Tecnologias e Bibliotecas Utilizadas

* **.NET 10 (C#):** Escolhido por sua alta performance, tipagem forte e maturidade para a construção de APIs corporativas seguras e escaláveis.
* **Entity Framework Core 10:** ORM adotado para agilizar o mapeamento objeto-relacional, garantindo um controle seguro das migrations e do data seeding automático do banco de dados.
* **PostgreSQL 15:** Banco de dados relacional robusto, escolhido por sua confiabilidade em transações e consistência de dados (ACID).
* **Docker & Docker Compose:** Utilizados para garantir a paridade entre ambientes, eliminando divergências de configuração entre o desenvolvimento local e a máquina de avaliação.
* **Swashbuckle (Swagger):** Adotado para gerar a documentação interativa da API de forma automatizada, facilitando a validação e os testes manuais dos endpoints.
* **xUnit + Moq:** Frameworks utilizados na suíte de testes unitários do domínio, dos services, dos controllers e do tratamento de erros.
* **GitHub Actions:** Pipeline de CI que compila, executa os testes e valida a imagem Docker a cada pull request.

---

## Decisões de Arquitetura Relevantes

1. **Clean Architecture e DDD:** O código C# foi estruturado em camadas (Domain, Application, Infrastructure, Api). O Domain define as interfaces de repositório e a Infrastructure as implementa, de modo que as regras de negócio não dependem de detalhes de banco de dados ou do framework web.
2. **Tratamento de Fuso Horário:** Toda a persistência de datas ocorre em UTC (ISO 8601), evitando divergências entre os relógios da API e do PostgreSQL. O filtro `data` da busca de viagens é interpretado como um dia no horário de Brasília e convertido para a janela UTC correspondente.
3. **Consistência de Estado (Garantia de Assentos):** Além da validação na aplicação, um índice único no PostgreSQL impede que um mesmo assento tenha duas reservas confirmadas na mesma viagem. Isso evita overbooking mesmo quando duas requisições chegam na mesma fração de segundo.
4. **Tratamento Global de Erros:** As exceções de domínio são convertidas em respostas HTTP padronizadas (`400`, `404`, `409`) por um handler central, no formato `{ "erro": "mensagem" }`, sem expor detalhes internos.

---

## O que foi implementado

**O que foi implementado (MVP Backend Completo):**
* [x] API RESTful completa conectada ao PostgreSQL.
* [x] Infraestrutura Docker conteinerizada (Banco de Dados e Aplicação).
* [x] Execução automática de Migrations e Data Seeding ao iniciar a API.
* [x] Algoritmo nativo de validação de CPF na camada de Domínio.
* [x] Regra de negócio: Trava de segurança que proíbe o cancelamento de uma reserva com menos de 2 horas de antecedência para a partida.
* [x] Cancelamento que preserva o histórico (a reserva passa ao status `Cancelada` em vez de ser apagada).
* [x] Consulta e cancelamento de reserva restritos ao titular (CPF obrigatório no header `X-Cpf`), com CPF mascarado nas respostas.
* [x] Busca de viagens retornando apenas partidas futuras.
* [x] Suíte de testes automatizados (domínio, services, controllers e tratamento de erros).
* [x] Pipeline de CI no GitHub Actions.
* [x] Documentação interativa via Swagger UI.

---

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/rotas` | Lista todas as rotas |
| `GET` | `/api/viagens?origem=&destino=&data=` | Busca viagens futuras (filtros opcionais; `data` no horário de Brasília) |
| `GET` | `/api/viagens/{id}` | Detalhes da viagem, com assentos ocupados e vagas restantes |
| `POST` | `/api/reservas` | Cria uma reserva |
| `GET` | `/api/reservas/{codigo}` | Consulta uma reserva — **exige o header `X-Cpf`** com o CPF do titular |
| `DELETE` | `/api/reservas/{codigo}` | Cancela uma reserva — **exige o header `X-Cpf`** com o CPF do titular |

Exemplos de requisição prontos estão em `backend/src/OnibusExpress.Api/OnibusExpress.Api.http`.

---

## Como rodar a aplicação localmente

### Pré-requisitos Gerais
* Git instalado para clonagem do repositório.

---

### Opção 1: Execução com Docker (Recomendado)
Esta é a maneira mais rápida e isolada. Não é necessário ter o SDK do .NET ou o PostgreSQL instalados na máquina hospedeira.

**Pré-requisitos adicionais:** Docker e Docker Compose instalados e em execução.

**Passo a Passo:**

1. Abra o terminal na pasta raiz do projeto (onde está localizado o arquivo `docker-compose.yml`).

2. Crie o arquivo `.env` a partir do modelo e defina uma senha em `POSTGRES_PASSWORD`:

   ```bash
   cp .env.example .env
   ```

   > A senha é obrigatória: se `POSTGRES_PASSWORD` ficar vazia, o container do PostgreSQL não inicia.

3. Execute o comando para compilar as imagens e subir os serviços:

   ```bash
   docker compose up --build
   ```

4. Aguarde o terminal indicar que a API iniciou (`Application started.`). As migrations e os dados iniciais (rotas e viagens de exemplo) são criados automaticamente nesta etapa.

5. Acesse a documentação interativa e teste os endpoints no navegador:
   http://localhost:5277/swagger

---

### Opção 2: Execução sem Docker (Modo Local)
Caso queira debugar o código fonte diretamente no ambiente de desenvolvimento local.

**Pré-requisitos adicionais:** SDK do .NET 10 instalado e um servidor PostgreSQL ativo (porta padrão 5432).

**Passo a Passo:**

1. A senha do banco não fica versionada no `appsettings.json`. Configure a string de conexão via user-secrets (na pasta `backend/src/OnibusExpress.Api`):

   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=onibus_express;Username=postgres;Password=SUA_SENHA"
   ```

2. Na pasta `backend` (onde está o arquivo de solução `OnibusExpress.slnx`), inicie a aplicação:

   ```bash
   dotnet run --project src/OnibusExpress.Api
   ```

   As migrations e os dados iniciais são aplicados automaticamente na inicialização.

3. Acesse o Swagger no navegador: http://localhost:5277/swagger

---

## Como rodar os testes automatizados

A suíte de testes valida as regras de negócio, os services, os controllers e o tratamento de erros sem depender do banco de dados físico, garantindo execuções rápidas e isoladas. Os mesmos testes rodam automaticamente no GitHub Actions a cada pull request.

Na pasta `backend` (onde está o arquivo `OnibusExpress.slnx`), execute:

```bash
dotnet test
```
