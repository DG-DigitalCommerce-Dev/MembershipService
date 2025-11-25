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

        public static string ProcessingCancelRequest = "Processing cancel request for subscription {SubscriptionId}";
        public static string CancelRequestProcessed = "Cancel request processed for subscription {SubscriptionId}";
        public static string PageValueShouldBeGreaterThanZero = "Page value should be greater than 0";
        public static string UnexpectedErrorOccurred = "An unexpected error occurred";
        public static string NoSubscriptionsFound = "No subscriptions found.";
        public static string CancelRequestReceived = "Cancel request received for subscription {SubscriptionId}";
        public static string SubscriptionNotFound = "Subscription not found";
        public static string SubscriptionNotActive = "Subscription not ACTIVE";
        public static string VtexCancellationFailed = "VTEX cancellation failed";
        public static string SubscriptionCancelledSuccessfully = "Subscription cancelled successfully.";
        public static string StatusFailed = "FAILED";
        public static string StatusCancelled = "CANCELLED";
        public static string VtexReturnedErrorForSubscription = "VTEX returned {StatusCode} for subscription {SubscriptionId}";
        public static string ErrorFetchingSubscription = "Error fetching subscription {SubscriptionId}";
        public static string FailedToCancelSubscription = "Failed to cancel subscription {SubscriptionId}. StatusCode: {StatusCode}";
        public static string ErrorCancellingSubscription = "Error cancelling subscription {SubscriptionId}";
        public static string SubscriptionListFetchFailed = "Subscription list fetch failed for email {CustomerEmail}";
        public static string ErrorFetchingTrialEligibility = "Error fetching trial eligibility for {CustomerEmail}";
    }
}
