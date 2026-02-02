namespace TodoAPI.Infrastructures.Data;

public class DapperContext(IConfiguration configuration) : IDapperContext
{
    public SqlConnection CreateTodoConnection() =>
        new SqlConnection(configuration.GetConnectionString("Todo"));
}
