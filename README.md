# Baby Turismo - Fleet Management System

Sistema profissional de gestão de frota para empresas de transporte.

![Baby Turismo](https://img.shields.io/badge/status-production-green)
![License](https://img.shields.io/badge/license-proprietary-red)

## Sobre o Projeto

Baby Turismo é uma plataforma web moderna desenvolvida para centralizar toda a operação de empresas de transporte em um único ambiente. O sistema elimina planilhas, reduz falhas operacionais, automatiza processos e fornece indicadores estratégicos em tempo real.

### Swagger API

Documentação interativa completa com **17 módulos** e **100+ endpoints**:

![BabyTurismo Swagger Overview](docs/swagger-endpoints.png)

*Visão geral dos módulos: Auth, Drivers, Vehicles, Trips, Finance, Stock, Maintenances, FuelLogs, Dashboard e mais.*

## Principais Funcionalidades

- **Gestão de Motoristas** - Cadastro, CNH, disponibilidade e histórico
- **Gestão de Veículos** - Frota, documentação, alertas de combustível
- **Controle de Viagens** - Agenda, checklists, observações operacionais
- **Gestão Financeira** - Receitas, despesas, fluxo de caixa, centro de custos
- **Controle de Estoque** - Produtos, movimentações, fornecedores
- **Manutenções** - Preventivas e corretivas com histórico
- **Abastecimentos** - Registro de combustível e consumo médio
- **Dashboards Analíticos** - KPIs personalizáveis com gráficos interativos
- **Sistema Multi-Tenant** - Isolamento completo entre empresas
- **Auditoria Completa** - Rastreamento de todas as operações

## Stack Tecnológica

### Backend
- **ASP.NET Core 9** com C#
- **Entity Framework Core** para ORM
- **PostgreSQL** (Supabase) como banco de dados
- **Redis** para cache
- **Clean Architecture** com Domain-Driven Design
- **CQRS** com MediatR
- **JWT** para autenticação
- **FluentValidation** para validações
- **Serilog** para logs estruturados

### Frontend
- **React 18** com TypeScript
- **Vite** como build tool
- **Tailwind CSS** + **shadcn/ui** para UI
- **TanStack Query** para data fetching
- **React Hook Form** + **Zod** para formulários
- **Apache ECharts** + **Recharts** para gráficos
- **React Grid Layout** para dashboards personalizáveis
- **Framer Motion** para animações

### Infraestrutura
- **Docker** + **Docker Compose** para containers
- **Nginx** como reverse proxy
- **Supabase Storage** para arquivos

## Arquitetura

O projeto segue os princípios de **Clean Architecture** com **Domain-Driven Design**:

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│      (React + TypeScript)               │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│           API Layer                     │
│    (ASP.NET Core + JWT + RBAC)          │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│       Application Layer                 │
│    (CQRS + MediatR + Validators)        │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│         Domain Layer                    │
│  (Entities + Value Objects + Events)    │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│      Infrastructure Layer               │
│  (EF Core + Redis + Storage + External) │
└─────────────────────────────────────────┘
```

### API Endpoints

![BabyTurismo Swagger Endpoints](docs/swagger-overview.png)

*API documentada com Swagger/OpenAPI. Cada endpoint possui request/response models, validações e exemplos.*

## Estrutura do Projeto

```
BabyC/
├── backend/                    # API .NET
│   ├── src/
│   │   ├── BabyTurismo.Api/           # Controllers, Middleware, Services
│   │   ├── BabyTurismo.Application/   # Commands, Queries, Handlers
│   │   ├── BabyTurismo.Domain/        # Entities, Value Objects, Events
│   │   ├── BabyTurismo.Infrastructure/# EF Core, Redis, External Services
│   │   └── BabyTurismo.Shared/        # DTOs, Results, Common
│   └── tests/
│       └── BabyTurismo.Tests/         # Unit Tests (xUnit)
├── frontend/                   # React Application
│   └── src/
│       ├── components/         # UI Components
│       ├── pages/              # Page Components
│       ├── services/           # API Services
│       ├── store/              # State Management
│       ├── hooks/              # Custom Hooks
│       └── types/              # TypeScript Types
├── nginx/                      # Nginx Configuration
├── docker-compose.yml          # Docker Orchestration
└── .env.example                # Environment Variables Template
```

## Pré-requisitos

- Docker e Docker Compose
- .NET 9 SDK (para desenvolvimento)
- Node.js 18+ (para desenvolvimento)

## Instalação e Execução

### 1. Configurar Variáveis de Ambiente

```bash
cp .env.example .env
```

Edite o arquivo `.env` com suas configurações:
- Database credentials (Supabase PostgreSQL)
- JWT secrets
- Redis connection
- CORS origins

### 2. Executar com Docker

```bash
docker-compose up -d
```

O sistema estará disponível em:
- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger — documentação interativa para testar todos os endpoints

### 3. Executar em Modo Desenvolvimento

**Backend:**
```bash
cd backend
dotnet run
```

**Frontend:**
```bash
cd frontend
npm install
npm run dev
```

## Testes

O projeto possui uma suite completa de testes unitários:

```bash
cd backend
dotnet test
```

### Cobertura de Testes

- **Auth** - Login, Logout, Refresh Token
- **Drivers** - Criação, Atualização, Disponibilidade
- **Vehicles** - Criação, Atribuição, Alertas
- **Trips** - Criação, Início, Conclusão, Cancelamento

## Módulos do Sistema

### Core
- Usuários e Autenticação
- Permissões (RBAC)
- Auditoria
- Multi-Tenancy

### Operacional
- Motoristas
- Veículos
- Agenda
- Viagens
- Checklists

### Frota
- Manutenções
- Abastecimentos
- Documentos
- Alertas de Combustível

### Financeiro
- Receitas e Despesas
- Fluxo de Caixa
- Centro de Custos
- Categorias

### Estoque
- Produtos
- Movimentações
- Fornecedores

### Analytics
- Dashboards Personalizáveis
- KPIs
- Relatórios
- Exportação (PDF, Excel, CSV)

## Segurança

- **JWT Authentication** com Refresh Tokens
- **RBAC** (Role-Based Access Control)
- **Multi-tenant isolation** completo
- **Audit logs** de todas as operações
- **Rate limiting** na API
- **CORS** configurável
- **BCrypt** para senhas
- **HTTPS** obrigatório em produção

## Integrações Futuras

- Google Maps API
- WhatsApp Business
- Push Notifications
- OCR para documentos
- Aplicativo Mobile
- GPS e Telemetria

## Deploy

### Produção

O sistema está configurado para deploy em:
- **Frontend**: Vercel
- **Backend**: Render
- **Database**: Supabase
- **Storage**: Supabase Storage

Consulte o arquivo `.env.example` para as variáveis de ambiente necessárias.

## Licença

Projeto proprietário. Todos os direitos reservados.

## Contato

Para mais informações sobre o projeto, entre em contato.

---

**Desenvolvido com foco em qualidade, escalabilidade e manutenibilidade.**
