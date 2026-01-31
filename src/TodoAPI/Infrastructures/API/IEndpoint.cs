namespace TodoAPI.Infrastructures.API;

public interface IEndpoint
{
    /// <summary>
    /// C# 11 的 static abstract interface member，讓介面可以約定「靜態方法」的簽名。
    /// </summary>
    /// <param name="app">IEndpointRouteBuilder</param>
    public static abstract void MapEndpoint(IEndpointRouteBuilder app);
}
