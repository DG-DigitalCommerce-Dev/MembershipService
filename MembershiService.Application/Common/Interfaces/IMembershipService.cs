using MembershipService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MembershipService.Application.DTOs;
namespace MembershipService.Application.Common.Interfaces
{
    public interface IMembershipInfoService
    {
        Task<IEnumerable<MembershipDto>> GetActiveMembershipInfo();
    }
}
