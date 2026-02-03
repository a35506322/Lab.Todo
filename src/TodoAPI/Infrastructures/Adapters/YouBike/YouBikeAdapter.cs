namespace TodoAPI.Infrastructures.Adapters.YouBike;

public class YouBikeAdapter : IYouBikeAdapter
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public YouBikeAdapter(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        _httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("YouBike:BaseUrl"));
    }

    public async Task<IEnumerable<YouBikeImmediateDto>> GetYouBikeImmediateAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<YouBikeImmediateDto>>(
            "dotapp/youbike/v2/youbike_immediate.json"
        );
        return response ?? Enumerable.Empty<YouBikeImmediateDto>();
    }
}
