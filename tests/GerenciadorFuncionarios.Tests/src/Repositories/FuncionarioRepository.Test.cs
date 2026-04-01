using Xunit;
using Microsoft.EntityFrameworkCore;
using GerenciadorFuncionarios.Data;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Enums;
using GerenciadorFuncionarios.Repositories;

public class FuncionarioRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly FuncionarioRepository _repository;

    public FuncionarioRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

        _context = new AppDbContext(options);
        _repository = new FuncionarioRepository(_context);
    }

    [Fact]
    public async Task GetByEmail_Should_Return_Funcionario_When_Email_Exists_And_IsActive()
    {
        var func = Create_Funcionario();

        await _context.Funcionario.AddAsync(func);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByEmail(func.Email);

        Assert.NotNull(result);
        Assert.Equal(func.Email, result.Email);
    }

    [Fact]
    public async Task GetByEmail_Should_Return_Null_When_Email_NonExists()
    {
        var result = await _repository.GetByEmail("teste@email.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmail_Should_Return_Funcionario_When_EmailExists_And_IsNotActive()
    {
        var func = Create_Funcionario();
        func.IsActive = false;

        await _context.Funcionario.AddAsync(func);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByEmail(func.Email);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmail_Should_Return_First_When_Duplicates_Exist()
    {
        var email = "teste@email.com";

        var func1 = Create_Funcionario();
        var func2 = Create_Funcionario();

        await _context.Funcionario.AddRangeAsync(func1, func2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByEmail(email);

        Assert.NotNull(result);
        Assert.Equal(func1.Email, result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Funcionario_When_Id_Exists_And_IsActive()
    {
        var func = Create_Funcionario();

        await _context.Funcionario.AddAsync(func);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(func.Id);

        Assert.NotNull(result);
        Assert.Equal(func.Email, result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Id_NonExists()
    {
        var id = Guid.NewGuid();

        var result = await _repository.GetByIdAsync(id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Id_Exists_And_IsNotActive()
    {
        var func = Create_Funcionario();
        func.IsActive = false;

        await _context.Funcionario.AddAsync(func);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(func.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task Add_Should_Persist_Funcionario()
    {
        var funcionario = Create_Funcionario();

        await _repository.Add(funcionario);
        await _repository.SaveChangesAsync();

        var result = await _context.Funcionario
            .FirstOrDefaultAsync(f => f.Id == funcionario.Id);

        Assert.NotNull(result);
        Assert.Equal(funcionario.Email, result.Email);
    }

    [Fact]
    public async Task AnyByCPFAsync_Should_Return_True_When_CPF_Exists()
    {
        var func = Create_Funcionario();

        await _context.Funcionario.AddAsync(func);
        await _context.SaveChangesAsync();

        var result = await _repository.AnyByCPFAsync(func.CPF);

        Assert.True(result);
    }

    [Fact]
    public async Task AnyByCPFAsync_Should_Return_False_When_CPF_NonExists()
    {
        var cpf = "68714247097";

        var result = await _repository.AnyByCPFAsync(cpf);

        Assert.False(result);
    }

    [Fact]
    public async Task AnyByEmailAsync_Should_Return_True_When_Email_Exists()
    {
        var func = Create_Funcionario();

        await _context.Funcionario.AddAsync(func);
        await _context.SaveChangesAsync();

        var result = await _repository.AnyByEmailAsync(func.Email);

        Assert.True(result);
    }

    [Fact]
    public async Task AnyByEmailAsync_Should_Return_False_When_Email_NonExists()
    {
        var email = "teste@email.com";

        var result = await _repository.AnyByEmailAsync(email);

        Assert.False(result);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Persist_Data()
    {
        var funcionario = Create_Funcionario();

        await _repository.Add(funcionario);
        await _repository.SaveChangesAsync();

        var result = await _context.Funcionario.FirstOrDefaultAsync(f => f.Id == funcionario.Id);

        Assert.NotNull(result);
        Assert.Equal(funcionario.Email, result.Email);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Paginated_Funcionarios()
    {
        var funcs = new List<Funcionario>
        {
            Create_Funcionario(),
            Create_Funcionario(),
            Create_Funcionario()
        };

        await _context.Funcionario.AddRangeAsync(funcs);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(page: 1, pageSize: 2);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Paginated_Funcionarios_AreActives()
    {
        var funcs = new List<Funcionario>
        {
            Create_Funcionario(),
            Create_Funcionario(),
            Create_Funcionario()
        };

        funcs[0].IsActive = false;
        await _context.Funcionario.AddRangeAsync(funcs);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(page: 1, pageSize: 2);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Funcionarios_Ordered_By_Name()
    {
        var funcA = Create_Funcionario();
        funcA.Name = "Zé";

        var funcB = Create_Funcionario();
        funcB.Name = "Ana";

        var funcC = Create_Funcionario();
        funcC.Name = "Carlos";

        await _context.Funcionario.AddRangeAsync(funcA, funcB, funcC);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(page: 1, pageSize: 10);

        var nomesEsperados = new List<string> { "Ana", "Carlos", "Zé" };
        var nomesRetornados = result.Items.Select(f => f.Name).ToList();

        Assert.Equal(nomesEsperados, nomesRetornados);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_SecondPage_Correctly()
    {
        var funcs = new List<Funcionario>
        {
            Create_Funcionario(),
            Create_Funcionario(),
            Create_Funcionario(),
            Create_Funcionario()
        };

        await _context.Funcionario.AddRangeAsync(funcs);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(page: 2, pageSize: 2);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(4, result.TotalItems);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Null_When_Page_NonExists()
    {
        var funcs = new List<Funcionario>
        {
            Create_Funcionario(),
            Create_Funcionario(),
            Create_Funcionario(),
            Create_Funcionario()
        };

        await _context.Funcionario.AddRangeAsync(funcs);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(page: 3, pageSize: 2);

        Assert.Empty(result.Items);
        Assert.Equal(4, result.TotalItems);
        Assert.Equal(3, result.Page);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task GetAllAsync_Should_Skip_Correctly_For_Pagination()
    {
        var func1 = Create_Funcionario(); func1.Name = "A";
        var func2 = Create_Funcionario(); func2.Name = "B";
        var func3 = Create_Funcionario(); func3.Name = "C";
        var func4 = Create_Funcionario(); func4.Name = "D";
        var func5 = Create_Funcionario(); func5.Name = "E";

        await _context.Funcionario.AddRangeAsync(func1, func2, func3, func4, func5);
        await _context.SaveChangesAsync();

        int pageSize = 2;
        int page = 2;

        var result = await _repository.GetAllAsync(page, pageSize);

        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, f => f.Name == "C");
        Assert.Contains(result.Items, f => f.Name == "D");
        Assert.DoesNotContain(result.Items, f => f.Name == "A");
        Assert.DoesNotContain(result.Items, f => f.Name == "B");
        Assert.DoesNotContain(result.Items, f => f.Name == "E");

        Assert.Equal(5, result.TotalItems);
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
    }

    [Fact]
    public async Task GetAllAsync_Should_Ignore_Inactive_Funcionarios()
    {
        var func1 = Create_Funcionario(); func1.Name = "A"; func1.IsActive = false;
        var func2 = Create_Funcionario(); func2.Name = "B"; func2.IsActive = false;
        var func3 = Create_Funcionario(); func3.Name = "C";
        var func4 = Create_Funcionario(); func4.Name = "D";

        await _context.Funcionario.AddRangeAsync(func1, func2, func3, func4);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 2);

        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, f => f.Name == "C");
        Assert.Contains(result.Items, f => f.Name == "D");
        Assert.DoesNotContain(result.Items, f => f.Name == "A");
        Assert.DoesNotContain(result.Items, f => f.Name == "B");

        Assert.Equal(2, result.TotalItems);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task GetAllAsync_Should_Map_To_ResponseFuncionarioDTO_Correctly()
    {
        var func1 = Create_Funcionario(); func1.Name = "Alice";
        var func2 = Create_Funcionario(); func2.Name = "Bob";

        await _context.Funcionario.AddRangeAsync(func1, func2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(page: 1, pageSize: 10);

        Assert.Equal(2, result.Items.Count);

        var dto1 = result.Items.First(f => f.Id == func1.Id);
        var dto2 = result.Items.First(f => f.Id == func2.Id);

        Assert.Equal(func1.Id, dto1.Id);
        Assert.Equal(func1.Name, dto1.Name);
        Assert.Equal(func1.Email, dto1.Email);
        Assert.Equal(func1.CPF, dto1.CPF);
        Assert.Equal(func1.IsActive, dto1.IsActive);
        Assert.Equal(func1.Role, dto1.Role);
        Assert.Equal(func1.Phone, dto1.Phone);

        Assert.Equal(func2.Id, dto2.Id);
        Assert.Equal(func2.Name, dto2.Name);
        Assert.Equal(func2.Email, dto2.Email);
        Assert.Equal(func2.CPF, dto2.CPF);
        Assert.Equal(func2.IsActive, dto2.IsActive);
        Assert.Equal(func2.Role, dto2.Role);
        Assert.Equal(func2.Phone, dto2.Phone);
    }

    private Funcionario Create_Funcionario()
    {
        return new Funcionario
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Phone = "44999999999",
            CPF = "68714247097",
            Email = "teste@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = Role.ADMIN,
            IsActive = true
        };
    }
}