using MembershipService.Domain.Constants;

namespace MembershipService.Application.Common.Models
{
    public class CancelResponseModel
    {
        public bool Ok { get; set; }
        public required string Status { get; set; }

        public int RemainingTrialDays { get; set; }
        public required string Message { get; set; }
        public int HttpCode { get; set; }

        public static CancelResponseModel Success(int days) =>
            new()
            {
                Ok = true,
                Status = LogMessageConstants.StatusCancelled,
                RemainingTrialDays = days,
                Message = LogMessageConstants.SubscriptionCancelledSuccessfully,
                HttpCode = 200
            };

        public static CancelResponseModel Error(string message, int code) =>
            new()
            {
                Ok = false,
                Status = LogMessageConstants.StatusFailed,
                Message = message,
                HttpCode = code
            };
    }
}
