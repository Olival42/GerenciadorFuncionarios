# Especificação Técnica da Arquitetura — AquaGás Distribuidora

## 1. Visão Geral da Arquitetura

### 1.1 Padrões e Princípios Adotados

O projeto utiliza **Domain-Driven Design (DDD)** combinado com **Clean Architecture**. Essa abordagem garante:

- Separação clara de responsabilidades entre camadas
- Regras de negócio centralizadas na camada de domínio
- Independência de frameworks e tecnologias externas
- Isolamento de complexidade técnica nos limites da aplicação
- Modularização vertical por contextos de negócio

### 1.2 Estrutura de Camadas

A aplicação é organizada em quatro camadas hierárquicas onde o fluxo de dependências é unidirecional:

#### **Camada de Apresentação (Web/API)**
- **Responsabilidade:** Receber requisições HTTP, validar entrada, autenticar e autorizar, encaminhar para casos de uso
- **Conteúdo Permitido:** Controllers, DTOs de request/response, filtros de autorização
- **Conteúdo Proibido:** Lógica de negócio, acesso a repositórios, operações de persistência
- **Dependências:** Application (interfaces de casos de uso), Shared (DTOs, enums comuns)

#### **Camada de Aplicação (Application)**
- **Responsabilidade:** Orquestração de fluxos de negócio, coordenação de serviços de domínio e repositórios
- **Conteúdo Permitido:** Casos de uso (use cases), serviços de aplicação, mapeadores (mappers)
- **Conteúdo Proibido:** Lógica de domínio complexa, implementações de persistência, acesso direto a database
- **Dependências:** Domain (abstrações e entidades), Infrastructure (via interfaces injetadas)

#### **Camada de Domínio (Domain)**
- **Responsabilidade:** Encapsular regras de negócio, representar modelos do domínio, garantir invariantes
- **Conteúdo Permitido:** Entidades, value objects, aggregates, specificationes, exceções de domínio, abstrações de repositórios
- **Conteúdo Proibido:** Implementações de repositórios, bibliotecas externas, lógica de infraestrutura, detalhes técnicos
- **Dependências:** Nenhuma (camada independente)

#### **Camada de Infraestrutura (Infrastructure)**
- **Responsabilidade:** Implementar abstrações de domínio/aplicação, gerenciar persistência e dependências externas
- **Conteúdo Permitido:** Implementações de repositórios, contexto do banco de dados (DbContext), cache, segurança, injeção de dependência
- **Conteúdo Proibido:** Lógica de negócio, regras de domínio complexas
- **Dependências:** Domain (interfaces abstratas), bibliotecas externas (EF Core, Redis, JWT)

### 1.3 Fluxo de Dependências (Dependency Rule)

```
Web → Application → Domain ← Infrastructure
```

**Regras Obrigatórias:**
- Web depende de Application (nunca de Domain ou Infrastructure diretamente)
- Application depende de Domain e Infrastructure (via interfaces)
- Domain é independente (camada central)
- Infrastructure implementa interfaces definidas em Domain/Application (nunca o inverso)
- Inversão de Controle (IoC) utiliza injeção de dependência
- Acesso a Infrastructure acontece apenas através de abstrações

### 1.4 Organização de Módulos

Cada módulo é auto-suficiente e organizado verticalmente. A estrutura obrigatória é:

```
src/Modules/<ModuleName>/
├── <ModuleName>.Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Aggregates/
│   ├── Exceptions/
│   ├── Specifications/
│   └── Repositories/          (interfaces abstratas apenas)
│
├── <ModuleName>.Application/
│   ├── UseCases/
│   │   ├── CreateEntity/      (interface, implementação, DTOs)
│   │   ├── UpdateEntity/
│   │   ├── GetEntityById/
│   │   └── ListEntities/
│   ├── Services/
│   ├── Mapping/
│   └── DTOs/
│
├── <ModuleName>.Web/
│   ├── Controllers/
│   └── DTOs/
│
└── <ModuleName>.Infrastructure/
    ├── Repositories/         (implementações concretas)
    ├── Persistence/
    └── Services/
```

**Módulos Obrigatórios:**
- `Auth`: Autenticação e gerenciamento de sessão
- `Funcionario`: Gestão de funcionários e login
- `Produto`: Catálogo de produtos e controle de estoque
- `Cliente`: Cadastro de clientes e histórico
- `Venda`: Vendas avulsas
- `Plano`: Contratos recorrentes e planos de assinatura
- `Shared`: Componentes compartilhados entre módulos (auditoria, DTOs globais, exceções comuns)

---

## 2. Estrutura de Camadas Detalhada

### 2.1 Camada de Domínio (Domain)

#### 2.1.1 Entidades
- **Definição:** Objetos com identidade única que evoluem ao longo do tempo
- **Localização:** `<Module>.Domain/Entities/`
- **Características Obrigatórias:**
  - Possuem `Id` do tipo `Guid`
  - Encapsulam estado e comportamento relacionado
  - Validam invariantes de negócio em construtores e métodos
  - Implementam lógica de domínio complexa
- **Características Proibidas:**
  - Getters e setters públicos sem validação
  - Métodos que realizam operações de persistência
  - Dependências de bibliotecas externas

#### 2.1.2 Value Objects
- **Definição:** Objetos sem identidade que representam conceitos do domínio, comparados por valor
- **Localização:** `<Module>.Domain/ValueObjects/`
- **Características Obrigatórias:**
  - Imutáveis (propriedades readonly após construção)
  - Implementam `IEquatable<T>` para comparação por valor
  - Validam regras de formato e negócio no construtor
  - Encapsulam validação de formato (ex: CPF, email)
- **Value Objects Obrigatórios nos Módulos:**
  - `Auth`: `Password` (validação de força)
  - `Funcionario`: `CPF`, `Phone`
  - `Produto`: `Price`, `Quantity`
  - `Cliente`: `CpfCnpj`
  - `Plano`: `Money`, `ContractPeriod`

#### 2.1.3 Agregados (Aggregates)
- **Definição:** Clusters de entidades e value objects tratados como unidade transacional
- **Localização:** Entidade raiz no diretório `Entities/`, demais componentes em uma subpasta
- **Características Obrigatórias:**
  - Uma única entidade é o raiz (Aggregate Root)
  - Apenas o raiz pode ser referenciado de fora
  - Entidades internas são referenciadas via ID
  - Toda mudança no agregado passa pelo raiz
  - Transações devem ser garantidas no raiz

#### 2.1.4 Repositórios (Abstrações)
- **Definição:** Interfaces que definem contrato de persistência
- **Localização:** `<Module>.Domain/Repositories/`
- **Características Obrigatórias:**
  - Interfaces genéricas (`IRepository<T>`) ou específicas (`IFuncionarioRepository`)
  - Métodos de consulta sem efeitos colaterais (queries)
  - Métodos de persistência (Add, Update, Delete)
  - Métodos de busca por critérios de negócio
  - Nenhuma implementação, apenas contratos
- **Proibido:**
  - Métodos que retornam `IQueryable<T>` (expõe EF Core)
  - Métodos com lógica de persistência
  - Referências a DbContext ou banco de dados

#### 2.1.5 Exceções de Domínio
- **Definição:** Exceções que representam violações de regras de negócio
- **Localização:** `<Module>.Domain/Exceptions/`
- **Nomenclatura:** `<EntityName>Exception` ou `<RuleName>Exception`
- **Características Obrigatórias:**
  - Herdam de `DomainException`
  - Mensagem clara e descritiva
  - Podem conter contexto (ex: ID, valor inválido)
  - Sem informações técnicas (stack trace é para Infrastructure)

#### 2.1.6 Especificações (Specifications)
- **Definição:** Encapsulam critérios de busca de negócio
- **Localização:** `<Module>.Domain/Specifications/`
- **Uso:** Para queries complexas reutilizáveis
- **Exemplo:** `DisponibleProductsSpecification`, `ActiveFuncionariosSpecification`

### 2.2 Camada de Aplicação (Application)

#### 2.2.1 Casos de Uso (Use Cases)
- **Definição:** Orquestram fluxos de negócio, coordenando domínio e infraestrutura
- **Localização:** `<Module>.Application/UseCases/`
- **Estrutura Obrigatória:**
  ```
  UseCases/
  ├── CreateFuncionario/
  │   ├── ICreateFuncionarioUseCase.cs       (contrato)
  │   ├── CreateFuncionarioUseCase.cs        (implementação)
  │   ├── CreateFuncionarioRequest.cs        (DTO de entrada)
  │   └── CreateFuncionarioResponse.cs       (DTO de saída)
  ├── UpdateFuncionario/
  │   ├── IUpdateFuncionarioUseCase.cs
  │   ├── UpdateFuncionarioUseCase.cs
  │   ├── UpdateFuncionarioRequest.cs
  │   └── UpdateFuncionarioResponse.cs
  ├── GetFuncionarioById/
  │   ├── IGetFuncionarioByIdUseCase.cs
  │   ├── GetFuncionarioByIdUseCase.cs
  │   ├── GetFuncionarioByIdRequest.cs
  │   └── GetFuncionarioByIdResponse.cs
  └── ListFuncionarios/
      ├── IListFuncionariosUseCase.cs
      ├── ListFuncionariosUseCase.cs
      ├── ListFuncionariosRequest.cs
      └── ListFuncionariosResponse.cs
  ```
- **Características Obrigatórias:**
  - Cada caso de uso possui uma interface dedicada (`I<CaseOfUse>`)
  - Implementação simples em classe separada
  - Método principal: `ExecuteAsync(Request request, CancellationToken ct): Task<Response>`
  - Método pode aceitar apenas o request ou ambos request e parâmetros opcionais
  - Responsabilidades: validação, orquestração, persistência, auditoria
- **Proibido:**
  - Lógica de domínio complexa (deve estar em Domain)
  - Acesso direto a DbContext (usar repositórios)
  - Retornar entidades de domínio diretamente (usar DTOs)
  - Padrão CQRS (sem Command/Query separação)

#### 2.2.2 DTOs (Data Transfer Objects)
- **Definição:** Estruturas simples para transferência de dados entre camadas
- **Localização:** `<Module>.Application/DTOs/` ou dentro de cada casos de uso
- **Características Obrigatórias:**
  - Estruturas simples com propriedades públicas
  - Sem lógica de negócio
  - Tipos primitivos ou outros DTOs
  - Nomenclatura: `<Entity><Direction>Dto` (ex: `FuncionarioCreateDto`, `FuncionarioResponseDto`)
- **Tipos Obrigatórios:**
  - `...CreateRequest`: Entrada para criação
  - `...UpdateRequest`: Entrada para atualização
  - `...Response`: Saída de operações
  - `...ListResponse`: Retorno de listas

#### 2.2.3 Serviços de Aplicação
- **Definição:** Coordenam múltiplos repositórios ou casos de uso complexos
- **Localização:** `<Module>.Application/Services/`
- **Uso:** Apenas quando lógica envolve múltiplos agregados
- **Características:** Injetam repositórios, coordenam transações

#### 2.2.4 Mapeadores (Mappers)
- **Definição:** Convertem entre entidades de domínio e DTOs
- **Ferramenta Obrigatória:** AutoMapper
- **Localização:** `<Module>.Application/Mapping/`
- **Características Obrigatórias:**
  - Perfis de mapeamento definidos
  - Conversões bidirecionais (DTO ↔ Entity)
  - Validações durante mapeamento quando necessário

### 2.3 Camada de Apresentação (Web/API)

#### 2.3.1 Controllers
- **Definição:** Pontos de entrada HTTP que delegam para casos de uso
- **Localização:** `<Module>.Web/Controllers/`
- **Características Obrigatórias:**
  - Herdam de `ControllerBase`
  - Rota padrão: `[ApiController][Route("api/v1/[controller]")]`
  - Métodos: GET, POST, PUT, DELETE com as devidas responsabilidades
  - Recebem DTOs de Request (não entidades)
  - Injetam use cases no construtor
  - Chamam use cases via `ExecuteAsync()`
  - Retornam DTOs de Response e status HTTP
  - Validação de autorização via `[Authorize]`
- **Proibido:**
  - Lógica de negócio dentro do controller
  - Acesso direto a repositórios
  - Retornar entidades de domínio

#### 2.3.2 Filtros de Autorização
- **Localização:** `<Module>.Web/Filters/`
- **Uso:** Validar papéis ([[Authorize(Roles = "GERENTE")]])
- **Características Obrigatórias:**
  - Baseados em atributos
  - Validar papel de usuário
  - Bloquear operações não autorizadas com 403 Forbidden

### 2.4 Camada de Infraestrutura (Infrastructure)

#### 2.4.1 Implementações de Repositórios
- **Definição:** Implementações concretas das interfaces definidas em Domain
- **Localização:** `<Module>.Infrastructure/Repositories/`
- **Características Obrigatórias:**
  - Herdam das interfaces de Domain
  - Utilizam Entity Framework Core via DbContext
  - Implementam métodos de consulta e persistência
  - Retornam apenas o que a interface define
  - Lançam `DomainException` em violações de regras

#### 2.4.2 DbContext
- **Definição:** Contexto do Entity Framework untuk persistência
- **Localização:** `Infrastructure/Persistence/`
- **Características Obrigatórias:**
  - Configuração de entidades pode estar em `OnModelCreating` ou em `IEntityTypeConfiguration`
  - Mapeamento de Value Objects usando `.OwnsOne()` ou `.ComplexProperty()`
  - Índices para campos únicos (CPF, Username, etc.)
  - Shadow properties para auditorias (`CreatedAt`, `UpdatedAt`)

#### 2.4.3 Serviços de Infraestrutura
- **Localização:** `<Module>.Infrastructure/Services/`
- **Exemplos:** `JwtService`, `CacheService`, `EmailService`
- **Características:** Implementam interfaces abstratas definidas em Domain/Application

---

## 3. Modelos de Domínio

### 3.1 Definições Obrigatórias

#### 3.1.1 Entidades: Características Fundamentais
- Possuem identidade única (`Id: Guid`)
- Encapsulam comportamento relacionado ao negócio
- Validam invariantes em construtores e métodos de alteração
- Implementam mudanças de estado através de métodos (nunca setters públicos)

#### 3.1.2 Value Objects: Características Fundamentais
- Imutáveis após construção
- Comparação por valor (não por identidade)
- Validação completa no construtor
- Implementam `IEquatable<T>` e `IComparable<T>` quando apropriado

#### 3.1.3 Agregados: Características Fundamentais
- Uma entidade raiz (Aggregate Root)
- Entidades filhas referenciadas apenas via ID (nunca navegação de referência)
- Transação atômica no raiz
- Modificações sempre passam pelo raiz

### 3.2 Especificação de Entidades Críticas

#### 3.2.1 Usuario (Aggregate Root - Auth Module)
```csharp
- Id: Guid (PK)
- UserName: string (unique, required)
- PasswordHash: string (required, hashed com BCrypt)
- Role: UserRole enum (GERENTE, FUNCIONARIO)
- IsActive: bool (default: true)
- CreatedAt: DateTime
- UpdatedAt: DateTime

Métodos Obrigatórios:
- UpdatePassword(Password newPassword)
- Deactivate()
- Activate()
- ValidatePassword(Password password): bool

Invariantes:
- UserName não pode ser vazio ou duplicado (RN)
- PasswordHash obrigatório
- Role deve ser valor válido do enum
```

#### 3.2.2 Funcionario (Aggregate Root - Funcionario Module)
```csharp
- Id: Guid (PK)
- UserId: Guid (FK para Usuario)
- Name: string (required, max 200)
- CPF: CPF ValueObject (required, unique)
- Phone: Phone ValueObject (required)
- Email: string (optional)
- IsActive: bool (default: true)
- CreatedAt: DateTime
- UpdatedAt: DateTime

Métodos Obrigatórios:
- Deactivate()
- Activate()
- UpdateProfile(string name, Phone phone, string email)

Invariantes:
- CPF único (RN09)
- Name obrigatório
- Phone obrigatório
- IsActive determina acesso
```

#### 3.2.3 Produto (Aggregate Root - Produto Module)
```csharp
- Id: Guid (PK)
- Name: string (required, unique, max 200)
- Type: ProductType enum
- Price: Price ValueObject (required, > 0)
- Quantity: int (required, >= 0)
- IsActive: bool (default: true)
- CreatedAt: DateTime
- UpdatedAt: DateTime

Métodos Obrigatórios:
- DecreaseStock(int quantity): void
- IncreaseStock(int quantity): void
- Deactivate()

Invariantes:
- Name único (RN)
- Quantity nunca negativo (RN06)
- Price sempre > 0
- Produtos inativos bloqueados de operações (exceto query)
```

#### 3.2.4 Cliente (Aggregate Root - Cliente Module)
```csharp
- Id: Guid (PK)
- Name: string (required, max 200)
- CpfCnpj: CpfCnpj ValueObject (required, unique)
- Type: ClientType enum (PF ou PJ)
- Phone: Phone ValueObject (required)
- Email: string (optional)
- IsActive: bool (default: true)
- CreatedAt: DateTime
- UpdatedAt: DateTime
- Addresses: List<Address> (valor objeto ou entidade filha)

Métodos Obrigatórios:
- AddAddress(Address address): void
- UpdateProfile(string name, string phone, string email): void
- Deactivate()

Invariantes:
- CpfCnpj único (RN09)
- Type determina validação de documento
- Clientes nunca deletados fisicamente (soft delete)
```

#### 3.2.5 Venda (Aggregate Root - Venda Module)
```csharp
- Id: Guid (PK)
- CustomerId: Guid? (optional, FK)
- EmployeeId: Guid (required, FK)
- Total: Money ValueObject
- CurrentDiscount: double (0 a 100%)
- VendaItems: List<VendaItem>
- Date: DateTime
- CreatedAt: DateTime

Métodos Obrigatórios:
- AddItem(VendaItem item): void
- RemoveItem(Guid itemId): void
- ApplyDiscount(double percentage): void (somente GERENTE)
- CalculateTotal(): Money
- Confirm(): void

Invariantes:
- Desconto aplicável apenas por GERENTE
- VendaItems não pode estar vazio
- Total recalculado a cada mudança
- Estoque decrementado após confirmação (RN06)
```

#### 3.2.6 Plano (Aggregate Root - Plano Module)
```csharp
- Id: Guid (PK)
- CustomerId: Guid (required, FK)
- Cycle: PlanCycle enum (MENSAL, TRIMESTRAL, ANUAL, CUSTOM)
- Total: Money ValueObject
- CurrentDiscount: double
- PlanoItems: List<PlanoItem>
- StartDate: DateTime
- EndDate: DateTime
- IsActive: bool
- CreatedAt: DateTime

Métodos Obrigatórios:
- AddItem(PlanoItem item): void
- GenerateDeliveries(): List<Delivery>
- Upgrade(List<PlanoItem> newItems): void
- Downgrade(List<PlanoItem> newItems): ContractPenalty
- Cancel(): ContractPenalty
- ApplyDiscount(double percentage): void (somente GERENTE)

Invariantes:
- Ciclo válido
- Items não vazio
- Upgrade sem penalidade
- Downgrade/Cancel gera penalidade (RN08)
```

#### 3.2.7 Delivery (Entidade filha de Plano)
```csharp
- Id: Guid (PK)
- PlanId: Guid (FK, obrigatório)
- Period: int (número do ciclo)
- DueDate: DateTime
- DeliveryDate: DateTime?
- Status: DeliveryStatus enum (PENDING, DELIVERED, OVERDUE, CANCELLED)
- CreatedAt: DateTime

Métodos Obrigatórios:
- ConfirmDelivery(DateTime deliveredAt): void
- MarkOverdue(): void
- Cancel(): void

Invariantes:
- DueDate obrigatório
- Status transições válidas (PENDING → DELIVERED/OVERDUE/CANCELLED)
- Vencida sem entrega → Status OVERDUE
```

#### 3.2.8 StockMovement (Entidade de Auditoria - Shared Module)
```csharp
- Id: Guid (PK)
- ProductId: Guid (FK)
- Type: MovementType enum (ENTRADA, SAIDA, AJUSTE)
- Quantity: int
- Reason: string (enum: VENDA, ENTRADA_MANUAL, PLANO, AJUSTE)
- ReferenceId: Guid (FK para Venda, Plano, etc.)
- CreatedBy: Guid (FK Usuario)
- CreatedAt: DateTime

Invariantes:
- Imutável após criação (RN06)
- Saída não pode deixar estoque negativo
- Cada operação gera movimento
```

#### 3.2.9 AuditLog (Entidade de Auditoria - Shared Module)
```csharp
- Id: Guid (PK)
- UserId: Guid (FK)
- Action: AuditAction enum (CREATE, UPDATE, DELETE, LOGIN, LOGOUT)
- EntityType: string
- EntityId: Guid
- OldValues: JSON string
- NewValues: JSON string
- IpAddress: string
- Timestamp: DateTime

Invariantes:
- Imutável (RNF02)
- Cobertura obrigatória: login/logout, desconto, cancelamento, exclusão
```

### 3.3 Value Objects Obrigatórios

#### 3.3.1 CPF
```csharp
- Value: string (11 dígitos, validado)

Validação:
- Formato numérico
- 11 dígitos antes de normalização
- Validação de dígito verificador
```

#### 3.3.2 CNPJ
```csharp
- Value: string

Validação:
- CNPJ: 14 dígitos
- Dígito verificador validado
```

#### 3.3.3 Phone
```csharp
- Value: string (formato internacional ou local)

Validação:
- Mínimo 10 dígitos
- Máximo 15 dígitos
```

#### 3.3.4 Password
```csharp
- Value: string (nunca retorna valor legível)

Validação Obrigatória:
- Mínimo 8 caracteres
- Pelo menos 1 maiúscula
- Pelo menos 1 minúscula
- Pelo menos 1 número
- Pelo menos 1 caractere especial (!@#$%^&*)
```

#### 3.3.5 Price
```csharp
- Value: decimal (precisão 2 casas decimais)

Validação:
- Obrigatório > 0
- Máximo 2 casas decimais
```

#### 3.3.6 Money (em Venda/Plano)
```csharp
- Value: decimal
- Currency: string (padrão: BRL)

Validação:
- Não negativo
- Precisão 2 casas decimais
```

---

## 4. Repositories e Abstrações

### 4.1 Interface Genérica de Repositório

Todo repositório deve implementar a interface base:

```csharp
public interface IRepository<T> where T : class, IAggregateRoot
{
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### 4.2 Repositórios Específicos Obrigatórios

#### 4.2.1 IFuncionarioRepository
```csharp
Métodos Obrigatórios:
- GetByIdAsync(Guid id)
- GetByCpfAsync(string cpf)
- GetByUserNameAsync(string userName)
- GetAllActiveAsync()
- GetAllAsync()
- AddAsync(Funcionario funcionario)
- UpdateAsync(Funcionario funcionario)
- DeleteAsync(Guid id)
- ExistsByCpfAsync(string cpf)
- ExistsByUserNameAsync(string userName)
```

#### 4.2.2 IProdutoRepository
```csharp
Métodos Obrigatórios:
- GetByIdAsync(Guid id)
- GetByNameAsync(string name)
- GetAllActiveAsync()
- GetAllAsync()
- GetByTypeAsync(ProductType type)
- GetLowStockProductsAsync(int threshold)
- AddAsync(Produto produto)
- UpdateAsync(Produto produto)
- DeactivateAsync(Guid id)
```

#### 4.2.3 IClienteRepository
```csharp
Métodos Obrigatórios:
- GetByIdAsync(Guid id)
- GetByCpfCnpjAsync(string cpfCnpj)
- GetAllActiveAsync()
- GetAllAsync()
- GetWithAddressesAsync(Guid id)
- AddAsync(Cliente cliente)
- UpdateAsync(Cliente cliente)
- DeactivateAsync(Guid id)
- ExistsByCpfCnpjAsync(string cpfCnpj)
```

#### 4.2.4 IVendaRepository
```csharp
Métodos Obrigatórios:
- GetByIdAsync(Guid id)
- GetAllAsync()
- GetByCustomerIdAsync(Guid customerId)
- GetByEmployeeIdAsync(Guid employeeId)
- GetByDateRangeAsync(DateTime start, DateTime end)
- AddAsync(Venda venda)
- UpdateAsync(Venda venda)
```

#### 4.2.5 IPlanoRepository
```csharp
Métodos Obrigatórios:
- GetByIdAsync(Guid id)
- GetByCustomerIdAsync(Guid customerId)
- GetAllActiveAsync()
- GetAllAsync()
- AddAsync(Plano plano)
- UpdateAsync(Plano plano)
- DeactivateAsync(Guid id)
```

#### 4.2.6 IStockMovementRepository
```csharp
Métodos Obrigatórios:
- GetByIdAsync(Guid id)
- GetByProductIdAsync(Guid productId)
- GetByDateRangeAsync(DateTime start, DateTime end)
- GetByReasonAsync(MovementReason reason)
- AddAsync(StockMovement movement)
- (Proibido: Delete, Update — imutável)
```

#### 4.2.7 IAuditLogRepository
```csharp
Métodos Obrigatórios:
- GetByIdAsync(Guid id)
- GetByUserIdAsync(Guid userId)
- GetByEntityAsync(string entityType, Guid entityId)
- GetByDateRangeAsync(DateTime start, DateTime end)
- AddAsync(AuditLog log)
- (Proibido: Delete, Update — imutável)
```

### 4.3 Regras de Implementação de Repositórios

- Localizar em `<Module>.Infrastructure/Repositories/`
- Herdar de interface específica definida em `<Module>.Domain/Repositories/`
- Usar Entity Framework Core via `DbContext`
- Retornar `Task<T>` ou `Task<IEnumerable<T>>`
- Lançar `DomainException` em violações de regra
- Nenhuma exposição de `IQueryable<T>`
- Tratamento de exceções do banco de dados e mapeamento para `DomainException`

---

## 5. Serviços de Domínio

### 5.1 Definição
Serviços de domínio encapsulam lógica que não pertence a uma entidade ou value object específico, geralmente envolvendo múltiplas entidades ou agregados.

### 5.2 Características Obrigatórias
- Localização: `<Module>.Domain/Services/`
- Interfaces apenas (implementação em Infrastructure)
- Nomes em formato `I<BusinessConcept>Service`
- Métodos retornam tipos de domínio ou lançam `DomainException`
- Nenhuma dependência de frameworks externos

### 5.3 Serviços de Domínio Obrigatórios

#### 5.3.1 JwtService (Auth Module)
```csharp
Responsabilidade: Geração e validação de tokens JWT

Interface:
- GenerateTokenAsync(Usuario usuario): string
- ValidateTokenAsync(string token): ClaimsPrincipal
- GenerateRefreshTokenAsync(Usuario usuario): RefreshToken
- RevokeRefreshTokenAsync(string token): void
```

#### 5.3.2 EstoqueService (Produto Module)
```csharp
Responsabilidade: Validação de operações de estoque

Interface:
- ValidateSufficientStockAsync(Guid productId, int quantity): bool
- ReserveStockAsync(Guid productId, int quantity): void
- DecrementStockAsync(Guid productId, int quantity): void
- IncrementStockAsync(Guid productId, int quantity): void
```

#### 5.3.3 VendaService (Venda Module)
```csharp
Responsabilidade: Orquestração de venda com impacto em estoque

Interface:
- ProcessVendaAsync(Venda venda): Task<VendaConfirmation>
- CalculateDiscountAsync(Venda venda, double percentage): Money
```

#### 5.3.4 PlanoService (Plano Module)
```csharp
Responsabilidade: Geração de entregas e cálculo de multas

Interface:
- GenerateDeliveriesAsync(Plano plano): Task<List<Delivery>>
- CalculatePenaltyAsync(Plano plano, PenaltyType type): ContractPenalty
- ValidateUpgradeAsync(Plano plano, List<PlanoItem> newItems): bool
```

---

## 6. Use Cases (Casos de Uso)

### 6.1 Padrão Obrigatório de Use Case

Cada caso de uso é implementado como uma classe independente com interface dedicada, seguindo o padrão descrito abaixo:

#### Interface de Use Case
```csharp
// Localização: <Module>.Application/UseCases/<CaseName>/I<CaseName>UseCase.cs
public interface ICreateFuncionarioUseCase
{
    Task<CreateFuncionarioResponse> ExecuteAsync(
        CreateFuncionarioRequest request, 
        CancellationToken cancellationToken);
}
```

#### Request DTO
```csharp
// Localização: <Module>.Application/UseCases/<CaseName>/CreateFuncionarioRequest.cs
public record CreateFuncionarioRequest
{
    [Required(ErrorMessage = "UserName é obrigatório")]
    [StringLength(100)]
    public string UserName { get; init; }

    [Required(ErrorMessage = "Password é obrigatório")]
    public string Password { get; init; }

    [Required]
    [StringLength(200)]
    public string Name { get; init; }

    [Required]
    public string CPF { get; init; }

    [Required]
    public string Phone { get; init; }
}
```

#### Response DTO
```csharp
// Localização: <Module>.Application/UseCases/<CaseName>/CreateFuncionarioResponse.cs
public record CreateFuncionarioResponse
{
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string Name { get; init; }
    public DateTime CreatedAt { get; init; }
}
```

#### Implementação de Use Case
```csharp
// Localização: <Module>.Application/UseCases/<CaseName>/CreateFuncionarioUseCase.cs
public class CreateFuncionarioUseCase : ICreateFuncionarioUseCase
{
    private readonly IFuncionarioRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateFuncionarioUseCase> _logger;

    public CreateFuncionarioUseCase(
        IFuncionarioRepository repository,
        IMapper mapper,
        ILogger<CreateFuncionarioUseCase> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateFuncionarioResponse> ExecuteAsync(
        CreateFuncionarioRequest request,
        CancellationToken cancellationToken)
    {
        // Validações
        if (await _repository.ExistsByUserNameAsync(request.UserName, cancellationToken))
            throw new DomainException("UserName já existe");

        // Criar entidade de domínio
        var funcionario = Funcionario.Create(
            request.UserName,
            request.Password,
            request.Name,
            request.CPF,
            request.Phone);

        // Persistir
        await _repository.AddAsync(funcionario, cancellationToken);

        _logger.LogInformation("Funcionario {FuncionarioId} criado com sucesso", funcionario.Id);

        // Retornar DTO
        return _mapper.Map<CreateFuncionarioResponse>(funcionario);
    }
}
```

### 6.2 Regras Obrigatórias de Implementação

- **Uma interface por caso de uso** em `I<CaseName>UseCase.cs`
- **Uma classe implementadora** em `<CaseName>UseCase.cs`
- **DTOs Request e Response** na mesma pasta do caso de uso
- **Padrão de assinatura:** `ExecuteAsync(Request request, CancellationToken ct): Task<Response>`
- **Responsabilidades:**
  - Validação de entrada (usar Data Annotations no Request)
  - Orquestração de domínio (coordenar entidades e serviços)
  - Persistência através de repositórios
  - Logging de ações importantes
  - Mapeamento para DTO de resposta
- **Proibido:**
  - Lógica complexa do domínio (deve estar em Domain)
  - Acesso direto a DbContext
  - Retornar entidades de domínio
  - Chamar outro use case (usar serviço de domínio se necessário)

### 6.3 Tratamento de Erros em Use Cases

Todos os use cases devem lançar `DomainException` em casos de violação de regra de negócio:

```csharp
public async Task<FuncionarioResponse> ExecuteAsync(
    GetFuncionarioByIdRequest request,
    CancellationToken cancellationToken)
{
    var funcionario = await _repository.GetByIdAsync(request.Id, cancellationToken);
    
    if (funcionario == null)
        throw new DomainException($"Funcionário {request.Id} não encontrado");
    
    if (!funcionario.IsActive)
        throw new DomainException($"Funcionário {request.Id} está inativo");

    return _mapper.Map<FuncionarioResponse>(funcionario);
}
```

### 6.4 Injeção de Dependência de Use Cases

Todos os use cases devem ser registrados no container de DI no `Program.cs`:

```csharp
services.AddScoped<ICreateFuncionarioUseCase, CreateFuncionarioUseCase>();
services.AddScoped<IUpdateFuncionarioUseCase, UpdateFuncionarioUseCase>();
services.AddScoped<IGetFuncionarioByIdUseCase, GetFuncionarioByIdUseCase>();
services.AddScoped<IListFuncionariosUseCase, ListFuncionariosUseCase>();
```

---

### 6.5 Casos de Uso Obrigatórios por Módulo

#### Auth Module
- `Login`
- `Logout`
- `RefreshToken`

#### Funcionario Module
- `CreateFuncionario`
- `UpdateFuncionario`
- `DeactivateFuncionario`
- `ActivateFuncionario`
- `GetFuncionarioById`
- `ListFuncionarios`
- `GetFuncionarioByUsername`

#### Produto Module
- `CreateProduto`
- `UpdateProduto`
- `DeactivateProduto`
- `IncreaseStock`
- `DecreaseStock`
- `GetProdutoById`
- `ListProdutos`
- `ListLowStockProducts`

#### Cliente Module
- `CreateCliente`
- `UpdateCliente`
- `DeactivateCliente`
- `AddAddress`
- `GetClienteById`
- `ListClientes`
- `GetClienteConsumptionHistory`

#### Venda Module
- `CreateVenda`
- `AddVendaItem`
- `ApplyDiscount` somente GERENTE
- `ConfirmVenda`
- `GetVendaById`
- `ListVendas`
- `ListVendasByCustomer`
- `ListVendasByDateRange`

#### Plano Module
- `CreatePlano`
- `AddPlanoItem`
- `ApplyDiscount` somente GERENTE
- `UpgradePlano`
- `DowngradePlano` - gera penalidade
- `CancelPlano` - gera penalidade
- `GetPlanoById`
- `ListPlanos`
- `ListPlanosByCustomer`

---

## 7. DTOs (Data Transfer Objects)

### 7.1 Regras Obrigatórias

- **Localização:** `<Module>.Application/DTOs/` ou dentro de cada caso de uso
- **Nomenclatura:** Seguir padrão `<Entity><Direction>Dto`
- **Estrutura:** Propriedades públicas simples, sem lógica
- **Tipos:** Apenas primitivos, strings e outros DTOs (nunca entidades de domínio)
- **Validação:** Usar Data Annotations (`[Required]`, `[StringLength]`, etc.)

### 7.2 Tipos Obrigatórios

#### Create/UpdateRequest
```csharp
[ApiController]
public record CreateFuncionarioRequest
{
    [Required(ErrorMessage = "UserName é obrigatório")]
    [StringLength(100)]
    public string UserName { get; init; }

    [Required(ErrorMessage = "Password é obrigatório")]
    public string Password { get; init; }

    [Required]
    [StringLength(200)]
    public string Name { get; init; }

    [Required]
    public string CPF { get; init; }

    [Required]
    public string Phone { get; init; }
}
```

#### Response
```csharp
public record FuncionarioResponse
{
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string Name { get; init; }
    public string CPF { get; init; }
    public string Phone { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}
```

#### List Response
```csharp
public record FuncionarioListResponse
{
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public List<FuncionarioResponse> Items { get; init; }
}
```

### 7.3 Mapeamento

- **Ferramenta:** AutoMapper
- **Localização:** `<Module>.Application/Mapping/MappingProfiles.cs`
- **Características:** Perfis bidirecionais (DTO ↔ Entity)

---

## 8. Controllers (Endpoints)

### 8.1 Padrão Obrigatório de Endpoint

```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class FuncionarioController : ControllerBase
{
    private readonly ICreateFuncionarioUseCase _createUseCase;
    private readonly IGetFuncionarioByIdUseCase _getByIdUseCase;
    private readonly IListFuncionariosUseCase _listUseCase;
    private readonly IUpdateFuncionarioUseCase _updateUseCase;
    private readonly IDeactivateFuncionarioUseCase _deactivateUseCase;

    public FuncionarioController(
        ICreateFuncionarioUseCase createUseCase,
        IGetFuncionarioByIdUseCase getByIdUseCase,
        IListFuncionariosUseCase listUseCase,
        IUpdateFuncionarioUseCase updateUseCase,
        IDeactivateFuncionarioUseCase deactivateUseCase)
    {
        _createUseCase = createUseCase;
        _getByIdUseCase = getByIdUseCase;
        _listUseCase = listUseCase;
        _updateUseCase = updateUseCase;
        _deactivateUseCase = deactivateUseCase;
    }

    [HttpPost]
    [AllowAnonymous]  // Apenas para exceções (ex: Login)
    public async Task<IActionResult> Create(
        [FromBody] CreateFuncionarioRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _createUseCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetFuncionarioByIdRequest { Id = id };
        var response = await _getByIdUseCase.ExecuteAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var request = new ListFuncionariosRequest { PageNumber = pageNumber, PageSize = pageSize };
        var response = await _listUseCase.ExecuteAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "GERENTE")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateFuncionarioRequest request,
        CancellationToken cancellationToken)
    {
        var updateRequest = request with { Id = id };
        await _updateUseCase.ExecuteAsync(updateRequest, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "GERENTE")]
    public async Task<IActionResult> Deactivate(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new DeactivateFuncionarioRequest { Id = id };
        await _deactivateUseCase.ExecuteAsync(request, cancellationToken);
        return NoContent();
    }
}
```

### 8.2 Regras Obrigatórias de Endpoints

1. **Autenticação:** `[Authorize]` em todos os endpoints (exceto Login)
2. **Autorização:** `[Authorize(Roles = "...")]` em operações sensíveis
3. **Validação:** Data Annotations no DTO
4. **Injeção:** Use cases injetados no construtor, chamados diretamente via `ExecuteAsync()`
5. **Status HTTP:** 
   - 201 Created (POST bem-sucedido)
   - 200 OK (GET/PUT bem-sucedido)
   - 204 No Content (DELETE bem-sucedido, sem retorno)
   - 400 Bad Request (validação falhou)
   - 401 Unauthorized (sem autenticação)
   - 403 Forbidden (sem autorização)
   - 404 Not Found (recurso não encontrado)
   - 500 Internal Server Error (erro não tratado)
6. **Método por operação:**
   - POST: Criar novo recurso
   - GET: Recuperar recurso(s)
   - PUT: Atualizar recurso existente
   - DELETE: Remover/desativar recurso

### 8.3 Endpoints Obrigatórios por Módulo

#### Funcionario
- `POST /api/v1/funcionario` - Criar
- `GET /api/v1/funcionario/{id}` - Obter por ID
- `GET /api/v1/funcionario` - Listar com paginação
- `PUT /api/v1/funcionario/{id}` - Atualizar (GERENTE)
- `DELETE /api/v1/funcionario/{id}` - Desativar (GERENTE)

#### Produto
- `POST /api/v1/produto` - Criar
- `GET /api/v1/produto/{id}` - Obter por ID
- `GET /api/v1/produto` - Listar com filtros
- `PUT /api/v1/produto/{id}` - Atualizar
- `POST /api/v1/produto/{id}/increase-stock` - Aumentar estoque (GERENTE)
- `POST /api/v1/produto/{id}/decrease-stock` - Diminuir estoque (GERENTE)

#### Cliente
- `POST /api/v1/cliente` - Criar
- `GET /api/v1/cliente/{id}` - Obter por ID
- `GET /api/v1/cliente` - Listar
- `PUT /api/v1/cliente/{id}` - Atualizar
- `POST /api/v1/cliente/{id}/address` - Adicionar endereço
- `GET /api/v1/cliente/{id}/consumptionHistory` - Histórico de consumo

#### Venda
- `POST /api/v1/venda` - Criar
- `POST /api/v1/venda/{id}/items` - Adicionar item
- `POST /api/v1/venda/{id}/discount` - Aplicar desconto (GERENTE)
- `POST /api/v1/venda/{id}/confirm` - Confirmar
- `GET /api/v1/venda/{id}` - Obter por ID
- `GET /api/v1/venda` - Listar com filtros

#### Plano
- `POST /api/v1/plano` - Criar
- `PUT /api/v1/plano/{id}` - Atualizar
- `POST /api/v1/plano/{id}/upgrade` - Fazer upgrade (sem penalidade)
- `POST /api/v1/plano/{id}/downgrade` - Fazer downgrade (com penalidade)
- `POST /api/v1/plano/{id}/cancel` - Cancelar (com penalidade)
- `GET /api/v1/plano/{id}` - Obter por ID
- `GET /api/v1/plano` - Listar com filtros

---

## 9. Fluxo de Dependências

### 9.1 Regra Central: Inversão de Dependência

```
EXTERNO (Web) → APLICAÇÃO → DOMÍNIO ← INFRAESTRUTURA
```

**Explicação:**
- Web depende de Application (nunca de Domain ou Infrastructure)
- Application depende de Domain abstrações e Infrastructure (via interfaces)
- Domain não depende de nada (camada central)
- Infrastructure depende de Domain abstrações (inversão)

### 9.2 Exemplo Permitido de Fluxo

```csharp
1. Web/Controller recebe HTTP request
   ↓
2. Controller valida DTO
   ↓
3. Controller injeta ICreateFuncionarioUseCase (Application)
   ↓
4. Controller chama _useCase.ExecuteAsync(request)
   ↓
5. Use Case injeta IFuncionarioRepository (abstração em Domain)
   ↓
6. Use Case injeta IEstoqueService (abstração em Domain)
   ↓
7. Use Case coordena persistência via repositório
   ↓
8. Infrastructure implementa repositório com EF Core
   ↓
9. Use Case persiste auditoria via IAuditService
   ↓
10. Controller retorna Response (DTO)
```

### 9.3 Padrão Obrigatório: Injeção de Dependência

```csharp
// Program.cs configuração
// Repositories
services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
services.AddScoped<IEstoqueService, EstoqueService>();

// Use Cases
services.AddScoped<ICreateFuncionarioUseCase, CreateFuncionarioUseCase>();
services.AddScoped<IUpdateFuncionarioUseCase, UpdateFuncionarioUseCase>();
services.AddScoped<IGetFuncionarioByIdUseCase, GetFuncionarioByIdUseCase>();
services.AddScoped<IListFuncionariosUseCase, ListFuncionariosUseCase>();
services.AddScoped<IDeactivateFuncionarioUseCase, DeactivateFuncionarioUseCase>();

// Genericó
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
services.AddAutoMapper(/* assemblies */);
```

### 9.4 Diagrama de Dependências por Módulo

```
AUTH Module
  Web/Controllers → ILoginUseCase, ILogoutUseCase
    ↓
  Application/UseCases → IUserRepository
                      → IJwtService
    ↓
  Domain/Repositories → interfaces apenas
  Domain/Services → interfaces apenas
    ↓
  Infrastructure/Repositories → implementação com EF
  Infrastructure/Services → implementação concreta
```

---

## 10. Convenções de Código

### 10.1 Nomenclatura

#### Entidades
- Singular, PascalCase: `Funcionario`, `Produto`, `Cliente`
- Propriedade ID: `Id: Guid`
- Booleanos com prefixo: `IsActive`, `IsDeleted`, `HasDiscountApplied`

#### Value Objects
- PascalCase: `CPF`, `Price`, `Phone`, `Money`
- Propriedade de valor: `Value` (protected readonly)

#### Repositórios
- Interface: `I<Entity>Repository` (ex: `IFuncionarioRepository`)
- Implementação: `<Entity>Repository` (ex: `FuncionarioRepository`)

#### Casos de Uso (Use Cases)
- Interface: `I<Verb><Entity>UseCase` (ex: `ICreateFuncionarioUseCase`, `IUpdateProductUseCase`)
- Implementação: `<Verb><Entity>UseCase` (ex: `CreateFuncionarioUseCase`)
- Request: `<Verb><Entity>Request` (ex: `CreateFuncionarioRequest`)
- Response: `<Entity>Response` ou `<Verb><Entity>Response` (ex: `FuncionarioResponse`, `CreateFuncionarioResponse`)

#### Serviços
- Interface: `I<Concept>Service` (ex: `IEstoqueService`, `IJwtService`)
- Implementação: `<Concept>Service`

#### DTOs
- Criação: `Create<Entity>Request`, `Create<Entity>Response`
- Atualização: `Update<Entity>Request`
- Listagem: `<Entity>Response`, `<Entity>ListResponse`
- Queries: `<Entity>QueryResponse`

#### Controllers
- PascalCase: `FuncionarioController`, `ProdutoController`
- Rota: `/api/v1/[controller]` (plural é aceito por convenção)

#### Métodos de Repositório
- Queries: `GetByIdAsync`, `GetAllAsync`, `GetByXxxAsync`, `ExistsByXxxAsync`
- Persistência: `AddAsync`, `UpdateAsync`, `DeleteAsync`
- Combinações: `GetByDateRangeAsync`, `GetLowStockProductsAsync`

### 10.2 Estrutura de Arquivos Obrigatória

```
src/
├── Modules/
│   ├── Auth/
│   │   ├── Auth.Domain/
│   │   │   ├── Entities/
│   │   │   │   └── Usuario.cs
│   │   │   ├── ValueObjects/
│   │   │   │   └── Password.cs
│   │   │   ├── Repositories/
│   │   │   │   └── IUsuarioRepository.cs
│   │   │   ├── Services/
│   │   │   │   └── IJwtService.cs
│   │   │   └── Exceptions/
│   │   │       └── InvalidCredentialsException.cs
│   │   ├── Auth.Application/
│   │   │   ├── UseCases/
│   │   │   │   ├── Login/
│   │   │   │   │   ├── ILoginUseCase.cs
│   │   │   │   │   ├── LoginUseCase.cs
│   │   │   │   │   ├── LoginRequest.cs
│   │   │   │   │   └── LoginResponse.cs
│   │   │   │   ├── Logout/
│   │   │   │   │   ├── ILogoutUseCase.cs
│   │   │   │   │   ├── LogoutUseCase.cs
│   │   │   │   │   ├── LogoutRequest.cs
│   │   │   │   │   └── LogoutResponse.cs
│   │   │   │   └── RefreshToken/
│   │   │   │       ├── IRefreshTokenUseCase.cs
│   │   │   │       ├── RefreshTokenUseCase.cs
│   │   │   │       ├── RefreshTokenRequest.cs
│   │   │   │       └── RefreshTokenResponse.cs
│   │   │   └── Mapping/
│   │   │       └── AuthMappingProfile.cs
│   │   ├── Auth.Web/
│   │   │   ├── Controllers/
│   │   │   │   └── AuthController.cs
│   │   │   └── DTOs/
│   │   │       ├── LoginRequest.cs
│   │   │       ├── LoginResponse.cs
│   │   │       ├── LogoutRequest.cs
│   │   │       ├── LogoutResponse.cs
│   │   │       ├── RefreshTokenRequest.cs
│   │   │       └── RefreshTokenResponse.cs
│   │   └── Auth.Infrastructure/
│   │       ├── Repositories/
│   │       │   └── UsuarioRepository.cs
│   │       └── Services/
│   │           └── JwtService.cs
│   │
│   ├── Funcionario/ (mesmo padrão)
│   ├── Produto/ (mesmo padrão)
│   ├── Cliente/ (mesmo padrão)
│   ├── Venda/ (mesmo padrão)
│   ├── Plano/ (mesmo padrão)
│   │
│   └── Shared/
│       ├── Shared.Domain/
│       │   ├── Entities/
│       │   │   ├── AuditLog.cs
│       │   │   └── StockMovement.cs
│       │   ├── Repositories/
│       │   │   ├── IAuditLogRepository.cs
│       │   │   └── IStockMovementRepository.cs
│       │   └── Enums/
│       │       ├── AuditAction.cs
│       │       └── MovementType.cs
│       ├── Shared.Application/
│       │   ├── Services/
│       │   │   ├── IAuditService.cs
│       │   │   └── IStockManagementService.cs
│       │   └── DTOs/
│       │       └── AuditLogDto.cs
│       └── Shared.Infrastructure/
│           ├── Repositories/
│           │   ├── AuditLogRepository.cs
│           │   └── StockMovementRepository.cs
│           └── Services/
│               ├── AuditService.cs
│               └── StockManagementService.cs
│
├── Shared/
│   ├── DataErrors.cs
│   ├── Responses/
│   │   ├── ApiResponse.cs
│   │   ├── ErrorResponse.cs
│   │   └── PaginatedResponse.cs
│   └── Exceptions/
│       ├── DomainException.cs
│       └── BusinessException.cs
│
├── Web/
│   ├── ExceptionHandlers/
│   │   └── GlobalExceptionHandler.cs
│   ├── Validation/
│   │   └── FluentValidationConfiguration.cs
│   └── Filters/
│       └── ValidationFilter.cs
│
└── Program.cs
```

### 10.3 Padrões Obrigatórios de Código

#### Uso de Guid para ID
```csharp
public class Funcionario
{
    public Guid Id { get; private set; }
    public static Funcionario Create(string name, CPF cpf)
    {
        return new Funcionario { Id = Guid.NewGuid(), /* */ };
    }
}
```

#### Value Object Imutável
```csharp
public record CPF
{
    public string Value { get; init; }

    public CPF(string value)
    {
        if (!IsValid(value))
            throw new InvalidCpfException(value);
        Value = Normalize(value);
    }
}
```

#### Entidade com Comportamento
```csharp
public class Produto
{
    public void DecreaseStock(int quantity)
    {
        if (Quantity < quantity)
            throw new InsufficientStockException(Id, Quantity, quantity);
        Quantity -= quantity;
    }
}
```

#### Repository Pattern
```csharp
public interface IFuncionarioRepository : IRepository<Funcionario>
{
    Task<Funcionario> GetByCpfAsync(CPF cpf, CancellationToken token = default);
    Task<bool> ExistsByCpfAsync(CPF cpf, CancellationToken token = default);
}

public class FuncionarioRepository : IFuncionarioRepository
{
    private readonly AppDbContext _context;

    public async Task<Funcionario> GetByCpfAsync(CPF cpf, CancellationToken token)
    {
        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.CPF.Value == cpf.Value, token);
        
        if (funcionario is null)
            throw new FuncionarioNotFoundException(cpf.Value);
        
        return funcionario;
    }
}
```

#### Use Case Handler
```csharp
public class CreateFuncionarioHandler : IRequestHandler<CreateFuncionarioCommand, CreateFuncionarioResponse>
{
    private readonly IFuncionarioRepository _repository;
    private readonly IMapper _mapper;

    public async Task<CreateFuncionarioResponse> Handle(CreateFuncionarioCommand request, CancellationToken ct)
    {
        // 1. Validação
        if (await _repository.ExistsByCpfAsync(request.Cpf, ct))
            throw new CpfAlreadyRegisteredException(request.Cpf);

        // 2. Criar entidade de domínio
        var funcionario = Funcionario.Create(request.Name, new CPF(request.Cpf));

        // 3. Persistir
        await _repository.AddAsync(funcionario, ct);
        await _repository.SaveChangesAsync(ct);

        // 4. Mapear e retornar
        return _mapper.Map<CreateFuncionarioResponse>(funcionario);
    }
}
```

### 10.4 Padrões de Exception

#### Exceptions de Domínio
```csharp
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

public class FuncionarioNotFoundException : DomainException
{
    public FuncionarioNotFoundException(Guid id) 
        : base($"Funcionário com ID {id} não encontrado") { }
}

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(Guid productId, int available, int requested)
        : base($"Estoque insuficiente para produto {productId}. Disponível: {available}, Solicitado: {requested}") { }
}
```

#### Tratamento Global
```csharp
public class GlobalExceptionHandler
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ErrorResponse("Erro interno do servidor"));
        }
    }
}
```

### 10.5 Padrões de Async/Await

- **Todos os métodos de I/O:** `Task<T>`, `async/await`
- **Cancelation Tokens:** `CancellationToken cancellationToken = default` em todos os métodos assíncronos
- **Await apropriado:** Sempre aguardar operações assíncronas
- **ConfigureAwait:** Usar em bibliotecas, não necessário em aplicações

```csharp
// ✓ Correto
public async Task<Funcionario> GetByIdAsync(Guid id, CancellationToken ct = default)
{
    return await _context.Funcionarios.FirstOrDefaultAsync(f => f.Id == id, ct);
}

// ✗ Incorreto
public Task<Funcionario> GetByIdAsync(Guid id)
{
    return _context.Funcionarios.FirstOrDefaultAsync(f => f.Id == id);
}
```

---

## 11. Regras de Integração com PRD

### 11.1 Mapeamento de Requisitos Funcionais para Código

| RF | Implementação Arquitetural |
|----|-----------------------------|
| RF01 - Autenticação | Auth.Application.UseCases.Login + IJwtService |
| RF02 - Cadastro Clientes | Cliente.Application.UseCases.Create |
| RF03 - Cadastro Produtos | Produto.Application.UseCases.Create |
| RF04 - Gestão Planos | Plano.Application.UseCases.* |
| RF05 - Vendas Avulsas | Venda.Application.UseCases.* |
| RF06 - Controle Estoque | Produto.Domain + EstoqueService |
| RF07 - Histórico Consumo | Cliente.Application.Queries.GetConsumptionHistory |
| RF08 - Relatórios | Reports.Application.Queries |

### 11.2 Mapeamento de Regras de Negócio para Código

| RN | Localização em Código |
|----|----------------------|
| RN06 - Saída sem exceção registra movimento | StockMovement entity + auditoria |
| RN07 - Desconto bloqueado para Funcionário | [Authorize(Roles = "GERENTE")] |
| RN08 - Upgrade/Downgrade/Cancelamento | PlanoService + ContractPenalty |
| RN09 - CPF/CNPJ único | CpfCnpj VO + INDEX DB + ExistsByXxx repositório |

### 11.3 Mapeamento de RNF para Código

| RNF | Técnica |
|-----|---------|
| RNF01 - Segurança (hashing) | Password VO + BCrypt em Infrastructure |
| RNF02 - Auditoria (imutável) | AuditLog entity sem Update/Delete |
| RNF05 - Integridade transacional | EF Core transactions em handlers |
