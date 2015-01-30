using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using NHibernateThenFetchManyTest.Entities;

namespace NHibernateThenFetchManyTest.Mappings
{
    public class MaintenanceJobMap : ClassMap<MaintenanceJob>
    {
        public MaintenanceJobMap()
        {
            Id(x => x.Id);
            Map(x => x.JobNumber);
            Map(x => x.JobInfo);
            References(x => x.MaintenanceObject);
        }
    }
}
