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
        public static string CallingVtexMembershipApi = "Calling VTEX endpoint for Membership Information";
        public static string ErrorOnVtexMembershipApi = "Unexpected Error when attempting to get membership information from VTEX endpoint";
        public static string RequestingMembershipData = "Attempting to get Membership information from VTEX endpoint";
        public static string MembershipInfoReceived = "Membership Information Received from VTEX endpoint";
        public static string ProcessingMembershipInfoEndpoint = "Start processing of api/Membership/skus end point";
    }
}
