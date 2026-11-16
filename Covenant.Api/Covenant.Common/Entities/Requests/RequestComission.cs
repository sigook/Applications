namespace Covenant.Common.Entities.Requests;

public class RequestComission
{
    internal RequestComission()
    {
    }

    public RequestComission(Guid requestId, Guid salesRepresentativeId)
    {
        RequestId = requestId;
        SalesRepresentativeId = salesRepresentativeId;
    }

    public Guid RequestId { get; internal set; }
    public Guid SalesRepresentativeId { get; internal set; }
    public virtual Request Request { get; private set; }
    public virtual User SalesRepresentative { get; private set; }

    public void ChangeSalesRepresentative(Guid salesRepresentativeId) => SalesRepresentativeId = salesRepresentativeId;
}
