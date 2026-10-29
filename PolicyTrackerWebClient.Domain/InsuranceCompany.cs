namespace PolicyTrackerWebClient.Domain
{
    public class InsuranceCompany : IAuditable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }
        public string Status { get; set; }
    }
}
