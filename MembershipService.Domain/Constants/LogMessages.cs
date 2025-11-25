namespace MembershipService.Domain.Constants
{
    public static class LogMessages
    {
        public const string RequestReceived = "Request received: fetching subscription plans.";
        public const string SendingResponse = "Sending subscription plans response.";

        public const string FetchingFromVtex = "Fetching subscription plans from VTEX...";
        public const string TransformingToDto = "Transforming VTEX subscription data into DTO.";
        public const string NoSubscriptionsFound = "No subscription plans found.";

        public const string FetchProduct = "Fetching VTEX product for RefId: {RefId}";
        public const string FetchPrice = "Fetching VTEX price for SKU: {SkuId}";

        public const string VtexFetchError = "Error occurred while fetching subscription plans from VTEX.";
        public const string ProductFetchError = "Failed to fetch product for RefId: {RefId}";
        public const string PriceFetchError = "Failed to fetch price for SKU: {SkuId}";
        public const string SkuBuildError = "Failed to build SKU: {SkuId}";

        public const string SubscriptionsRetrievedSuccessfully = "Subscriptions retrieved successfully";
        public const string SubscriptionCancelledSuccessfully = "Subscription {SubscriptionId} cancelled successfully";
    }
}
