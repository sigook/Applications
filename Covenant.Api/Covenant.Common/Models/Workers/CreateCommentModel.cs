namespace Covenant.Common.Models.Workers;

public class CreateCommentModel
{
    public string Comment { get; set; }
    public int Rate
    {
        get;
        set => field = value < 0 ? 0 : value;
    }
}