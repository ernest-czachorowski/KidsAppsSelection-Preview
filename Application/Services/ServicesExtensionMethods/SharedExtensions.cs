namespace Application.Services.ServicesExtensionMethods;

public static class SharedExtensions
{
    public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, bool condition)
    {
        if (condition)
            return source.Where(predicate);
        else
            return source;
    }
}
