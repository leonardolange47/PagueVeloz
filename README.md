# 💳 PagueVeloz Transaction Processor

## 📌 Sobre o Projeto
O **PagueVeloz Transaction Processor** é um sistema de **processamento de transações financeiras** projetado para ambientes de **alta disponibilidade**, **concorrência** e **resiliência**, simulando uma plataforma de adquirência de pagamentos.  

O sistema suporta:
- Operações financeiras (`credit`, `debit`, `reserve`, `capture`, `reversal`, `transfer`).
- Idempotência via `reference_id`.
- Controle de concorrência com **locks**.
- Arquitetura limpa, com camadas bem definidas.
- Testes **unitários e de integração**.
- API REST com documentação via Swagger.
- Suporte a **Entity Framework Core com SQL Server** e **InMemory** para testes.

---

## 📂 Estrutura da Solução

| Projeto                                | Tipo                      | Responsabilidade                                                                 |
|--------------------------------------|--------------------------|---------------------------------------------------------------------------------|
| `PagueVeloz.Domain`                   | Class Library (.NET 9)   | Entidades, enums, exceções, interfaces de repositórios e serviços.              |
| `PagueVeloz.Application`              | Class Library (.NET 9)   | Implementações de serviços de domínio, lógica de negócio.                       |
| `PagueVeloz.Infrastructure`           | Class Library (.NET 9)   | Persistência (EF Core), repositórios, contexto de banco de dados.              |
| `PagueVeloz.Api`                      | Web API (.NET 9)         | API REST, controllers, Swagger.                                                |
| `PagueVeloz.TransactionProcessor`     | Console App (.NET 9)     | Interface CLI para processamento de transações.                                |
| `PagueVeloz.Tests.Unit`               | xUnit                    | Testes unitários de entidades, serviços e repositórios.                        |
| `PagueVeloz.Tests.Integration`        | xUnit                    | Testes de integração com banco InMemory e `CustomWebApplicationFactory`.       |

Frameworks
| Projeto                  | Pacotes adicionais principais                                 | Justificativa                                       |
| ------------------------ | ------------------------------------------------------------- | --------------------------------------------------- |
| **Domain**               | ❌ Nenhum                                                     | Mantém camada pura.                                 |
| **Application**          | ❌ Nenhum                                                     | Só implementa regras de negócio.                    |
| **Infrastructure**       | `EF Core`, `SqlServer`, `Design`, `Tools`                     | Persistência e migrations.                          |
| **Api**                  | `Swashbuckle.AspNetCore`, `Configuration.Json`, `EF Core`     | API REST com Swagger e configuração.                |
| **TransactionProcessor** | ❌ Nenhum                                                     | CLI sem dependências externas.                      |
| **Tests.Unit**           | `xUnit`, `Moq`, `FluentAssertions`                            | Testes isolados.                                    |
| **Tests.Integration**    | `xUnit`, `FluentAssertions`, `Mvc.Testing`, `EFCore.InMemory` | Testes de integração com servidor e banco InMemory. |


---

## 🏗️ Arquitetura
┌────────────────────────────┐
│ Presentation │ -> API (Controllers) e CLI
├────────────────────────────┤
│ Application │ -> Casos de uso e lógica de negócio
├────────────────────────────┤
│ Domain │ -> Entidades, Interfaces, Regras de negócio
├────────────────────────────┤
│ Infrastructure │ -> EF Core, Repositórios, Persistência
└────────────────────────────┘

yaml


---

## ⚙️ Tecnologias
- **.NET 9**
- **Entity Framework Core** (SQL Server, InMemory)
- **Swagger/OpenAPI**
- **xUnit** (testes unitários e integração)
- **Moq** (mock de dependências)
- **FluentAssertions**
- **Docker (opcional)** para rodar o ambiente

---

## 🚀 Configuração do Banco de Dados
Arquivo `appsettings.json` na API:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=LEONARDO-LANGE\\SQLEXPRESS;Database=PagueVelozDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
▶️ Executando o Projeto
Restaurar pacotes e compilar:

bash

dotnet build
Aplicar Migrations:

bash

dotnet ef migrations add InitialCreate -p PagueVeloz.Infrastructure -s PagueVeloz.Api
dotnet ef database update -p PagueVeloz.Infrastructure -s PagueVeloz.Api
Rodar API:

bash

dotnet run --project PagueVeloz.Api
Acesse o Swagger em: http://localhost:5000

Rodar CLI:

bash

dotnet run --project PagueVeloz.TransactionProcessor
Rodar testes:

bash

dotnet test
📜 Endpoints Principais
Método	Rota	Descrição
POST	/api/customer	Cria um cliente
GET	/api/customer/{id}	Consulta cliente por ID
POST	/api/accounts	Cria uma conta
GET	/api/accounts/{id}	Consulta conta por ID
POST	/api/transactions	Cria uma transação única
POST	/api/transactions/batch	Cria várias transações de uma só vez
GET	/api/transactions/{id}	Lista transações por cliente

📖 Documentação com Swagger
Com a API rodando (dotnet run --project PagueVeloz.Api), acesse:

🔗 Swagger UI:

bash

http://localhost:5000/swagger
🔹 Lá você pode:

Executar todas as operações.

Preencher JSONs diretamente.

Validar retornos sem precisar de cURL.

🖥️ Testes Manuais com cURL
1️⃣ Criar Cliente
bash

curl -X POST http://localhost:5000/api/customer \
-H "Content-Type: application/json" \
-d "\"Cliente Teste\""
2️⃣ Criar Conta
bash

curl -X POST http://localhost:5000/api/accounts \
-H "Content-Type: application/json" \
-d '{
  "customerId": "GUID_CLIENTE",
  "creditLimit": 500
}'
3️⃣ Crédito na Conta
bash

curl -X POST http://localhost:5000/api/transactions \
-H "Content-Type: application/json" \
-d '{
  "accountId": "GUID_CONTA",
  "type": 0,
  "amount": 1000,
  "currency": "BRL",
  "metadata": "Depósito inicial"
}'
4️⃣ Débito com Saldo Suficiente
bash

curl -X POST http://localhost:5000/api/transactions \
-H "Content-Type: application/json" \
-d '{
  "accountId": "GUID_CONTA",
  "type": 1,
  "amount": 200,
  "currency": "BRL",
  "metadata": "Compra"
}'
5️⃣ Débito com Limite de Crédito
bash

curl -X POST http://localhost:5000/api/transactions \
-H "Content-Type: application/json" \
-d '{
  "accountId": "GUID_CONTA",
  "type": 1,
  "amount": 1200,
  "currency": "BRL",
  "metadata": "Compra com limite"
}'
6️⃣ Débito Excedendo Limite (Erro)
bash

curl -X POST http://localhost:5000/api/transactions \
-H "Content-Type: application/json" \
-d '{
  "accountId": "GUID_CONTA",
  "type": 1,
  "amount": 2000,
  "currency": "BRL",
  "metadata": "Compra inválida"
}'
7️⃣ Reserva de Saldo
bash

curl -X POST http://localhost:5000/api/transactions \
-H "Content-Type: application/json" \
-d '{
  "accountId": "GUID_CONTA",
  "type": 2,
  "amount": 100,
  "currency": "BRL",
  "metadata": "Reserva para pagamento"
}'
8️⃣ Captura de Saldo Reservado
bash

curl -X POST http://localhost:5000/api/transactions \
-H "Content-Type: application/json" \
-d '{
  "accountId": "GUID_CONTA",
  "type": 3,
  "amount": 100,
  "currency": "BRL",
  "metadata": "Captura reserva"
}'
9️⃣ Estorno de Transação
bash

curl -X POST http://localhost:5000/api/transactions \
-H "Content-Type: application/json" \
-d '{
  "accountId": "GUID_CONTA",
  "type": 4,
  "amount": 100,
  "currency": "BRL",
  "metadata": "Estorno"
}'
🔟 Transferência entre Contas
bash

curl -X POST http://localhost:5000/api/transactions \
-H "Content-Type: application/json" \
-d '{
  "accountId": "GUID_CONTA",
  "type": 5,
  "amount": 200,
  "currency": "BRL",
  "metadata": "Transferência para conta destino",
  "targetAccountId": "GUID_CONTA_DESTINO"
}'
1️⃣1️⃣ Batch de Múltiplas Transações
bash

curl -X POST http://localhost:5000/api/transactions/batch \
-H "Content-Type: application/json" \
-d '[
  {
    "accountId": "GUID_CONTA",
    "type": 0,
    "amount": 100,
    "currency": "BRL",
    "metadata": "Crédito 1"
  },
  {
    "accountId": "GUID_CONTA",
    "type": 0,
    "amount": 50,
    "currency": "BRL",
    "metadata": "Crédito 2"
  }
]'
1️⃣2️⃣ Consultar Conta
bash

curl -X GET http://localhost:5000/api/accounts/GUID_CONTA
1️⃣3️⃣ Consultar Transações por Cliente
bash

curl -X GET http://localhost:5000/api/transactions/GUID_CLIENTE
🐳 Docker (Opcional)
yaml
version: '3.4'
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "YourStrong!Passw0rd"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
yaml

---
