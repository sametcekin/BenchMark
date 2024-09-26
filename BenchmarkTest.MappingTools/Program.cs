using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Bogus;
using Bogus.Extensions;
using Riok.Mapperly.Abstractions;

#region Product Context

public static class ProductContext
{
    public static List<Product> GenerateProducts()
    {
        var fakerProducts = new Faker<Product>()
            .RuleFor(x => x.Id, y => y.IndexFaker)
            .RuleFor(x => x.Sku, y => y.Random.String2(5, 25).ClampLength(1, 30))
            .RuleFor(x => x.Name, y => y.Random.String2(5, 25))
            .RuleFor(x => x.Description, y => y.Random.String2(5, 25))
            .RuleFor(x => x.Description1, y => y.Random.String2(5, 25))
            .RuleFor(x => x.FNSKU, y => y.Random.String2(5, 25))
            .RuleFor(x => x.CurrencyId, y => y.Random.Byte(1, 20));
        return fakerProducts.Generate(500);
    }
}

#endregion

#region Domain Models and DTOs

public class Product
{
    public long Id { get; set; }
    public string Sku { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Description1 { get; set; }
    public string FNSKU { get; set; }
    public byte? CurrencyId { get; set; }
}

public class ProductResult
{
    public long Id { get; set; }
    public string Sku { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Description2 { get; set; }
    public string FNSKU { get; set; }
    public byte? CurrencyId { get; set; }
}

#endregion

#region Mapperly Configuration

[Mapper]
public partial class ProductMapper
{
    [MapProperty(nameof(Product.Description1), nameof(ProductResult.Description2))]
    public partial List<ProductResult> MapList(List<Product> product);
}

#endregion

#region Benchmark Class

[MemoryDiagnoser]
public class MappingBenchmark
{
    private readonly List<Product> _products;
    private readonly IMapper _autoMapper;
    public MappingBenchmark()
    {
        _products = ProductContext.GenerateProducts();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Product, ProductResult>();
        });
        _autoMapper = config.CreateMapper();
    }

    [Benchmark]
    public List<ProductResult> MapWithMapperly() => new ProductMapper().MapList(_products);

    [Benchmark]
    public List<ProductResult> MapWithAutoMapper() => _autoMapper.Map<List<ProductResult>>(_products);
}

#endregion

class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<MappingBenchmark>();
    }
}