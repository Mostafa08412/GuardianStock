using IMS.Domain.Core.Primitives;

namespace IMS.Domain.Inventories
{
    public class LowStockAlert : ValueObject
    {
        public LowStockAlert(int threshold, DateTime? notificationSentAt, DateTime? dismissedAt, DateTime triggeredAtUTC)
        {
            Threshold = threshold;
            NotificationSentAt = notificationSentAt;
            DismissedAt = dismissedAt;
            TriggeredAtUTC = triggeredAtUTC;
        }

        public int Threshold { get; private set; }

        public DateTime? NotificationSentAt { get; private set; }

        public DateTime? DismissedAt { get; private set; }

        public DateTime TriggeredAtUTC { get; private set; }

        public bool IsNotificationSent => NotificationSentAt != null;

        public bool HasPendingAction => !DismissedAt.HasValue && IsNotificationSent;



        public static LowStockAlert Trigger(int threshold) => new(threshold, null, null, DateTime.UtcNow);
        public LowStockAlert MarkAsSent() => new(Threshold, DateTime.UtcNow, null, TriggeredAtUTC);
        public LowStockAlert Dismiss() => new(Threshold, NotificationSentAt, DateTime.UtcNow, TriggeredAtUTC);

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Threshold;
            yield return NotificationSentAt;
            yield return DismissedAt;
            yield return TriggeredAtUTC;

        }
    }
}
