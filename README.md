# 📈 Compra Programada de Ações — Itaú Corretora

Desafio técnico desenvolvido para a Itaú Corretora.

O sistema permite que clientes definam um valor mensal de investimento e recebam ações automaticamente três vezes por mês — nos dias **5, 15 e 25** — sem precisar acompanhar o mercado. As compras são feitas de forma consolidada em uma conta master e distribuídas proporcionalmente para cada cliente.

---

## 🧱 Stack

| | |
|---|---|
| **Linguagem** | C# .NET 8 |
| **Framework** | ASP.NET Core 8 |
| **Banco** | MySQL 8 + Entity Framework Core (Pomelo) |
| **Mensageria** | Apache Kafka |
| **Agendamento** | .NET BackgroundService + Cronos |
| **Testes** | xUnit · Moq · FluentAssertions |
| **Documentação** | Swagger / OpenAPI com anotações |

---

## 🗂️ Estrutura do Projeto

```
├── TesteItauCorretora/                  → API: controllers, middlewares, Program.cs
├── TesteItauCorretora.Core/             → Domínio: entidades, regras, interfaces, DTOs
├── TesteItauCorretora.Infrastructure/   → Banco, Kafka, repositórios
└── TesteItauCorretora.Teste/            → Testes unitários
```

A camada **Core** não tem dependência de nenhuma outra camada do projeto — ela é o coração do sistema e pode ser testada de forma completamente isolada.

---

## 🔧 Por que essas escolhas?

**Clean Architecture**
O motor de compra concentra diversas regras interdependentes — cálculo de proporções, truncamento, resíduos, lote padrão vs fracionário, IR. Isolar isso em um Core independente facilita testar cada regra individualmente e garante que nenhuma dependência de infraestrutura "vaze" para dentro do domínio.

**Kafka**
Os eventos de IR são publicados em **lote por ativo** usando `Task.WhenAll`, evitando múltiplas chamadas de rede na mesma execução. O publisher detecta automaticamente se deve usar autenticação SASL (Confluent Cloud) ou conexão simples (Kafka local via Docker) com base na presença da `ApiKey` no `appsettings.json`.

**BackgroundService com Cronos**
O motor roda automaticamente via `BackgroundService` do próprio .NET, sem dependência de biblioteca externa de scheduling. A expressão cron é configurável no `appsettings.json` — basta alterar o horário sem recompilar. O próprio motor valida internamente se a data é dia de execução e ajusta para o próximo dia útil quando necessário.

---

## ✅ O que foi implementado

- Adesão de clientes com validação de CPF, e-mail e valor mínimo de R$ 100,00
- Saída do plano com histórico preservado
- Alteração de valor mensal com registro de histórico
- Cadastro e validação da cesta Top Five (exatamente 5 ativos, soma = 100%)
- Motor de compra com execução automática nos dias 5, 15 e 25
- Ajuste automático para próximo dia útil quando o dia cai em fim de semana
- Consolidação dos aportes e distribuição proporcional para custódias filhotes
- Separação automática de ordens em lote padrão (≥ 100 ações) e mercado fracionário
- Controle de resíduos na conta master entre execuções
- Cálculo e publicação do IR dedo-duro (0,005%) no Kafka a cada distribuição
- Rebalanceamento por troca de cesta ou desvio de proporção
- Consulta de carteira com preço médio por ativo
- Consulta de rentabilidade com P&L por cliente
- Header `X-Request-Id` em todas as respostas via middleware

---

## 🚀 Como rodar

### Pré-requisitos

- .NET 8 SDK instalado
- Docker e Docker Compose instalados
- Arquivo COTAHIST da B3 (`.TXT` diário)

### 1. Arquivo COTAHIST — obrigatório

Baixe o arquivo de cotações históricas da B3 e coloque na pasta configurada:

👉 [https://www.b3.com.br/pt_br/market-data-e-indices/servicos-de-dados/market-data/historico/mercado-a-vista/cotacoes-historicas/](https://www.b3.com.br/pt_br/market-data-e-indices/servicos-de-dados/market-data/historico/mercado-a-vista/cotacoes-historicas/)

Após baixar, coloque o arquivo em `C:\Cotahist` (ou ajuste o caminho no `appsettings.json`).

### 2. Subir a infraestrutura com Docker

Na raiz do projeto, execute:

```bash
docker compose up -d
```

Isso sobe o **MySQL**, o **Kafka**, o **Zookeeper** e o **Kafka UI**.

| Serviço | URL |
|---|---|
| API + Swagger (HTTPS) | `https://localhost:7281/swagger` |
| API + Swagger (HTTP) | `http://localhost:5207/swagger` |
| Kafka UI | `http://localhost:8080` |
| MySQL | `localhost:3306` |

### 3. Configurar o appsettings

Substitua o conteúdo do `appsettings.json` pelo arquivo `appsettings.Docker.json` que está na raiz do projeto. Ele já está configurado para apontar para o Kafka e MySQL locais:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=TesteItauCorretora;user=root;password=root"
  },
  "Paths": {
    "CotahistPath": "C:\\Cotahist"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "ApiKey": "",
    "ApiSecret": "",
    "TopicoIR": "itau-corretora-eventos-ir"
  },
  "MotorCompra": {
    "Enabled": true,
    "CronExpression": "0 0 10 * * *"
  }
}
```

> Quando `ApiKey` está vazio, o sistema conecta no Kafka local sem autenticação automaticamente.

### 4. Rodar a API

```bash
dotnet run --project TesteItauCorretora
```

Swagger disponível em `https://localhost:7281/swagger` assim que a aplicação subir.

### 5. Visualizar mensagens do Kafka

Acesse o **Kafka UI** em `http://localhost:8080`, vá em **Topics → itau-corretora-eventos-ir → Messages** para visualizar os eventos de IR publicados a cada execução do motor.

---

## 🧪 Testes

```bash
dotnet test
```

**164 testes** cobrindo:

- Validações de domínio nas entidades `Cliente`, `CestaRecomendacao` e `ContaGrafica`
- Todas as regras do motor de compra (datas, truncamento, lotes, distribuição, resíduos, IR)
- Comportamento dos repositórios via Moq
- Cenários de erro e exceções esperadas

---

## 📨 Kafka — Eventos de IR

| Tópico | Gatilho |
|---|---|
| `itau-corretora-eventos-ir` | Publicado a cada distribuição de ações para um cliente |

Payload da mensagem:

```json
{
  "clienteId": 1,
  "cpf": "12345678901",
  "ticker": "PETR4",
  "quantidade": 8,
  "valorOperacao": 280.00,
  "valorIR": 0.01,
  "tipoEvento": "Compra",
  "dataEvento": "2026-03-05T10:00:00"
}
```

---

## 🔄 Fluxo do Motor de Compra

```
Dia 5, 15 ou 25
       ↓
É fim de semana? → Avança para segunda-feira
       ↓
Busca clientes ativos e soma aportes (ValorMensal ÷ 3)
       ↓
Busca cesta ativa e cotações do COTAHIST
       ↓
Para cada ativo:
  → Calcula valor proporcional
  → TRUNCAR(valor ÷ cotação) = quantidade desejada
  → Desconta resíduo da master da compra anterior
  → Separa em lote padrão (≥100) + fracionário
  → Distribui proporcionalmente para cada filhote
  → Publica IR no Kafka em lote
  → Registra resíduo restante na master
```

---

## 📋 Endpoints

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/clientes/adesao` | Adesão de novo cliente |
| `POST` | `/api/clientes/{id}/saida` | Saída do plano |
| `PUT` | `/api/clientes/{id}/valor-mensal` | Alterar valor mensal |
| `GET` | `/api/clientes/{id}/carteira` | Posição atual da carteira |
| `GET` | `/api/clientes/{id}/rentabilidade` | Rentabilidade e P&L |
| `POST` | `/api/admin/cesta` | Cadastrar nova cesta Top Five |
| `POST` | `/api/admin/motor/executar` | Disparar motor manualmente |

Documentação interativa completa no Swagger em `https://localhost:7281/swagger`.
