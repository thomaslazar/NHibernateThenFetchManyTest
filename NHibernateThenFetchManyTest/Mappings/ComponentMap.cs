using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using NHibernateThenFetchManyTest.Entities;

namespace NHibernateThenFetchManyTest.Mappings
{
    public class ComponentMap : ClassMap<Component>
    {
        public ComponentMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Price);
            References(x => x.MaintenanceObject);
        }
    }
}
