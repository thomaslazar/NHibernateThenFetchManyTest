using System.Collections.Generic;

namespace NHibernateThenFetchManyTest.Entities
{
    public class Component
    {
        public virtual int Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual double Price { get; set; }
        public virtual MaintenanceObject MaintenanceObject { get; set; }
    }
}
