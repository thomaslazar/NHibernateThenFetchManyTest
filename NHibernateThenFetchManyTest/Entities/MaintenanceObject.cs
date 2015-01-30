using System.Collections.Generic;

namespace NHibernateThenFetchManyTest.Entities
{
    public class MaintenanceObject
    {
        public virtual int Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual IList<Component> Components { get; set; }
        public virtual IList<MaintenanceJob> Jobs { get; set; }

        public MaintenanceObject()
        {
            Components = new List<Component>();
            Jobs = new List<MaintenanceJob>();
        }

        public virtual void AddProduct(Component component)
        {
            component.MaintenanceObject = this;
            Components.Add(component);
        }

        public virtual void AddEmployee(MaintenanceJob maintenanceJob)
        {
            maintenanceJob.MaintenanceObject = this;
            Jobs.Add(maintenanceJob);
        }
    }
}
