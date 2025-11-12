using MembershipService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MembershipService.Infrastructure.Interfaces
{
    public interface IVtexMembershipClient
    {
        Task<MembershipResponse> GetActiveMembershipInfo(
            string xVtexAPIAppToken,
            string xVtexAPIAppKey,
            string status
        );
    }

}
