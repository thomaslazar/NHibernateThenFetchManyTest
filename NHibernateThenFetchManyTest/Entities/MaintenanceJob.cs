namespace NHibernateThenFetchManyTest.Entities
{
    public class MaintenanceJob
    {
        public virtual int Id { get; protected set; }
        public virtual string JobNumber { get; set; }
        public virtual string JobInfo { get; set; }
        public virtual MaintenanceObject MaintenanceObject { get; set; }
    }
}
