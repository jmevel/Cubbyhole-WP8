using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Cubbyhole.Models;
using Newtonsoft.Json;

namespace Cubbyhole.API.Mock
{
    #pragma warning disable
    public class PlanMock
    {
        public static async Task<List<Plan>> Get4FakePlans()
        {
            List<Plan> plans = new List<Plan>();

            Plan plan = new Plan();
            plan.Id = 1;
            plan.Name = "Plan1";
            plan.Duration = 10;
            plan.MaxBandwidth = 1;
            plan.MaxQuotaSHared = 2;
            plan.MaxStorage = 3;
            plan.Price = 0;
            plans.Add(plan);

            Plan plan2 = new Plan();
            plan2.Id = 2;
            plan2.Name = "Plan2";
            plan2.Duration = 30;
            plan2.MaxBandwidth = 2;
            plan2.MaxQuotaSHared = 3;
            plan2.MaxStorage = 4;
            plan2.Price = (float)9.99;
            plans.Add(plan2);

            Plan plan3 = new Plan();
            plan3.Id = 3;
            plan3.Name = "Plan3";
            plan3.Duration = 60;
            plan3.MaxBandwidth = 3;
            plan3.MaxQuotaSHared = 4;
            plan3.MaxStorage = 5;
            plan3.Price = (float)14.99;
            plans.Add(plan3);

            Plan plan4 = new Plan();
            plan4.Id = 4;
            plan4.Name = "Plan4";
            plan4.Duration = 90;
            plan4.MaxBandwidth = 4;
            plan4.MaxQuotaSHared = 5;
            plan4.MaxStorage = 6;
            plan4.Price = (float)19.99;
            plans.Add(plan4);

            return plans;
        }
    }
}
