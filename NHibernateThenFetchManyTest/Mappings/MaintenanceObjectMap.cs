using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using NHibernateThenFetchManyTest.Entities;

namespace NHibernateThenFetchManyTest.Mappings
{
    public class MaintenanceObjectMap : ClassMap<MaintenanceObject>
    {
        public MaintenanceObjectMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            HasMany(x => x.Jobs)
                .Inverse()
                .Cascade.All();
            HasMany(x => x.Components)
                .Inverse()
                .Cascade.All();
        }
    }
}
