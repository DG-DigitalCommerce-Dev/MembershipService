using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MembershipService.Domain.Models;

namespace MembershipService.Infrastructure.Interfaces
{
    public interface IVtexMembershipRepository
    {
        Task<VtexMembershipResponse> GetActiveMembershipData(int page);
    }
}
