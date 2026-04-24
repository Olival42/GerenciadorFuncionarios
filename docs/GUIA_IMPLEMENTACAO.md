# Guia Técnico de Implementação Corrigido — AquaGás Distribuidora

## Visão Geral

Este guia técnico descreve a implementação obrigatória de cada endpoint do sistema AquaGás Distribuidora, seguindo os princípios de Domain-Driven Design (DDD) e Clean Architecture. O documento foi corrigido para garantir consistência 100% com o DRP, DER e arquitetura existente. Serve como base para a criação de cards de desenvolvimento, definindo exatamente o que deve ser implementado sem sugestões ou alternativas.

### Estrutura de Implementação
- **Padrão:** Use Case simples (sem CQRS)
- **Injeção:** Direta de use cases no controller
- **Validação:** Data Annotations nos DTOs
- **Tratamento de Erros:** `DomainException` lançada pelos use cases
- **Mapeamento:** AutoMapper para conversão entre entidades e DTOs

### Módulos Implementados
- Auth
- Funcionario
- Produto
- Cliente
- Venda
- Plano
- Shared (para AuditLog, StockMovement, ContractPenalty)

---

## Auth Module

### 1. Login

#### 1.1 Nome do Endpoint
- **Método HTTP:** POST
- **Rota:** `/api/v1/auth/login`

#### 1.2 Arquivos que Devem Ser Criados
- Controller
- Use Case (Interface e Implementação)
- DTOs (Request/Response)
- Repository Implementation

#### 1.3 Nome dos Arquivos
- `Auth.Web/Controllers/AuthController.cs`
- `Auth.Application/UseCases/Login/ILoginUseCase.cs`
- `Auth.Application/UseCases/Login/LoginUseCase.cs`
- `Auth.Application/UseCases/Login/LoginRequest.cs`
- `Auth.Application/UseCases/Login/LoginResponse.cs`
- `Auth.Domain/Repositories/IUsuarioRepository.cs`
- `Auth.Infrastructure/Repositories/UsuarioRepository.cs`

#### 1.4 Responsabilidades
- **Controller:** Receber request, chamar use case, retornar response
- **Use Case:** Validar credenciais, gerar token JWT e refresh token, salvar refresh token, registrar auditoria
- **Repository:** Buscar usuário por UserName, salvar refresh token
- **Não deve:** Controller não deve conter lógica de negócio; Use Case não deve acessar diretamente DbContext

#### 1.5 Validações
- UserName obrigatório e existente
- Password obrigatório e válido
- Usuário deve estar ativo
- Campos obrigatórios: UserName, Password

#### 1.6 Fluxo de Chamadas
Controller → ILoginUseCase.ExecuteAsync(LoginRequest) → UsuarioRepository.GetByUserNameAsync → IJwtService.GenerateTokenAsync → RefreshTokenRepository.AddAsync → IAuditService.LogAsync("LOGIN", "Usuario", usuario.Id) → Retorno LoginResponse

#### 1.7 Retorno
- **Tipo:** LoginResponse
- **DTO:** `{ AccessToken: string, UserName: string, Role: string, ExpiresAt: DateTime }`
- **Cookie:** RefreshToken (httpOnly, secure, sameSite)
- **Códigos HTTP:** 200 OK (sucesso), 400 Bad Request (dados inválidos), 401 Unauthorized (credenciais incorretas)

#### 1.8 Dependências
- Controller injeta: ILoginUseCase
- Use Case injeta: IUsuarioRepository, IJwtService, IRefreshTokenRepository, IAuditService, IMapper

#### 1.9 Regras de Negócio
- Usuário deve estar ativo (RF01)
- Senha deve ser validada com hash (RNF01)
- Token JWT deve ser gerado com expiração (RNF01)
- Refresh token deve ser persistido (DER)
- Login deve ser auditado (RNF02)

### 2. Logout

#### 2.1 Nome do Endpoint
- **Método HTTP:** POST
- **Rota:** `/api/v1/auth/logout`

#### 2.2 Arquivos que Devem Ser Criados
- Controller (adicionar método ao AuthController)
- Use Case (Interface e Implementação)
- DTOs (Request/Response)

#### 2.3 Nome dos Arquivos
- Método em `Auth.Web/Controllers/AuthController.cs`
- `Auth.Application/UseCases/Logout/ILogoutUseCase.cs`
- `Auth.Application/UseCases/Logout/LogoutUseCase.cs`
- `Auth.Application/UseCases/Logout/LogoutRequest.cs`
- `Auth.Application/UseCases/Logout/LogoutResponse.cs`

#### 2.4 Responsabilidades
- **Controller:** Receber request com token, chamar use case
- **Use Case:** Invalidar refresh token, registrar auditoria
- **Não deve:** Controller não deve conter lógica de invalidação

#### 2.5 Validações
- Token obrigatório e válido
- Usuário autenticado

#### 2.6 Fluxo de Chamadas
Controller → ILogoutUseCase.ExecuteAsync(LogoutRequest) → IJwtService.RevokeTokenAsync → RefreshTokenRepository.RemoveAsync → AuditLogRepository.AddAsync → Retorno LogoutResponse

#### 2.7 Retorno
- **Tipo:** LogoutResponse
- **DTO:** `{ Success: bool, Message: string }`
- **Códigos HTTP:** 200 OK (sucesso), 401 Unauthorized (token inválido)

#### 2.8 Dependências
- Controller injeta: ILogoutUseCase
- Use Case injeta: IJwtService, IRefreshTokenRepository, IAuditLogRepository

#### 2.9 Regras de Negócio
- Token deve ser revogado (RNF01)
- Logout deve ser auditado (RNF02)

### 3. RefreshToken

#### 3.1 Nome do Endpoint
- **Método HTTP:** POST
- **Rota:** `/api/v1/auth/refresh`

#### 3.2 Arquivos que Devem Ser Criados
- Controller (adicionar método ao AuthController)
- Use Case (Interface e Implementação)
- DTOs (Request/Response)
- Repository Interface
- Repository Implementation

#### 3.3 Nome dos Arquivos
- Método em `Auth.Web/Controllers/AuthController.cs`
- `Auth.Application/UseCases/RefreshToken/IRefreshTokenUseCase.cs`
- `Auth.Application/UseCases/RefreshToken/RefreshTokenUseCase.cs`
- `Auth.Application/UseCases/RefreshToken/RefreshTokenRequest.cs`
- `Auth.Application/UseCases/RefreshToken/RefreshTokenResponse.cs`
- `Auth.Domain/Repositories/IRefreshTokenRepository.cs`
- `Auth.Infrastructure/Repositories/RefreshTokenRepository.cs`

#### 3.4 Responsabilidades
- **Controller:** Receber refresh token, chamar use case
- **Use Case:** Validar refresh token, gerar novo access token, atualizar refresh token
- **Repository:** Buscar e atualizar refresh token
- **Não deve:** Controller não deve gerar tokens

#### 3.5 Validações
- RefreshToken obrigatório e válido
- Não expirado

#### 3.6 Fluxo de Chamadas
Controller → IRefreshTokenUseCase.ExecuteAsync(RefreshTokenRequest) → RefreshTokenRepository.GetByTokenAsync → IJwtService.GenerateTokenAsync → RefreshTokenRepository.UpdateAsync → Retorno RefreshTokenResponse

#### 3.7 Retorno
- **Tipo:** RefreshTokenResponse
- **DTO:** `{ Token: string, AccessToken: string, ExpiresAt: DateTime }`
- **Códigos HTTP:** 200 OK (sucesso), 401 Unauthorized (token inválido)

#### 3.8 Dependências
- Controller injeta: IRefreshTokenUseCase
- Use Case injeta: IRefreshTokenRepository, IJwtService

#### 3.9 Regras de Negócio
- Refresh token deve ser validado e não expirado (RNF01)
- Novo refresh token deve ser gerado (DER)

---

## Funcionario Module

### 1. CreateFuncionario

#### 1.1 Nome do Endpoint
- **Método HTTP:** POST
- **Rota:** `/api/v1/funcionario`

#### 1.2 Arquivos que Devem Ser Criados
- Controller
- Use Case (Interface e Implementação)
- DTOs (Request/Response)
- Repository Interface (já existente)
- Repository Implementation (já existente)

#### 1.3 Nome dos Arquivos
- `Funcionario.Web/Controllers/FuncionarioController.cs`
- `Funcionario.Application/UseCases/CreateFuncionario/ICreateFuncionarioUseCase.cs`
- `Funcionario.Application/UseCases/CreateFuncionario/CreateFuncionarioUseCase.cs`
- `Funcionario.Application/UseCases/CreateFuncionario/CreateFuncionarioRequest.cs`
- `Funcionario.Application/UseCases/CreateFuncionario/CreateFuncionarioResponse.cs`
- `Funcionario.Domain/Repositories/IFuncionarioRepository.cs` (já existe)
- `Funcionario.Infrastructure/Repositories/FuncionarioRepository.cs` (já existe)

#### 1.4 Responsabilidades
- **Controller:** Receber request, chamar use case, retornar 201 Created
- **Use Case:** Criar entidade Funcionario, validar unicidade, persistir, registrar auditoria
- **Repository:** Adicionar funcionário ao banco
- **Não deve:** Controller não deve validar regras de negócio

#### 1.5 Validações
- UserName único e obrigatório
- Password obrigatório
- Name obrigatório (máx 200)
- CPF único e válido
- Phone obrigatório
- Campos obrigatórios: UserName, Password, Name, CPF, Phone

#### 1.6 Fluxo de Chamadas
Controller → ICreateFuncionarioUseCase.ExecuteAsync(CreateFuncionarioRequest) → IFuncionarioRepository.ExistsByUserNameAsync → IFuncionarioRepository.ExistsByCpfAsync → Funcionario.Create → IFuncionarioRepository.AddAsync → AuditLogRepository.AddAsync → Retorno CreateFuncionarioResponse

#### 1.7 Retorno
- **Tipo:** CreateFuncionarioResponse
- **DTO:** `{ Id: Guid, Name: string, CPF: string, Phone: string, Email: string, IsActive: bool, CreatedAt: DateTime }`
- **Códigos HTTP:** 201 Created (sucesso), 400 Bad Request (validação), 409 Conflict (duplicado)

#### 1.8 Dependências
- Controller injeta: ICreateFuncionarioUseCase
- Use Case injeta: IFuncionarioRepository, IAuditLogRepository, IMapper

#### 1.9 Regras de Negócio
- CPF único (RN09)
- UserName único
- Funcionário criado ativo por padrão
- Criação deve ser auditada (RNF02)

### 2. UpdateFuncionario

#### 2.1 Nome do Endpoint
- **Método HTTP:** PUT
- **Rota:** `/api/v1/funcionario/{id}`

#### 2.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Request/Response)

#### 2.3 Nome dos Arquivos
- Método em `Funcionario.Web/Controllers/FuncionarioController.cs`
- `Funcionario.Application/UseCases/UpdateFuncionario/IUpdateFuncionarioUseCase.cs`
- `Funcionario.Application/UseCases/UpdateFuncionario/UpdateFuncionarioUseCase.cs`
- `Funcionario.Application/UseCases/UpdateFuncionario/UpdateFuncionarioRequest.cs`
- `Funcionario.Application/UseCases/UpdateFuncionario/UpdateFuncionarioResponse.cs`

#### 2.4 Responsabilidades
- **Controller:** Receber id e request, chamar use case, retornar 200 OK
- **Use Case:** Buscar funcionário, atualizar perfil, persistir, registrar auditoria
- **Não deve:** Controller não deve buscar entidade

#### 2.5 Validações
- Id válido (Guid)
- Funcionário existe e ativo
- Name obrigatório (máx 200)
- Phone obrigatório

#### 2.6 Fluxo de Chamadas
Controller → IUpdateFuncionarioUseCase.ExecuteAsync(UpdateFuncionarioRequest) → IFuncionarioRepository.GetByIdAsync → Funcionario.UpdateProfile → IFuncionarioRepository.UpdateAsync → AuditLogRepository.AddAsync → Retorno void

#### 2.7 Retorno
- **Tipo:** void
- **Códigos HTTP:** 204 No Content (sucesso), 404 Not Found (não encontrado), 400 Bad Request (validação)

#### 2.8 Dependências
- Controller injeta: IUpdateFuncionarioUseCase
- Use Case injeta: IFuncionarioRepository, IAuditLogRepository

#### 2.9 Regras de Negócio
- Apenas GERENTE pode atualizar (RF01)
- Atualização deve ser auditada (RNF02)

### 3. DeactivateFuncionario

#### 3.1 Nome do Endpoint
- **Método HTTP:** DELETE
- **Rota:** `/api/v1/funcionario/{id}`

#### 3.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Request)

#### 3.3 Nome dos Arquivos
- Método em `Funcionario.Web/Controllers/FuncionarioController.cs`
- `Funcionario.Application/UseCases/DeactivateFuncionario/IDeactivateFuncionarioUseCase.cs`
- `Funcionario.Application/UseCases/DeactivateFuncionario/DeactivateFuncionarioUseCase.cs`
- `Funcionario.Application/UseCases/DeactivateFuncionario/DeactivateFuncionarioRequest.cs`

#### 3.4 Responsabilidades
- **Controller:** Receber id, chamar use case, retornar 204 No Content
- **Use Case:** Buscar funcionário, desativar, persistir, registrar auditoria
- **Não deve:** Controller não deve buscar entidade

#### 3.5 Validações
- Id válido (Guid)
- Funcionário existe
- Apenas GERENTE pode desativar

#### 3.6 Fluxo de Chamadas
Controller → IDeactivateFuncionarioUseCase.ExecuteAsync(DeactivateFuncionarioRequest) → IFuncionarioRepository.GetByIdAsync → Funcionario.Deactivate → IFuncionarioRepository.UpdateAsync → AuditLogRepository.AddAsync → Retorno void

#### 3.7 Retorno
- **Tipo:** void
- **Códigos HTTP:** 204 No Content (sucesso), 404 Not Found (não encontrado), 403 Forbidden (sem permissão)

#### 3.8 Dependências
- Controller injeta: IDeactivateFuncionarioUseCase
- Use Case injeta: IFuncionarioRepository, IAuditLogRepository

#### 3.9 Regras de Negócio
- Apenas GERENTE pode desativar (RF01)
- Desativação deve ser auditada (RNF02)

### 4. GetFuncionarioById

#### 4.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/funcionario/{id}`

#### 4.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Response)

#### 4.3 Nome dos Arquivos
- Método em `Funcionario.Web/Controllers/FuncionarioController.cs`
- `Funcionario.Application/UseCases/GetFuncionarioById/IGetFuncionarioByIdUseCase.cs`
- `Funcionario.Application/UseCases/GetFuncionarioById/GetFuncionarioByIdUseCase.cs`
- `Funcionario.Application/UseCases/GetFuncionarioById/GetFuncionarioByIdResponse.cs`

#### 4.4 Responsabilidades
- **Controller:** Receber id, chamar use case, retornar response
- **Use Case:** Buscar funcionário por ID, mapear para DTO
- **Não deve:** Controller não deve buscar dados

#### 4.5 Validações
- Id válido (Guid)

#### 4.6 Fluxo de Chamadas
Controller → IGetFuncionarioByIdUseCase.ExecuteAsync(Guid id) → IFuncionarioRepository.GetByIdAsync → Retorno GetFuncionarioByIdResponse

#### 4.7 Retorno
- **Tipo:** GetFuncionarioByIdResponse
- **DTO:** `{ Id: Guid, Name: string, CPF: string, Phone: string, Email: string, IsActive: bool, CreatedAt: DateTime }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found (não encontrado)

#### 4.8 Dependências
- Controller injeta: IGetFuncionarioByIdUseCase
- Use Case injeta: IFuncionarioRepository, IMapper

### 5. GetFuncionarios

#### 4.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/funcionario`

#### 4.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Response)

#### 4.3 Nome dos Arquivos
- Método em `Funcionario.Web/Controllers/FuncionarioController.cs`
- `Funcionario.Application/UseCases/GetFuncionarios/IGetFuncionariosUseCase.cs`
- `Funcionario.Application/UseCases/GetFuncionarios/GetFuncionariosUseCase.cs`
- `Funcionario.Application/UseCases/GetFuncionarios/GetFuncionariosResponse.cs`

#### 4.4 Responsabilidades
- **Controller:** Receber filtros e paginação, chamar use case, retornar lista
- **Use Case:** Buscar lista de funcionários paginada, mapear para DTO
- **Não deve:** Controller não deve buscar dados

#### 4.5 Validações
- Paginação válida (Page > 0, PageSize > 0)

#### 4.6 Fluxo de Chamadas
Controller → IGetFuncionariosUseCase.ExecuteAsync(GetFuncionariosRequest) → IFuncionarioRepository.GetAllAsync → Retorno GetFuncionariosResponse

#### 4.7 Retorno
- **Tipo:** GetFuncionariosResponse
- **DTO:** `{ Items: List<FuncionarioDto>, Total: int, Page: int, PageSize: int }`
- **Códigos HTTP:** 200 OK (sucesso)

#### 4.8 Dependências
- Controller injeta: IGetFuncionariosUseCase
- Use Case injeta: IFuncionarioRepository, IMapper

---

## Produto Module

### 1. CreateProduto

#### 1.1 Nome do Endpoint
- **Método HTTP:** POST
- **Rota:** `/api/v1/produto`

#### 1.2 Arquivos que Devem Ser Criados
- Controller
- Use Case (Interface e Implementação)
- DTOs (Request/Response)
- Repository Interface
- Repository Implementation

#### 1.3 Nome dos Arquivos
- `Produto.Web/Controllers/ProdutoController.cs`
- `Produto.Application/UseCases/CreateProduto/ICreateProdutoUseCase.cs`
- `Produto.Application/UseCases/CreateProduto/CreateProdutoUseCase.cs`
- `Produto.Application/UseCases/CreateProduto/CreateProdutoRequest.cs`
- `Produto.Application/UseCases/CreateProduto/CreateProdutoResponse.cs`
- `Produto.Domain/Repositories/IProdutoRepository.cs`
- `Produto.Infrastructure/Repositories/ProdutoRepository.cs`

#### 1.4 Responsabilidades
- **Controller:** Receber request, chamar use case, retornar 201 Created
- **Use Case:** Criar entidade Produto, validar unicidade, persistir, registrar auditoria
- **Repository:** Adicionar produto ao banco
- **Não deve:** Controller não deve validar regras de negócio

#### 1.5 Validações
- Name único e obrigatório
- Type obrigatório
- Unidade obrigatório
- Price > 0
- Quantity >= 0
- Campos obrigatórios: Name, Type, Unidade, Price, Quantity

#### 1.6 Fluxo de Chamadas
Controller → ICreateProdutoUseCase.ExecuteAsync(CreateProdutoRequest) → IProdutoRepository.ExistsByNameAsync → Produto.Create → IProdutoRepository.AddAsync → AuditLogRepository.AddAsync → Retorno CreateProdutoResponse

#### 1.7 Retorno
- **Tipo:** CreateProdutoResponse
- **DTO:** `{ Id: Guid, Name: string, Type: string, Price: decimal, Quantity: int, IsActive: bool, CreatedAt: DateTime, UpdatedAt: DateTime }`
- **Códigos HTTP:** 201 Created (sucesso), 400 Bad Request (validação), 409 Conflict (duplicado)

#### 1.8 Dependências
- Controller injeta: ICreateProdutoUseCase
- Use Case injeta: IProdutoRepository, IAuditLogRepository, IMapper

#### 1.9 Regras de Negócio
- Name único (RF03)
- Price > 0 (RF03)
- Produto criado ativo por padrão
- Criação deve ser auditada (RNF02)

### 2. GetProdutoById

#### 2.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/produto/{id}`

#### 2.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Response)

#### 2.3 Nome dos Arquivos
- Método em `Produto.Web/Controllers/ProdutoController.cs`
- `Produto.Application/UseCases/GetProdutoById/IGetProdutoByIdUseCase.cs`
- `Produto.Application/UseCases/GetProdutoById/GetProdutoByIdUseCase.cs`
- `Produto.Application/UseCases/GetProdutoById/GetProdutoByIdResponse.cs`

#### 2.4 Responsabilidades
- **Controller:** Receber id, chamar use case, retornar response
- **Use Case:** Buscar produto por ID, mapear para DTO
- **Não deve:** Controller não deve buscar dados

#### 2.5 Validações
- Id válido (Guid)

#### 2.6 Fluxo de Chamadas
Controller → IGetProdutoByIdUseCase.ExecuteAsync(Guid id) → IProdutoRepository.GetByIdAsync → Retorno GetProdutoByIdResponse

#### 2.7 Retorno
- **Tipo:** GetProdutoByIdResponse
- **DTO:** `{ Id: Guid, Name: string, Type: string, Price: decimal, Quantity: int, IsActive: bool, CreatedAt: DateTime, UpdatedAt: DateTime }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found (não encontrado)

#### 2.8 Dependências
- Controller injeta: IGetProdutoByIdUseCase
- Use Case injeta: IProdutoRepository, IMapper

### 3. UpdateProduto

#### 3.1 Nome do Endpoint
- **Método HTTP:** PUT
- **Rota:** `/api/v1/produto/{id}`

#### 3.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Request/Response)

#### 3.3 Nome dos Arquivos
- Método em `Produto.Web/Controllers/ProdutoController.cs`
- `Produto.Application/UseCases/UpdateProduto/IUpdateProdutoUseCase.cs`
- `Produto.Application/UseCases/UpdateProduto/UpdateProdutoUseCase.cs`
- `Produto.Application/UseCases/UpdateProduto/UpdateProdutoRequest.cs`
- `Produto.Application/UseCases/UpdateProduto/UpdateProdutoResponse.cs`

#### 3.4 Responsabilidades
- **Controller:** Receber id e request, chamar use case, retornar response
- **Use Case:** Buscar produto, atualizar detalhes, persistir, registrar auditoria
- **Não deve:** Controller não deve buscar entidade

#### 3.5 Validações
- Id válido (Guid)
- Produto existe
- Name único (se alterado)
- Price > 0

#### 3.6 Fluxo de Chamadas
Controller → IUpdateProdutoUseCase.ExecuteAsync(UpdateProdutoRequest) → IProdutoRepository.GetByIdAsync → Produto.Update → IProdutoRepository.UpdateAsync → IAuditService.LogAsync("UPDATE", "Produto", produto.Id) → Retorno UpdateProdutoResponse

#### 3.7 Retorno
- **Tipo:** UpdateProdutoResponse
- **DTO:** `{ Id: Guid, Name: string, Type: string, Price: decimal, Quantity: int, IsActive: bool, UpdatedAt: DateTime }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found (não encontrado), 400 Bad Request (validação)

#### 3.8 Dependências
- Controller injeta: IUpdateProdutoUseCase
- Use Case injeta: IProdutoRepository, IAuditService, IMapper

### 4. UpdateStock

#### 2.1 Nome do Endpoint
- **Método HTTP:** POST
- **Rota:** `/api/v1/produto/{id}/stock`

#### 2.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Request)
- Service (Interface e Implementação)

#### 2.3 Nome dos Arquivos
- Método em `Produto.Web/Controllers/ProdutoController.cs`
- `Produto.Application/UseCases/UpdateStock/IUpdateStockUseCase.cs`
- `Produto.Application/UseCases/UpdateStock/UpdateStockUseCase.cs`
- `Produto.Application/UseCases/UpdateStock/UpdateStockRequest.cs`
- `Produto.Application/Services/IStockMovementService.cs`
- `Produto.Application/Services/StockMovementService.cs`

#### 2.4 Responsabilidades
- **Controller:** Receber id e request, chamar use case, retornar 204 No Content
- **Use Case:** Buscar produto, atualizar estoque, registrar movimento, persistir, registrar auditoria
- **Service:** Criar StockMovement
- **Não deve:** Controller não deve buscar entidade

#### 2.5 Validações
- Id válido (Guid)
- Produto existe e ativo
- Quantity > 0
- Type válido (ENTRADA/SAIDA)
- Estoque não pode ficar negativo em SAIDA

#### 2.6 Fluxo de Chamadas
Controller → IUpdateStockUseCase.ExecuteAsync(UpdateStockRequest) → IProdutoRepository.GetByIdAsync → Produto.DecreaseStock/IncreaseStock → IProdutoRepository.UpdateAsync → IStockMovementService.CreateMovementAsync → IAuditService.LogAsync("UPDATE_STOCK", "Produto", produto.Id) → EstoqueHub.NotifyLowStockAsync (se estoque < 5) → Retorno UpdateStockResponse

#### 2.7 Retorno
- **Tipo:** UpdateStockResponse
- **DTO:** `{ Id: Guid, Name: string, CurrentQuantity: int, LastMovement: string, UpdatedAt: DateTime }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found (não encontrado), 400 Bad Request (validação), 409 Conflict (estoque insuficiente)

#### 2.8 Dependências
- Controller injeta: IUpdateStockUseCase
- Use Case injeta: IProdutoRepository, IStockMovementService, IAuditService, IHubContext<EstoqueHub>

#### 2.9 Regras de Negócio
- Bloquear estoque negativo (RF06, RN06)
- Registrar StockMovement (RN06)
- Atualização deve ser auditada (RNF02)

### 3. GetProdutos

#### 3.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/produto`

#### 3.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Response)

#### 3.3 Nome dos Arquivos
- Método em `Produto.Web/Controllers/ProdutoController.cs`
- `Produto.Application/UseCases/GetProdutos/IGetProdutosUseCase.cs`
- `Produto.Application/UseCases/GetProdutos/GetProdutosUseCase.cs`
- `Produto.Application/UseCases/GetProdutos/GetProdutosResponse.cs`

#### 3.4 Responsabilidades
- **Controller:** Receber filtros e paginação, chamar use case, retornar lista
- **Use Case:** Buscar lista de produtos paginada, mapear para DTO
- **Não deve:** Controller não deve buscar dados

#### 3.5 Validações
- Paginação válida (Page > 0, PageSize > 0)

#### 3.6 Fluxo de Chamadas
Controller → IGetProdutosUseCase.ExecuteAsync(GetProdutosRequest) → IProdutoRepository.GetAllAsync → Retorno GetProdutosResponse

#### 3.7 Retorno
- **Tipo:** GetProdutosResponse
- **DTO:** `{ Items: List<ProdutoDto>, Total: int, Page: int, PageSize: int }`
- **Códigos HTTP:** 200 OK (sucesso)

#### 3.8 Dependências
- Controller injeta: IGetProdutosUseCase
- Use Case injeta: IProdutoRepository, IMapper

---

## Cliente Module

### 1. CreateCliente

#### 1.1 Nome do Endpoint
- **Método HTTP:** POST
- **Rota:** `/api/v1/cliente`

#### 1.2 Arquivos que Devem Ser Criados
- Controller
- Use Case (Interface e Implementação)
- DTOs (Request/Response)
- Repository Interface
- Repository Implementation

#### 1.3 Nome dos Arquivos
- `Cliente.Web/Controllers/ClienteController.cs`
- `Cliente.Application/UseCases/CreateCliente/ICreateClienteUseCase.cs`
- `Cliente.Application/UseCases/CreateCliente/CreateClienteUseCase.cs`
- `Cliente.Application/UseCases/CreateCliente/CreateClienteRequest.cs`
- `Cliente.Application/UseCases/CreateCliente/CreateClienteResponse.cs`
- `Cliente.Domain/Repositories/IClienteRepository.cs`
- `Cliente.Infrastructure/Repositories/ClienteRepository.cs`

#### 1.4 Responsabilidades
- **Controller:** Receber request, chamar use case, retornar 201 Created
- **Use Case:** Criar entidade Cliente, validar unicidade, adicionar endereço, persistir, registrar auditoria
- **Repository:** Adicionar cliente ao banco
- **Não deve:** Controller não deve validar regras de negócio

#### 1.5 Validações
- Name obrigatório
- CpfCnpj único e válido (CPF ou CNPJ conforme Type)
- Type obrigatório (PF/PJ)
- Phone obrigatório
- Address obrigatório (pelo menos um)
- Campos obrigatórios: Name, CpfCnpj, Type, Phone, Addresses

#### 1.6 Fluxo de Chamadas
Controller → ICreateClienteUseCase.ExecuteAsync(CreateClienteRequest) → IClienteRepository.ExistsByCpfCnpjAsync → Cliente.Create → Cliente.AddAddress → IClienteRepository.AddAsync → IAuditService.LogAsync("CREATE", "Cliente", cliente.Id) → Retorno CreateClienteResponse

#### 1.7 Retorno
- **Tipo:** CreateClienteResponse
- **DTO:** `{ Id: Guid, Name: string, CpfCnpj: string, Type: string, Phone: string, Addresses: List<AddressDto>, IsActive: bool, CreatedAt: DateTime }`
- **Códigos HTTP:** 201 Created (sucesso), 400 Bad Request (validação), 409 Conflict (duplicado)

#### 1.8 Dependências
- Controller injeta: ICreateClienteUseCase
- Use Case injeta: IClienteRepository, IAuditService, IMapper

#### 1.9 Regras de Negócio
- CpfCnpj único (RN09)
- Endereço obrigatório (DER)
- Cliente criado ativo por padrão
- Criação deve ser auditada (RNF02)

### 2. GetClienteById

#### 2.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/cliente/{id}`

#### 2.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Response)

#### 2.3 Nome dos Arquivos
- Método em `Cliente.Web/Controllers/ClienteController.cs`
- `Cliente.Application/UseCases/GetClienteById/IGetClienteByIdUseCase.cs`
- `Cliente.Application/UseCases/GetClienteById/GetClienteByIdUseCase.cs`
- `Cliente.Application/UseCases/GetClienteById/GetClienteByIdResponse.cs`

#### 2.4 Responsabilidades
- **Controller:** Receber id, chamar use case, retornar response
- **Use Case:** Buscar cliente por ID, mapear para DTO
- **Não deve:** Controller não deve buscar dados

#### 2.5 Validações
- Id válido (Guid)

#### 2.6 Fluxo de Chamadas
Controller → IGetClienteByIdUseCase.ExecuteAsync(Guid id) → IClienteRepository.GetByIdAsync → Retorno GetClienteByIdResponse

#### 2.7 Retorno
- **Tipo:** GetClienteByIdResponse
- **DTO:** `{ Id: Guid, Name: string, CpfCnpj: string, Type: string, Phone: string, Email: string, Addresses: List<AddressDto>, IsActive: bool, CreatedAt: DateTime }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found (não encontrado)

#### 2.8 Dependências
- Controller injeta: IGetClienteByIdUseCase
- Use Case injeta: IClienteRepository, IMapper

### 3. GetClientes

#### 3.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/cliente`

#### 3.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Response)

#### 3.3 Nome dos Arquivos
- Método em `Cliente.Web/Controllers/ClienteController.cs`
- `Cliente.Application/UseCases/GetClientes/IGetClientesUseCase.cs`
- `Cliente.Application/UseCases/GetClientes/GetClientesUseCase.cs`
- `Cliente.Application/UseCases/GetClientes/GetClientesResponse.cs`

#### 3.4 Responsabilidades
- **Controller:** Receber filtros e paginação, chamar use case, retornar lista
- **Use Case:** Buscar lista de clientes paginada, mapear para DTO
- **Não deve:** Controller não deve buscar dados

#### 3.5 Validações
- Paginação válida (Page > 0, PageSize > 0)

#### 3.6 Fluxo de Chamadas
Controller → IGetClientesUseCase.ExecuteAsync(GetClientesRequest) → IClienteRepository.GetAllAsync → Retorno GetClientesResponse

#### 3.7 Retorno
- **Tipo:** GetClientesResponse
- **DTO:** `{ Items: List<ClienteDto>, Total: int, Page: int, PageSize: int }`
- **Códigos HTTP:** 200 OK (sucesso)

#### 3.8 Dependências
- Controller injeta: IGetClientesUseCase
- Use Case injeta: IClienteRepository, IMapper

### 4. UpdateCliente

#### 4.1 Nome do Endpoint
- **Método HTTP:** PUT
- **Rota:** `/api/v1/cliente/{id}`

#### 4.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Request/Response)

#### 4.3 Nome dos Arquivos
- Método em `Cliente.Web/Controllers/ClienteController.cs`
- `Cliente.Application/UseCases/UpdateCliente/IUpdateClienteUseCase.cs`
- `Cliente.Application/UseCases/UpdateCliente/UpdateClienteUseCase.cs`
- `Cliente.Application/UseCases/UpdateCliente/UpdateClienteRequest.cs`
- `Cliente.Application/UseCases/UpdateCliente/UpdateClienteResponse.cs`

#### 4.4 Responsabilidades
- **Controller:** Receber id e request, chamar use case, retornar response
- **Use Case:** Buscar cliente, atualizar detalhes, persistir, registrar auditoria
- **Não deve:** Controller não deve buscar entidade

#### 4.5 Validações
- Id válido (Guid)
- Cliente existe
- Name obrigatório
- Phone obrigatório

#### 4.6 Fluxo de Chamadas
Controller → IUpdateClienteUseCase.ExecuteAsync(UpdateClienteRequest) → IClienteRepository.GetByIdAsync → Cliente.UpdateProfile → IClienteRepository.UpdateAsync → IAuditService.LogAsync("UPDATE", "Cliente", cliente.Id) → Retorno UpdateClienteResponse

#### 4.7 Retorno
- **Tipo:** UpdateClienteResponse
- **DTO:** `{ Id: Guid, Name: string, Phone: string, Email: string, UpdatedAt: DateTime }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found (não encontrado), 400 Bad Request (validação)

#### 4.8 Dependências
- Controller injeta: IUpdateClienteUseCase
- Use Case injeta: IClienteRepository, IAuditService, IMapper

### 5. GetConsumptionHistory

#### 5.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/cliente/{id}/consumptionHistory`

#### 5.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Response)

#### 5.3 Nome dos Arquivos
- Método em `Cliente.Web/Controllers/ClienteController.cs`
- `Cliente.Application/UseCases/GetConsumptionHistory/IGetConsumptionHistoryUseCase.cs`
- `Cliente.Application/UseCases/GetConsumptionHistory/GetConsumptionHistoryUseCase.cs`
- `Cliente.Application/UseCases/GetConsumptionHistory/GetConsumptionHistoryResponse.cs`

#### 5.4 Responsabilidades
- **Controller:** Receber id e filtros, chamar use case, retornar histórico
- **Use Case:** Buscar histórico de vendas e entregas de planos por cliente (RF07)
- **Não deve:** Controller não deve consolidar dados

#### 5.5 Validações
- Id válido (Guid)
- Cliente existe

#### 5.6 Fluxo de Chamadas
Controller → IGetConsumptionHistoryUseCase.ExecuteAsync(GetConsumptionHistoryRequest) → IVendaRepository.GetByCustomerIdAsync → IDeliveryRepository.GetByCustomerIdAsync → Retorno GetConsumptionHistoryResponse

#### 5.7 Retorno
- **Tipo:** GetConsumptionHistoryResponse
- **DTO:** `{ Items: List<ConsumptionItemDto>, TotalQuantity: int, TotalSpent: decimal }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found (cliente não encontrado)

#### 5.8 Dependências
- Controller injeta: IGetConsumptionHistoryUseCase
- Use Case injeta: IVendaRepository, IDeliveryRepository, IMapper
---

## Venda Module

### 1. CreateVenda

#### 1.1 Nome do Endpoint
- **Método HTTP:** POST
- **Rota:** `/api/v1/venda`

#### 1.2 Arquivos que Devem Ser Criados
- Controller
- Use Case (Interface e Implementação)
- DTOs (Request/Response)
- Repository Interface
- Repository Implementation
- Service

#### 1.3 Nome dos Arquivos
- `Venda.Web/Controllers/VendaController.cs`
- `Venda.Application/UseCases/CreateVenda/ICreateVendaUseCase.cs`
- `Venda.Application/UseCases/CreateVenda/CreateVendaUseCase.cs`
- `Venda.Application/UseCases/CreateVenda/CreateVendaRequest.cs`
- `Venda.Application/UseCases/CreateVenda/CreateVendaResponse.cs`
- `Venda.Domain/Repositories/IVendaRepository.cs`
- `Venda.Infrastructure/Repositories/VendaRepository.cs`

#### 1.4 Responsabilidades
- **Controller:** Receber request, chamar use case, retornar 201 Created
- **Use Case:** Criar entidade Venda, validar estoque, decrementar estoque, registrar movimento, persistir, registrar auditoria
- **Repository:** Adicionar venda ao banco
- **Service:** Registrar StockMovement
- **Não deve:** Controller não deve validar regras de negócio

#### 1.5 Validações
- Items obrigatório e não vazio
- Quantity por item > 0
- Estoque suficiente para todos os itens
- Discount apenas para GERENTE
- CustomerId opcional

#### 1.6 Fluxo de Chamadas
Controller → ICreateVendaUseCase.ExecuteAsync(CreateVendaRequest) → IProdutoRepository.GetByIdsAsync → Venda.Create → Venda.Confirm → IProdutoRepository.UpdateAsync → IVendaRepository.AddAsync → IStockMovementService.CreateMovementsAsync → IAuditService.LogAsync("CREATE", "Venda", venda.Id) → Retorno CreateVendaResponse

#### 1.7 Retorno
- **Tipo:** CreateVendaResponse
- **DTO:** `{ Id: Guid, Total: decimal, Items: List<VendaItemDto>, Date: DateTime }`
- **Códigos HTTP:** 201 Created (sucesso), 400 Bad Request (validação), 409 Conflict (estoque insuficiente), 403 Forbidden (desconto não autorizado)

#### 1.8 Dependências
- Controller injeta: ICreateVendaUseCase
- Use Case injeta: IVendaRepository, IProdutoRepository, IStockMovementService, IAuditService, IMapper

#### 1.9 Regras de Negócio
- Bloquear venda se estoque insuficiente (RF06)
- Baixa imediata de estoque (RN06)
- Registrar StockMovement (RN06)
- Desconto restrito a GERENTE (RN07)
- Venda deve ser auditada (RNF02)

### 2. GetVendaById

#### 2.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/venda/{id}`

#### 2.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Response)

#### 2.3 Nome dos Arquivos
- Método em `Venda.Web/Controllers/VendaController.cs`
- `Venda.Application/UseCases/GetVendaById/IGetVendaByIdUseCase.cs`
- `Venda.Application/UseCases/GetVendaById/GetVendaByIdUseCase.cs`
- `Venda.Application/UseCases/GetVendaById/GetVendaByIdResponse.cs`

#### 2.4 Responsabilidades
- **Controller:** Receber id, chamar use case, retornar response
- **Use Case:** Buscar venda por ID, mapear para DTO
- **Não deve:** Controller não deve buscar dados

#### 2.5 Validações
- Id válido (Guid)

#### 2.6 Fluxo de Chamadas
Controller → IGetVendaByIdUseCase.ExecuteAsync(Guid id) → IVendaRepository.GetByIdAsync → Retorno GetVendaByIdResponse

#### 2.7 Retorno
- **Tipo:** GetVendaByIdResponse
- **DTO:** `{ Id: Guid, CustomerId: Guid?, EmployeeId: Guid, Total: decimal, Discount: double, Items: List<VendaItemDto>, Date: DateTime }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found (não encontrado)

#### 2.8 Dependências
- Controller injeta: IGetVendaByIdUseCase
- Use Case injeta: IVendaRepository, IMapper

### 3. GetVendas

#### 3.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/venda`

#### 3.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Response)

#### 3.3 Nome dos Arquivos
- Método em `Venda.Web/Controllers/VendaController.cs`
- `Venda.Application/UseCases/GetVendas/IGetVendasUseCase.cs`
- `Venda.Application/UseCases/GetVendas/GetVendasUseCase.cs`
- `Venda.Application/UseCases/GetVendas/GetVendasResponse.cs`

#### 3.4 Responsabilidades
- **Controller:** Receber filtros e paginação, chamar use case, retornar lista
- **Use Case:** Buscar lista de vendas paginada, mapear para DTO
- **Não deve:** Controller não deve buscar dados

#### 3.5 Validações
- Paginação válida (Page > 0, PageSize > 0)

#### 3.6 Fluxo de Chamadas
Controller → IGetVendasUseCase.ExecuteAsync(GetVendasRequest) → IVendaRepository.GetAllAsync → Retorno GetVendasResponse

#### 3.7 Retorno
- **Tipo:** GetVendasResponse
- **DTO:** `{ Items: List<VendaDto>, Total: int, Page: int, PageSize: int }`
- **Códigos HTTP:** 200 OK (sucesso)

#### 3.8 Dependências
- Controller injeta: IGetVendasUseCase
- Use Case injeta: IVendaRepository, IMapper

### 5. ConfirmVenda

#### 5.1 Nome do Endpoint
- **Método HTTP:** POST
- **Rota:** `/api/v1/venda/{id}/confirm`

#### 5.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Response)

#### 5.3 Nome dos Arquivos
- Método em `Venda.Web/Controllers/VendaController.cs`
- `Venda.Application/UseCases/ConfirmVenda/IConfirmVendaUseCase.cs`
- `Venda.Application/UseCases/ConfirmVenda/ConfirmVendaUseCase.cs`
- `Venda.Application/UseCases/ConfirmVenda/ConfirmVendaResponse.cs`

#### 5.4 Responsabilidades
- **Controller:** Receber id, chamar use case, retornar response
- **Use Case:** Buscar venda, confirmar, decrementar estoque, registrar movimento, persistir, registrar auditoria
- **Não deve:** Controller não deve alterar estado de estoque

#### 5.5 Validações
- Id válido (Guid)
- Venda existe e não confirmada
- Estoque ainda disponível

#### 5.6 Fluxo de Chamadas
Controller → IConfirmVendaUseCase.ExecuteAsync(Guid id) → IVendaRepository.GetByIdAsync → Venda.Confirm → IProdutoRepository.UpdateAsync → IVendaRepository.UpdateAsync → IStockMovementService.CreateMovementsAsync → IAuditService.LogAsync("UPDATE", "Venda", venda.Id) → Retorno ConfirmVendaResponse

#### 5.7 Retorno
- **Tipo:** ConfirmVendaResponse
- **DTO:** `{ Id: Guid, Status: string, ConfirmedAt: DateTime }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found, 409 Conflict (estoque)

#### 5.8 Dependências
- Controller injeta: IConfirmVendaUseCase
- Use Case injeta: IVendaRepository, IProdutoRepository, IStockMovementService, IAuditService, IMapper

---

## Plano Module

### 1. CreatePlano

#### 1.1 Nome do Endpoint
- **Método HTTP:** POST
- **Rota:** `/api/v1/plano`

#### 1.2 Arquivos que Devem Ser Criados
- Controller
- Use Case (Interface e Implementação)
- DTOs (Request/Response)
- Repository Interface
- Repository Implementation

#### 1.3 Nome dos Arquivos
- `Plano.Web/Controllers/PlanoController.cs`
- `Plano.Application/UseCases/CreatePlano/ICreatePlanoUseCase.cs`
- `Plano.Application/UseCases/CreatePlano/CreatePlanoUseCase.cs`
- `Plano.Application/UseCases/CreatePlano/CreatePlanoRequest.cs`
- `Plano.Application/UseCases/CreatePlano/CreatePlanoResponse.cs`
- `Plano.Domain/Repositories/IPlanoRepository.cs`
- `Plano.Infrastructure/Repositories/PlanoRepository.cs`

#### 1.4 Responsabilidades
- **Controller:** Receber request, chamar use case, retornar 201 Created
- **Use Case:** Criar entidade Plano, gerar deliveries, persistir, registrar auditoria
- **Repository:** Adicionar plano ao banco
- **Não deve:** Controller não deve validar regras de negócio

#### 1.5 Validações
- CustomerId obrigatório
- Items obrigatório e não vazio
- Cycle válido
- Discount apenas para GERENTE

#### 1.6 Fluxo de Chamadas
Controller → ICreatePlanoUseCase.ExecuteAsync(CreatePlanoRequest) → IClienteRepository.GetByIdAsync → Plano.Create → Plano.GenerateDeliveries → IPlanoRepository.AddAsync → IDeliveryRepository.AddRangeAsync → AuditLogRepository.AddAsync → Retorno CreatePlanoResponse

#### 1.7 Retorno
- **Tipo:** CreatePlanoResponse
- **DTO:** `{ Id: Guid, Total: decimal, Items: List<PlanoItemDto>, StartDate: DateTime, EndDate: DateTime }`
- **Códigos HTTP:** 201 Created (sucesso), 400 Bad Request (validação), 403 Forbidden (desconto não autorizado)

#### 1.8 Dependências
- Controller injeta: ICreatePlanoUseCase
- Use Case injeta: IPlanoRepository, IClienteRepository, IDeliveryRepository, IAuditLogRepository, IMapper

#### 1.9 Regras de Negócio
- Planos customizáveis (RN05)
- Desconto restrito a GERENTE (RN07)
- Criação deve ser auditada (RNF02)

### 2. GetPlanoById

#### 2.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/plano/{id}`

#### 2.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Response)

#### 2.3 Nome dos Arquivos
- Método em `Plano.Web/Controllers/PlanoController.cs`
- `Plano.Application/UseCases/GetPlanoById/IGetPlanoByIdUseCase.cs`
- `Plano.Application/UseCases/GetPlanoById/GetPlanoByIdUseCase.cs`
- `Plano.Application/UseCases/GetPlanoById/GetPlanoByIdResponse.cs`

#### 2.4 Responsabilidades
- **Controller:** Receber id, chamar use case, retornar response
- **Use Case:** Buscar plano por ID, mapear para DTO
- **Não deve:** Controller não deve buscar dados

#### 2.5 Validações
- Id válido (Guid)

#### 2.6 Fluxo de Chamadas
Controller → IGetPlanoByIdUseCase.ExecuteAsync(Guid id) → IPlanoRepository.GetByIdAsync → Retorno GetPlanoByIdResponse

#### 2.7 Retorno
- **Tipo:** GetPlanoByIdResponse
- **DTO:** `{ Id: Guid, CustomerId: Guid, Cycle: string, Total: decimal, Discount: double, StartDate: DateTime, EndDate: DateTime, IsActive: bool, Items: List<PlanoItemDto>, Deliveries: List<DeliveryDto> }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found (não encontrado)

#### 2.8 Dependências
- Controller injeta: IGetPlanoByIdUseCase
- Use Case injeta: IPlanoRepository, IMapper

### 3. GetPlanos

#### 3.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/plano`

#### 3.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Response)

#### 3.3 Nome dos Arquivos
- Método em `Plano.Web/Controllers/PlanoController.cs`
- `Plano.Application/UseCases/GetPlanos/IGetPlanosUseCase.cs`
- `Plano.Application/UseCases/GetPlanos/GetPlanosUseCase.cs`
- `Plano.Application/UseCases/GetPlanos/GetPlanosResponse.cs`

#### 3.4 Responsabilidades
- **Controller:** Receber filtros e paginação, chamar use case, retornar lista
- **Use Case:** Buscar lista de planos paginada, mapear para DTO
- **Não deve:** Controller não deve buscar dados

#### 3.5 Validações
- Paginação válida (Page > 0, PageSize > 0)

#### 3.6 Fluxo de Chamadas
Controller → IGetPlanosUseCase.ExecuteAsync(GetPlanosRequest) → IPlanoRepository.GetAllAsync → Retorno GetPlanosResponse

#### 3.7 Retorno
- **Tipo:** GetPlanosResponse
- **DTO:** `{ Items: List<PlanoDto>, Total: int, Page: int, PageSize: int }`
- **Códigos HTTP:** 200 OK (sucesso)

#### 3.8 Dependências
- Controller injeta: IGetPlanosUseCase
- Use Case injeta: IPlanoRepository, IMapper

### 4. UpdatePlano

#### 4.1 Nome do Endpoint
- **Método HTTP:** PUT
- **Rota:** `/api/v1/plano/{id}`

#### 4.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Request/Response)

#### 4.3 Nome dos Arquivos
- Método em `Plano.Web/Controllers/PlanoController.cs`
- `Plano.Application/UseCases/UpdatePlano/IUpdatePlanoUseCase.cs`
- `Plano.Application/UseCases/UpdatePlano/UpdatePlanoUseCase.cs`
- `Plano.Application/UseCases/UpdatePlano/UpdatePlanoRequest.cs`
- `Plano.Application/UseCases/UpdatePlano/UpdatePlanoResponse.cs`

#### 4.4 Responsabilidades
- **Controller:** Receber id e request, chamar use case, retornar response
- **Use Case:** Buscar plano, atualizar detalhes (exceto itens/ciclo que são via upgrade/downgrade), persistir, registrar auditoria
- **Não deve:** Controller não deve buscar entidade

#### 4.5 Validações
- Id válido (Guid)
- Plano existe e ativo

#### 4.6 Fluxo de Chamadas
Controller → IUpdatePlanoUseCase.ExecuteAsync(UpdatePlanoRequest) → IPlanoRepository.GetByIdAsync → Plano.UpdateDetails → IPlanoRepository.UpdateAsync → IAuditService.LogAsync("UPDATE", "Plano", plano.Id) → Retorno UpdatePlanoResponse

#### 4.7 Retorno
- **Tipo:** UpdatePlanoResponse
- **DTO:** `{ Id: Guid, Total: decimal, UpdatedAt: DateTime }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found, 400 Bad Request

#### 4.8 Dependências
- Controller injeta: IUpdatePlanoUseCase
- Use Case injeta: IPlanoRepository, IAuditService, IMapper

### 5. UpgradePlano

#### 2.1 Nome do Endpoint
- **Método HTTP:** PUT
- **Rota:** `/api/v1/plano/{id}/upgrade`

#### 2.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Request/Response)

#### 2.3 Nome dos Arquivos
- Método em `Plano.Web/Controllers/PlanoController.cs`
- `Plano.Application/UseCases/UpgradePlano/IUpgradePlanoUseCase.cs`
- `Plano.Application/UseCases/UpgradePlano/UpgradePlanoUseCase.cs`
- `Plano.Application/UseCases/UpgradePlano/UpgradePlanoRequest.cs`
- `Plano.Application/UseCases/UpgradePlano/UpgradePlanoResponse.cs`

#### 2.4 Responsabilidades
- **Controller:** Receber id e request, chamar use case, retornar 200 OK
- **Use Case:** Buscar plano, fazer upgrade, persistir, registrar auditoria
- **Não deve:** Controller não deve buscar entidade

#### 2.5 Validações
- Id válido (Guid)
- Plano existe e ativo
- NewItems obrigatório e não vazio

#### 2.6 Fluxo de Chamadas
Controller → IUpgradePlanoUseCase.ExecuteAsync(UpgradePlanoRequest) → IPlanoRepository.GetByIdAsync → Plano.Upgrade → IPlanoRepository.UpdateAsync → IAuditService.LogAsync("UPDATE", "Plano", plano.Id) → Retorno UpgradePlanoResponse

#### 2.7 Retorno
- **Tipo:** UpgradePlanoResponse
- **DTO:** `{ Id: Guid, Total: decimal, Items: List<PlanoItemDto> }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found (plano não encontrado), 400 Bad Request (validação)

#### 2.8 Dependências
- Controller injeta: IUpgradePlanoUseCase
- Use Case injeta: IPlanoRepository, IAuditService, IMapper

#### 2.9 Regras de Negócio
- Upgrade sem multa (RN08)
- Upgrade deve ser auditado (RNF02)

### 3. DowngradePlano

#### 3.1 Nome do Endpoint
- **Método HTTP:** PUT
- **Rota:** `/api/v1/plano/{id}/downgrade`

#### 3.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Request/Response)
- Service (Interface e Implementação)

#### 3.3 Nome dos Arquivos
- Método em `Plano.Web/Controllers/PlanoController.cs`
- `Plano.Application/UseCases/DowngradePlano/IDowngradePlanoUseCase.cs`
- `Plano.Application/UseCases/DowngradePlano/DowngradePlanoUseCase.cs`
- `Plano.Application/UseCases/DowngradePlano/DowngradePlanoRequest.cs`
- `Plano.Application/UseCases/DowngradePlano/DowngradePlanoResponse.cs`
- `Plano.Application/Services/IContractPenaltyService.cs`
- `Plano.Application/Services/ContractPenaltyService.cs`

#### 3.4 Responsabilidades
- **Controller:** Receber id e request, chamar use case, retornar 200 OK
- **Use Case:** Buscar plano, fazer downgrade, calcular multa, persistir, registrar auditoria
- **Service:** Criar ContractPenalty
- **Não deve:** Controller não deve buscar entidade

#### 3.5 Validações
- Id válido (Guid)
- Plano existe e ativo
- NewItems obrigatório e não vazio

#### 3.6 Fluxo de Chamadas
Controller → IDowngradePlanoUseCase.ExecuteAsync(DowngradePlanoRequest) → IPlanoRepository.GetByIdAsync → Plano.Downgrade → IContractPenaltyService.CreatePenaltyAsync → IPlanoRepository.UpdateAsync → IAuditService.LogAsync("UPDATE", "Plano", plano.Id) → Retorno DowngradePlanoResponse

#### 3.7 Retorno
- **Tipo:** DowngradePlanoResponse
- **DTO:** `{ Id: Guid, Total: decimal, Items: List<PlanoItemDto>, Penalty: ContractPenaltyDto }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found (plano não encontrado), 400 Bad Request (validação)

#### 3.8 Dependências
- Controller injeta: IDowngradePlanoUseCase
- Use Case injeta: IPlanoRepository, IContractPenaltyService, IAuditService, IMapper

#### 3.9 Regras de Negócio
- Downgrade gera multa (RN08)
- Multa calculada sobre valor remanescente (RN08)
- Downgrade deve ser auditado (RNF02)

### 4. CancelPlano

#### 4.1 Nome do Endpoint
- **Método HTTP:** DELETE
- **Rota:** `/api/v1/plano/{id}`

#### 4.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Request)
- Service (já existente)

#### 4.3 Nome dos Arquivos
- Método em `Plano.Web/Controllers/PlanoController.cs`
- `Plano.Application/UseCases/CancelPlano/ICancelPlanoUseCase.cs`
- `Plano.Application/UseCases/CancelPlano/CancelPlanoUseCase.cs`
- `Plano.Application/UseCases/CancelPlano/CancelPlanoRequest.cs`

#### 4.4 Responsabilidades
- **Controller:** Receber id, chamar use case, retornar 204 No Content
- **Use Case:** Buscar plano, cancelar, calcular multa, persistir, registrar auditoria
- **Service:** Criar ContractPenalty
- **Não deve:** Controller não deve buscar entidade

#### 4.5 Validações
- Id válido (Guid)
- Plano existe e ativo

#### 4.6 Fluxo de Chamadas
Controller → ICancelPlanoUseCase.ExecuteAsync(CancelPlanoRequest) → IPlanoRepository.GetByIdAsync → Plano.Cancel → IContractPenaltyService.CreatePenaltyAsync → IPlanoRepository.UpdateAsync → IAuditService.LogAsync("DELETE", "Plano", plano.Id) → Retorno CancelPlanoResponse

#### 4.7 Retorno
- **Tipo:** CancelPlanoResponse
- **DTO:** `{ Id: Guid, Status: string, PenaltyAmount: decimal, CancelledAt: DateTime }`
- **Códigos HTTP:** 200 OK (sucesso), 404 Not Found (plano não encontrado)

#### 4.8 Dependências
- Controller injeta: ICancelPlanoUseCase
- Use Case injeta: IPlanoRepository, IContractPenaltyService, IAuditService

#### 4.9 Regras de Negócio
- Cancelamento gera multa (RN08)
- Multa calculada sobre valor remanescente (RN08)
- Cancelamento deve ser auditado (RNF02)

---

## Shared Module

### AuditService

#### Arquivos que Devem Ser Criados
- Service (Interface e Implementação)

#### Nome dos Arquivos
- `Shared.Application/Services/IAuditService.cs`
- `Shared.Application/Services/AuditService.cs`

#### Responsabilidades
- **IAuditService:** Definir contrato para registro de auditoria
- **AuditService:** Registrar operações críticas no AuditLog

#### Métodos
- LogAsync(string action, string entityType, Guid entityId, string oldValues = null, string newValues = null)

#### Dependências
- AuditService injeta: IAuditLogRepository

#### Regras de Negócio
- Registrar em: Login, CreateVenda, UpdatePlano, CancelPlano
---

## Relatórios Module (Exclusivo Gestor)

### 1. GetSalesReport

#### 1.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/relatorio/vendas`

#### 1.2 Arquivos que Devem Ser Criados
- Controller
- Use Case (Interface e Implementação)
- DTOs (Request/Response)

#### 1.3 Nome dos Arquivos
- `Relatorio.Web/Controllers/RelatorioController.cs`
- `Relatorio.Application/UseCases/GetSalesReport/IGetSalesReportUseCase.cs`
- `Relatorio.Application/UseCases/GetSalesReport/GetSalesReportUseCase.cs`
- `Relatorio.Application/UseCases/GetSalesReport/GetSalesReportRequest.cs`
- `Relatorio.Application/UseCases/GetSalesReport/GetSalesReportResponse.cs`

#### 1.4 Responsabilidades
- **Controller:** Receber filtros (período), chamar use case, retornar response
- **Use Case:** Consolidar dados de vendas avulsas e contratos recorrentes (RF08)
- **Não deve:** Use Case não deve conter lógica de apresentação

#### 1.5 Validações
- Apenas GERENTE pode acessar (RF08, RN07)
- StartDate <= EndDate

#### 1.6 Fluxo de Chamadas
Controller → IGetSalesReportUseCase.ExecuteAsync(GetSalesReportRequest) → IVendaRepository.GetByPeriodAsync → IPlanoRepository.GetByPeriodAsync → Retorno GetSalesReportResponse

#### 1.7 Retorno
- **Tipo:** GetSalesReportResponse
- **DTO:** `{ TotalVendasAvulsas: decimal, TotalVendasContratos: decimal, TotalGeral: decimal, Items: List<SalesReportItemDto> }`
- **Códigos HTTP:** 200 OK (sucesso), 403 Forbidden (sem permissão)

#### 1.8 Dependências
- Controller injeta: IGetSalesReportUseCase
- Use Case injeta: IVendaRepository, IPlanoRepository, IMapper

#### 1.8 Dependências
- Controller injeta: IGetAuditLogsUseCase
- Use Case injeta: IAuditLogRepository, IMapper

#### 1.9 Regras de Negócio
- Acesso exclusivo a GERENTE (RF01)
- Logs imutáveis (RNF02)

### 2. GetStockMovements

#### 2.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/shared/stockmovements`

#### 2.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Request/Response)
- Repository Interface
- Repository Implementation

#### 2.3 Nome dos Arquivos
- Método em `Shared.Web/Controllers/SharedController.cs`
- `Shared.Application/UseCases/GetStockMovements/IGetStockMovementsUseCase.cs`
- `Shared.Application/UseCases/GetStockMovements/GetStockMovementsUseCase.cs`
- `Shared.Application/UseCases/GetStockMovements/GetStockMovementsRequest.cs`
- `Shared.Application/UseCases/GetStockMovements/GetStockMovementsResponse.cs`
- `Shared.Domain/Repositories/IStockMovementRepository.cs`
- `Shared.Infrastructure/Repositories/StockMovementRepository.cs`

#### 2.4 Responsabilidades
- **Controller:** Receber query parameters, chamar use case, retornar response
- **Use Case:** Filtrar movimentos por produto, data, tipo, retornar lista
- **Repository:** Buscar movimentos no banco
- **Não deve:** Controller não deve filtrar dados

#### 2.5 Validações
- Apenas GERENTE pode acessar
- StartDate <= EndDate se fornecidos

#### 2.6 Fluxo de Chamadas
Controller → IGetStockMovementsUseCase.ExecuteAsync(GetStockMovementsRequest) → IStockMovementRepository.GetFilteredAsync → Retorno GetStockMovementsResponse

#### 2.7 Retorno
- **Tipo:** GetStockMovementsResponse
- **DTO:** `{ Movements: List<StockMovementDto>, Total: int }`
- **Códigos HTTP:** 200 OK (sucesso), 403 Forbidden (sem permissão), 400 Bad Request (validação)

#### 1.8 Dependências
- Controller injeta: IGetStockMovementsUseCase
- Use Case injeta: IStockMovementRepository, IMapper

#### 1.9 Regras de Negócio
- Acesso exclusivo a GERENTE (RF01)
- Movimentos imutáveis (RN06)

### 3. GetContractPenalties

#### 3.1 Nome do Endpoint
- **Método HTTP:** GET
- **Rota:** `/api/v1/shared/contractpenalties`

#### 3.2 Arquivos que Devem Ser Criados
- Controller (adicionar método)
- Use Case (Interface e Implementação)
- DTOs (Request/Response)
- Repository Interface
- Repository Implementation

#### 3.3 Nome dos Arquivos
- Método em `Shared.Web/Controllers/SharedController.cs`
- `Shared.Application/UseCases/GetContractPenalties/IGetContractPenaltiesUseCase.cs`
- `Shared.Application/UseCases/GetContractPenalties/GetContractPenaltiesUseCase.cs`
- `Shared.Application/UseCases/GetContractPenalties/GetContractPenaltiesRequest.cs`
- `Shared.Application/UseCases/GetContractPenalties/GetContractPenaltiesResponse.cs`
- `Shared.Domain/Repositories/IContractPenaltyRepository.cs`
- `Shared.Infrastructure/Repositories/ContractPenaltyRepository.cs`

#### 3.4 Responsabilidades
- **Controller:** Receber query parameters, chamar use case, retornar response
- **Use Case:** Filtrar multas por cliente, plano, data, retornar lista
- **Repository:** Buscar multas no banco
- **Não deve:** Controller não deve filtrar dados

#### 3.5 Validações
- Apenas GERENTE pode acessar
- StartDate <= EndDate se fornecidos

#### 3.6 Fluxo de Chamadas
Controller → IGetContractPenaltiesUseCase.ExecuteAsync(GetContractPenaltiesRequest) → IContractPenaltyRepository.GetFilteredAsync → Retorno GetContractPenaltiesResponse

#### 3.7 Retorno
- **Tipo:** GetContractPenaltiesResponse
- **DTO:** `{ Penalties: List<ContractPenaltyDto>, Total: int }`
- **Códigos HTTP:** 200 OK (sucesso), 403 Forbidden (sem permissão), 400 Bad Request (validação)

#### 3.8 Dependências
- Controller injeta: IGetContractPenaltiesUseCase
- Use Case injeta: IContractPenaltyRepository, IMapper

#### 3.9 Regras de Negócio
- Acesso exclusivo a GERENTE (RF01)
- Multas imutáveis (RN08)