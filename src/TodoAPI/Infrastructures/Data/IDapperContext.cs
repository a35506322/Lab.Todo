namespace TodoAPI.Infrastructures.Data;

public interface IDapperContext
{
    public SqlConnection CreateTodoConnection();
}
