namespace ShopLego.UnitTests;

public sealed class ArchitectureTests
{
    [Fact]
    public void Domain_has_no_project_dependencies()
    {
        var references = typeof(Domain.Entities.User).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToArray();
        Assert.DoesNotContain("ShopLego.Application", references);
        Assert.DoesNotContain("ShopLego.Infrastructure", references);
        Assert.DoesNotContain("ShopLego.Api", references);
    }

    [Fact]
    public void Application_does_not_depend_on_outer_layers()
    {
        var references = typeof(Application.IProductService).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToArray();
        Assert.DoesNotContain("ShopLego.Infrastructure", references);
        Assert.DoesNotContain("ShopLego.Api", references);
    }
}
