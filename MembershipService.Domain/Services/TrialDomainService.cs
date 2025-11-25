using MembershipService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MembershipService.Domain.Services
{
    public static class TrialDomainService
    {

        public static int CalculateRemainingDays(Subscription sub)
        {
            return (sub.NextPurchaseDate - DateTime.UtcNow).Days;
        }
    }
}
