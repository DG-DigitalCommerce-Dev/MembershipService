using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MembershipService.Domain.Constants
{
    public class LogMessageConstants
    {
        public static string callingVtexMembershipApi = "Calling VTEX endpoint for Membership Information";
        public static string errorOnVtexMembershipApi = "Unexpected Error when attempting to get membership information from VTEX endpoint";
        public static string requestingMembershipData = "Attempting to get Membership information from VTEX endpoint";
        public static string membershipInfoReceived = "Membership Information Received from VTEX endpoint";
        public static string processingMembershipInfoEndpoint = "Start processing of api/Membership/skus end point";
    }
}
