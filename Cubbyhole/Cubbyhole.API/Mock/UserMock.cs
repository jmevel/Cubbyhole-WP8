using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubbyhole.Models;

namespace Cubbyhole.API.Mock
{
    #pragma warning disable
    public class UserMock
    {
        public static async Task<User> GetUser()
        {
            User user = new User();
            user.Id = 56;
            user.Email = "r@r.fr";

            //MD5 --> true password: "r"
            user.Password = "4b43b0aee35624cd95b910189b3dc231";
            user.CreationDate = Convert.ToDateTime("2014-02-02T18:31:35.610Z");
            user.RegistrationPaidPlanDate = Convert.ToDateTime("2014-02-02T18:31:45.912Z");
            user.Country = "China";
            user.UsedBandwidth = 0;
            user.UsedQuotaShared = 0;
            user.UsedSpace = 0;
            user.PlanId = 1;
            user.Token = "6a3d23d4da4c6069739adc141af91fcd";
            user.CreationDate = Convert.ToDateTime("2014-02-02T18:31:35.614Z");
            user.UpdateDate = Convert.ToDateTime("2014-02-02T18:31:35.614Z");
            return user;
        }
    }
}
