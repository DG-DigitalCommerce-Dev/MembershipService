namespace MembershipService.Domain.Constants
{
    public static class LogMessages
    {
        // Controller Logs
        public const string RequestReceived = "Request received: fetching subscription plans.";
        public const string SendingResponse = "Sending subscription plans response.";

        // Service Logs
        public const string FetchingFromVtex = "Fetching subscription plans from VTEX...";
        public const string TransformingToDto = "Transforming VTEX subscription data into DTO.";
        public const string NoSubscriptionsFound = "No subscription plans found.";


        // Infrastructure Logs
        public const string FetchProduct = "Fetching VTEX product for RefId: {RefId}";
        public const string FetchPrice = "Fetching VTEX price for SKU: {SkuId}";
    }
}
