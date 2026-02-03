namespace TodoAPI.Infrastructures.Adapters.YouBike;

public interface IYouBikeAdapter
{
    /// <summary>
    ///  查詢 YouBike 站點即時資料
    /// </summary>
    /// <returns>YouBike 站點即時資料</returns>
    Task<IEnumerable<YouBikeImmediateDto>> GetYouBikeImmediateAsync();
}
