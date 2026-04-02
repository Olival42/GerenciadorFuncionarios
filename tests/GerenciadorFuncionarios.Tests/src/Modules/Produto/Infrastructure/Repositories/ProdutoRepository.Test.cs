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
    public async Task GetAllAsync_Should_Return_Paginated_Data()
    {
        for (int i = 0; i < 20; i++)
        {
            _context.Produto.Add(new Produto
            {
                Name = $"Produto {i}",
                Price = 100,
                Quantity = 10,
                Type = TipoProduto.GAS,
                IsActive = true
            });
        }

        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 10, null, null, null, null);

        Assert.Equal(10, result.Items.Count());
        Assert.Equal(20, result.TotalItems);
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_Name()
    {
        _context.Produto.AddRange(
            new Produto { Name = "Gas 13kg", Price = 100, Quantity = 10, Type = TipoProduto.GAS, IsActive = true },
            new Produto { Name = "Agua 20L", Price = 20, Quantity = 10, Type = TipoProduto.AGUA, IsActive = true }
        );

        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 10, "Gas", null, null, null);

        Assert.Single(result.Items);
        Assert.Contains("Gas", result.Items.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_Type()
    {
        _context.Produto.AddRange(
            new Produto { Name = "Gas", Price = 100, Quantity = 10, Type = TipoProduto.GAS, IsActive = true },
            new Produto { Name = "Agua", Price = 20, Quantity = 10, Type = TipoProduto.AGUA, IsActive = true }
        );

        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 10, null, null, null, TipoProduto.GAS);

        Assert.Single(result.Items);
        Assert.Equal(TipoProduto.GAS, result.Items.First().Type);
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_Min_Price()
    {
        _context.Produto.AddRange(
            new Produto { Name = "A", Price = 50, Quantity = 10, Type = TipoProduto.GAS, IsActive = true },
            new Produto { Name = "B", Price = 150, Quantity = 10, Type = TipoProduto.GAS, IsActive = true }
        );

        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 10, null, 100, null, null);

        Assert.Single(result.Items);
        Assert.True(result.Items.First().Price >= 100);
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_Max_Price()
    {
        _context.Produto.AddRange(
            new Produto { Name = "A", Price = 50, Quantity = 10, Type = TipoProduto.GAS, IsActive = true },
            new Produto { Name = "B", Price = 150, Quantity = 10, Type = TipoProduto.GAS, IsActive = true }
        );

        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 10, null, null, 100, null);

        Assert.Single(result.Items);
        Assert.True(result.Items.First().Price <= 100);
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_Price_Range()
    {
        _context.Produto.AddRange(
            new Produto { Name = "A", Price = 50, Quantity = 10, Type = TipoProduto.GAS, IsActive = true },
            new Produto { Name = "B", Price = 120, Quantity = 10, Type = TipoProduto.GAS, IsActive = true },
            new Produto { Name = "C", Price = 200, Quantity = 10, Type = TipoProduto.GAS, IsActive = true }
        );

        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 10, null, 100, 150, null);

        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetAllAsync_Should_Apply_Multiple_Filters()
    {
        _context.Produto.AddRange(
            new Produto { Name = "Gas 13kg", Price = 120, Quantity = 10, Type = TipoProduto.GAS, IsActive = true },
            new Produto { Name = "Gas 20kg", Price = 200, Quantity = 10, Type = TipoProduto.GAS, IsActive = true },
            new Produto { Name = "Agua", Price = 120, Quantity = 10, Type = TipoProduto.AGUA, IsActive = true }
        );

        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 10, "Gas", 100, 150, TipoProduto.GAS);

        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetAllAsync_Should_Not_Return_Inactive()
    {
        _context.Produto.AddRange(
            new Produto { Name = "Gas", Price = 120, Quantity = 10, Type = TipoProduto.GAS, IsActive = false }
        );

        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 10, null, null, null, null);

        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetLowStockAsync_Should_Return_Products_Below_Limit()
    {
        var prods = new List<Produto>
    {
        Create_Produto(),
        Create_Produto()
    };

        prods[0].Quantity = 2;

        await _context.AddRangeAsync(prods);
        await _context.SaveChangesAsync();

        var result = await _repository.GetLowStockAsync(5);

        Assert.Single(result);
        Assert.Equal(prods[0].Id, result.First().Id);
    }

    [Fact]
    public async Task GetLowStockAsync_Should_Not_Return_Products_Above_Limit()
    {
        _context.Produto.Add(
            Create_Produto()
        );

        await _context.SaveChangesAsync();


        var result = await _repository.GetLowStockAsync(5);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLowStockAsync_Should_Not_Return_Inactive_Products()
    {
        var prod = Create_Produto(); prod.IsActive = false;

        _context.Produto.Add(
            prod
        );

        await _context.SaveChangesAsync();

        var result = await _repository.GetLowStockAsync(5);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLowStockAsync_Should_Return_Multiple_Products()
    {
        _context.Produto.AddRange(
            new Produto { Name = "A", Type = TipoProduto.GAS, Price = 100, Quantity = 2, IsActive = true },
            new Produto { Name = "B", Type = TipoProduto.GAS, Price = 100, Quantity = 3, IsActive = true }
        );

        await _context.SaveChangesAsync();

        var result = await _repository.GetLowStockAsync(5);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetLowStockAsync_Should_Return_Empty_When_No_Products()
    {
        var result = await _repository.GetLowStockAsync(5);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLowStockAsync_Should_Return_When_Quantity_Equals_Limit()
    {
        _context.Produto.Add(
            new Produto { Name = "A", Type = TipoProduto.GAS, Price = 100, Quantity = 5, IsActive = true }
        );

        await _context.SaveChangesAsync();

        var result = await _repository.GetLowStockAsync(5);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyByNameAsync_Should_Return_True_Ignoring_Case()
    {
        var prod = new Produto
        {
            Name = "Gas 13kg",
            Price = 100,
            Quantity = 10,
            Type = TipoProduto.GAS,
            IsActive = true
        };

        await _context.Produto.AddAsync(prod);
        await _context.SaveChangesAsync();

        var result = await _repository.AnyByNameAsync("gas 13KG");

        Assert.True(result);
    }

    [Fact]
    public async Task AnyByNameAsync_Should_Return_False_When_Name_Different_Ignoring_Case()
    {
        var prod = new Produto
        {
            Name = "Gas 13kg",
            Price = 100,
            Quantity = 10,
            Type = TipoProduto.GAS,
            IsActive = true
        };

        await _context.Produto.AddAsync(prod);
        await _context.SaveChangesAsync();

        var result = await _repository.AnyByNameAsync("Agua");

        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_Name_Ignoring_Case()
    {
        _context.Produto.AddRange(
            new Produto { Name = "Gas 13kg", Price = 100, Quantity = 10, Type = TipoProduto.GAS, IsActive = true },
            new Produto { Name = "Agua 20L", Price = 20, Quantity = 10, Type = TipoProduto.AGUA, IsActive = true }
        );

        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 10, "gas", null, null, null);

        Assert.Single(result.Items);
        Assert.Contains("Gas", result.Items.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_Name_With_UpperCase()
    {
        _context.Produto.Add(
            new Produto { Name = "Gas 13kg", Price = 100, Quantity = 10, Type = TipoProduto.GAS, IsActive = true }
        );

        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(1, 10, "GAS", null, null, null);

        Assert.Single(result.Items);
    }
}