namespace PolicyTrackerWebClient.Domain
{
    public interface IAuditable
    {
        int CreatedBy { get; set; }
        int? LastModifiedBy { get; set; }
    }
}
