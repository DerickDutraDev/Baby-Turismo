# Baby Turismo

Sistema profissional de gestão de frota para empresas de transporte rodoviário de passageiros.

![Status](https://img.shields.io/badge/status-production-green)
![License](https://img.shields.io/badge/license-proprietary-red)

## Sobre

Baby Turismo é uma plataforma SaaS multi-tenant desenvolvida para centralizar toda a operação de empresas de transporte em um único ambiente. O sistema elimina planilhas, reduz falhas operacionais, automatiza processos e fornece indicadores estratégicos em tempo real.

### Público-alvo

Empresas de transporte rodoviário de passageiros que precisam gerenciar frota, motoristas, viagens, manutenções e operações financeiras de forma integrada.

## Funcionalidades

### Gestão Operacional
- **Motoristas** - Cadastro, CNH, disponibilidade, histórico de viagens
- **Veículos** - Frota, documentação, vencimentos, alertas automáticos
- **Viagens** - Agenda, checklists operacionais, observações, status em tempo real
- **Manutenções** - Preventivas e corretivas com histórico completo
- **Abastecimentos** - Registro de combustível, consumo médio por veículo

### Gestão Financeira
- **Receitas e Despesas** - Lançamentos categorizados
- **Fluxo de Caixa** - Visão consolidada por período
- **Centro de Custos** - Rateio e acompanhamento por unidade
- **Fechamento Mensal** - Controle de meses fiscais

### Gestão de Estoque
- **Produtos** - Cadastro e categorização
- **Movimentações** - Entradas, saídas e transferências
- **Saldo** - Controle de estoque em tempo real

### Analytics
- **Dashboards** - KPIs personalizáveis com gráficos interativos
- **Relatórios** - Exportação em PDF, Excel e CSV
- **Indicadores** - Métricas operacionais e financeiras

### Segurança e Infraestrutura
- **Multi-Tenant** - Isolamento completo entre empresas
- **RBAC** - Controle de acesso baseado em perfis (Admin, Gestor, Motorista)
- **Auditoria** - Rastreamento completo de todas as operações
- **Autenticação** - JWT com Refresh Tokens

## Stack Tecnológica

### Backend
- ASP.NET Core 10 (C#)
- Entity Framework Core + PostgreSQL
- Clean Architecture + Domain-Driven Design
- CQRS com MediatR
- Redis para cache
- FluentValidation
- Serilog para logs estruturados

### Frontend
- React 18 + TypeScript
- Vite
- Tailwind CSS + shadcn/ui
- TanStack Query
- Apache ECharts + Recharts

### Infraestrutura
- Docker + Docker Compose
- Nginx (reverse proxy)
- Supabase (PostgreSQL + Storage)

## Arquitetura

O projeto segue **Clean Architecture** com **Domain-Driven Design**:

```
─────────────────────────────────────────┐
│         Presentation Layer              │
│      (React + TypeScript)               │
└────────────────┬────────────────────────┘
                 │
────────────────▼────────────────────────┐
│           API Layer                     │
│    (ASP.NET Core + JWT + RBAC)          │
└────────────────┬────────────────────────┘
                 │
────────────────▼────────────────────────┐
│       Application Layer                 │
│    (CQRS + MediatR + Validators)        │
└────────────────┬────────────────────────┘
                 │
────────────────▼────────────────────────┐
│         Domain Layer                    │
│  (Entities + Value Objects + Events)    │
────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────
│      Infrastructure Layer               │
│  (EF Core + Redis + Storage + External) │
─────────────────────────────────────────┘
```

## Estrutura do Projeto

```
Baby-Turismo/
├── backend/
│   ├── src/
│   │   ├── BabyTurismo.Api/           # Controllers, Middleware, Services
│   │   ├── BabyTurismo.Application/   # Commands, Queries, Handlers
│   │   ├── BabyTurismo.Domain/        # Entities, Value Objects, Events
│   │   ├── BabyTurismo.Infrastructure/# EF Core, Redis, External Services
│   │   └── BabyTurismo.Shared/        # DTOs, Results, Common
│   └── tests/
│       └── BabyTurismo.Tests/         # Unit Tests (xUnit)
├── frontend/
│   └── src/
│       ├── components/                # UI Components
│       ├── pages/                     # Page Components
│       ├── services/                  # API Services
│       ├── store/                     # State Management
│       ├── hooks/                     # Custom Hooks
│       └── types/                     # TypeScript Types
├── nginx/                             # Nginx Configuration
└── docker-compose.yml                 # Docker Orchestration
```

## Execução Local

### Pré-requisitos
- Docker e Docker Compose
- .NET 10 SDK (desenvolvimento)
- Node.js 22+ (desenvolvimento)

### Setup

```bash
# 1. Clonar o repositório
git clone https://github.com/DerickDutraDev/Baby-Turismo.git
cd Baby-Turismo

# 2. Configurar variáveis de ambiente
cp .env.example .env
# Edite .env com suas configurações (database, JWT, Redis)

# 3. Executar com Docker
docker compose up -d
```

### Endpoints
- **Frontend**: http://localhost
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **PostgreSQL**: localhost:5432
- **Redis**: localhost:6379

### Desenvolvimento

```bash
# Backend
cd backend
dotnet restore
dotnet run

# Frontend
cd frontend
npm install
npm run dev
```

### Testes

```bash
cd backend
dotnet test
```

**Cobertura:**
- Autenticação (Login, Logout, Refresh Token)
- Motoristas (CRUD, Disponibilidade)
- Veículos (CRUD, Atribuição)
- Viagens (Criação, Início, Conclusão, Cancelamento)

## Módulos do Sistema

| Módulo | Descrição |
|--------|-----------|
| Auth | Autenticação JWT, RBAC, Refresh Tokens |
| Drivers | Gestão de motoristas, CNH, disponibilidade |
| Vehicles | Frota, documentação, alertas |
| Trips | Viagens, checklists, agenda |
| Finance | Receitas, despesas, fluxo de caixa |
| Inventory | Produtos, movimentações, estoque |
| Maintenances | Manutenções preventivas/corretivas |
| FuelLogs | Abastecimentos, consumo médio |
| Dashboard | KPIs, gráficos, relatórios |

## Deploy

### Produção
- **Frontend**: Vercel
- **Backend**: Render
- **Database**: Supabase PostgreSQL
- **Storage**: Supabase Storage

### Variáveis de Ambiente

Consulte `.env.example` para a lista completa de variáveis necessárias.

## Integrações Futuras

- Google Maps API
- WhatsApp Business
- Push Notifications
- OCR para documentos
- Aplicativo Mobile
- GPS e Telemetria

## Licença

Projeto proprietário. Todos os direitos reservados.

## Autor

**Derick Dutra**
- GitHub: [@DerickDutraDev](https://github.com/DerickDutraDev)

---

**Desenvolvido com foco em qualidade, escalabilidade e manutenibilidade.**
