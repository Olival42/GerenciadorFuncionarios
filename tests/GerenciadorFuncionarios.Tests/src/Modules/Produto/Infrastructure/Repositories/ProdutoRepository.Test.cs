using Xunit;
using Microsoft.EntityFrameworkCore;
using GerenciadorFuncionarios.Infrastructure;
using GerenciadorFuncionarios.Modules.Produto.Infrastructure.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Domain.Models;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;

public class ProdutoRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly ProdutoRepository _repository;

    public ProdutoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ProdutoRepository(_context);
    }

    private Produto Create_Produto()
    {
        return new Produto
        {
            Name = "Gás 15kg",
            Type = TipoProduto.GAS,
            Quantity = 50,
            Price = 110.00,
            IsActive = true
        };
    }

    [Fact]
    public async Task Add_Should_Add_Product_To_Database()
    {
        var prod = Create_Produto();

        await _repository.Add(prod);

        var result = await _context.Produto
            .FirstOrDefaultAsync(p => p.Id == prod.Id);

        Assert.NotNull(result);
        Assert.Equal(prod.Name, result!.Name);
    }

    [Fact]
    public async Task Add_Should_SaveChanges_After_Add()
    {
        var prod = Create_Produto();

        await _repository.Add(prod);

        var exists = await _context.Produto
            .AnyAsync(p => p.Id == prod.Id);

        Assert.True(exists);
    }

    [Fact]
    public async Task GetProdutosAtivosAsync_Should_Return_Only_Active_Products()
    {
        var list = new List<Produto>
        {
            Create_Produto(),
            Create_Produto(),
            Create_Produto()
        };

        list[1].IsActive = false;

        await _context.Produto.AddRangeAsync(list);
        await _context.SaveChangesAsync();

        var result = await _repository.GetProdutosAtivosAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetProdutosAtivosAsync_Should_Return_Empty_When_No_Active_Products()
    {
        var list = new List<Produto>
        {
            Create_Produto(),
            Create_Produto(),
            Create_Produto()
        };

        list.ForEach(p => p.IsActive = false);

        await _context.Produto.AddRangeAsync(list);
        await _context.SaveChangesAsync();

        var result = await _repository.GetProdutosAtivosAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Product_When_Exists_And_Active()
    {
        var prod = Create_Produto();

        await _context.Produto.AddAsync(prod);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(prod.Id);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Product_Not_Found()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Product_Is_Inactive()
    {
        var prod = Create_Produto();
        prod.IsActive = false;

        await _context.Produto.AddAsync(prod);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(prod.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Correct_Product_By_Id()
    {
        var prod = Create_Produto();

        await _context.Produto.AddAsync(prod);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(prod.Id);

        Assert.Equal(prod.Id, result!.Id);
    }

    [Fact]
    public async Task AnyByNameAsync_Should_Return_True_When_Name_Exists()
    {
        var prod = Create_Produto();

        await _context.Produto.AddAsync(prod);
        await _context.SaveChangesAsync();

        var result = await _repository.AnyByNameAsync(prod.Name);

        Assert.True(result);
    }

    [Fact]
    public async Task AnyByNameAsync_Should_Return_False_When_Name_Not_Exists()
    {
        var result = await _repository.AnyByNameAsync("Produto inexistente");

        Assert.False(result);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Persist_Changes()
    {
        var prod = Create_Produto();

        await _context.Produto.AddAsync(prod);
        await _repository.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(prod.Id);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Not_Throw_When_No_Changes()
    {
        await _repository.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_First_Page()
    {
        var prods = new List<Produto>
        {
            Create_Produto(),
            Create_Produto(),
            Create_Produto()
        };

        await _context.Produto.AddRangeAsync(prods);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 2);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalItems);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Only_Active_Products()
    {
        var prods = new List<Produto>
        {
            Create_Produto(),
            Create_Produto(),
            Create_Produto()
        };

        prods[0].IsActive = false;

        await _context.Produto.AddRangeAsync(prods);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 10);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalItems);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Items_Ordered_By_Name()
    {
        var prodA = Create_Produto(); prodA.Name = "B";
        var prodB = Create_Produto(); prodB.Name = "A";
        var prodC = Create_Produto(); prodC.Name = "C";

        await _context.Produto.AddRangeAsync(prodA, prodB, prodC);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 10);

        var names = result.Items.Select(x => x.Name).ToList();

        Assert.Equal(new List<string> { "A", "B", "C" }, names);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_When_Page_Out_Of_Range()
    {
        var prods = new List<Produto>
        {
            Create_Produto(),
            Create_Produto()
        };

        await _context.Produto.AddRangeAsync(prods);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(5, 2);

        Assert.Empty(result.Items);
    }
}