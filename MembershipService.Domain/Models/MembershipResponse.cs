using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MembershipService.Domain.Models
{
    public class MembershipResponse
    {
        public List<MembershipInfo>? MembershipInfos { get; set; } = null;
        public string? Error { get; set; }
    }
}

