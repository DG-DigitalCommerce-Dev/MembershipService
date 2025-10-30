//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MembershipService.Application.Common.Interfaces
//{
//    internal interface ISubscriptionService
//    {
//    }
//}
using MembershipService.Domain.Models;

namespace MembershipService.Application.Common.Interfaces
{
    public interface ISubscriptionService
    {
        Task<SubscriptionResponse> GetSubscriptionPlansAsync();
    }
}

