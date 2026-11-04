namespace Covenant.Common.Models;

public class Pagination
{
    public static readonly Pagination Default = new Pagination();

    public virtual bool IsDescending { get; set; }

    public int PageIndex
    {
        get => field <= 0 ? 1 : field;
        set;
    } = 1;

    public int PageSize
    {
        get
        {
            if (field > 100) return 50;
            return field <= 0 ? 50 : field;
        }
        set;
    } = 50;
}
