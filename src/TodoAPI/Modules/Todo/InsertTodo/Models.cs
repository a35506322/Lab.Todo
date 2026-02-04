namespace TodoAPI.Modules.Todo.InsertTodo;

public class InsertTodoRequest
{
    /// <summary>
    /// 待辦標題
    /// </summary>
    [Required]
    [Display(Name = "待辦標題")]
    [MaxLength(100)]
    public string TodoTitle { get; set; } = string.Empty;

    /// <summary>
    /// 待辦內容
    /// </summary>
    [Display(Name = "待辦內容")]
    [MaxLength(500)]
    public string? TodoContent { get; set; }
}

public class InsertTodoResponse
{
    /// <summary>
    /// 待辦 ID
    /// </summary>
    public long TodoId { get; set; }

    /// <summary>
    /// 待辦標題
    /// </summary>
    public string TodoTitle { get; set; } = string.Empty;

    /// <summary>
    /// 待辦內容
    /// </summary>
    public string? TodoContent { get; set; }

    /// <summary>
    /// 是否完成 (Y/N)
    /// </summary>
    public string IsComplete { get; set; } = string.Empty;

    /// <summary>
    /// 新增時間
    /// </summary>
    public DateTime? AddTime { get; set; }

    /// <summary>
    /// 新增者
    /// </summary>
    public string AddUserId { get; set; } = string.Empty;
}
