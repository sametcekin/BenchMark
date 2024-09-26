using Riok.Mapperly.Abstractions;

namespace BenchMark.Common;

[Mapper]
public partial class ProductMapper
{
    [MapProperty(nameof(Product.Description1), nameof(ProductResult.Description2))]
    [MapProperty(nameof(Product.Id), nameof(ProductResult.Id))]
    public partial List<ProductResult> MapList(List<Product> product);
}
