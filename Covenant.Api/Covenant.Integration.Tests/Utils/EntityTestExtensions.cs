namespace Covenant.Integration.Tests.Utils;

public static class EntityTestExtensions
{
    public static T With<T>(this T entity, Action<T> configure)
    {
        configure(entity);
        return entity;
    }
}
