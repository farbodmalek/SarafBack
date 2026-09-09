namespace GirlyShopBackend.Core.DomainServices;

// منطق تولید Slug — چون هم برای Product هم Category لازمه،
// یه‌جا (Core/DomainServices) نگه‌داری می‌شه تا تکراری نشه
public static class SlugGenerator
{
    public static string Generate(string title)
    {
        return title.Trim().Replace(" ", "-");
    }
}
