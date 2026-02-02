namespace TodoAPI.Infrastructures.Repositories;

public static class RepositoriesConfig
{
    public static void AddRepositories(this IServiceCollection services)
    {
        /*
         Tips: 正常使用 EF Core 已經取代 Dapper + Repository 模式，EF Core 本身就是一個 Repository 模式
         但如果還是需要共用的一些查詢或者 SqlBulkCopy 一樣可以寫 Repository 來使用
         但前提你確定有共用，否則寫在該 Endpoint 內部即可
        */
        // services.AddScoped<ITodoRepository, TodoRepository>();
    }
}
