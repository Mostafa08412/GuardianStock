using IMS.Domain.Abstractions;
using IMS.Domain.Core.Primitives;
using IMS.Domain.Core.Primitives.Result;

namespace IMS.Domain.Inventories
{
    public class Inventory : Aggregate, IAuditable
    {
        public static readonly int MinimumLowStockThreshold = 10;
        protected Inventory() : base()
        {

        }

        private Inventory(Guid id, int quantity, int lowStockThreshold, Guid productId) : base(id)
        {
            Quantity = quantity;
            LowStockThreshold = lowStockThreshold;
            ProductId = productId;

        }

        public int Quantity { get; private set; }

        public int LowStockThreshold { get; private set; }

        public Guid ProductId { get; private set; }

        public LowStockAlert? LowStockAlert { get; private set; }

        public InventoryStatus Status
        {

            get
            {
                decimal quantity = new Decimal(this.Quantity);

                decimal lowStockThreshold = new Decimal(this.LowStockThreshold);

                if (lowStockThreshold == 0) return InventoryStatus.Critical;

                decimal ratio = quantity / lowStockThreshold;

                if (ratio <= 0.3m) return InventoryStatus.Critical;

                else if (ratio > 0.3m && ratio <= 1.0m) return InventoryStatus.Low;

                else return InventoryStatus.Healthy;

            }

        }

        public DateTime CreatedOnUTC { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTime UpdatedOnUTC { get; private set; }

        public string UpdatedBy { get; private set; }


        public static Result<Inventory> Create(int quantity, int lowStockThreshold, Guid productId)
        {
            var newId = Guid.CreateVersion7();


            if (quantity < 1)
            {
                return Result<Inventory>.Failure(Errors.InventoryErrors.InvalidQuantity);
            }

            if (quantity < lowStockThreshold)
            {
                return Result<Inventory>.Failure(Errors.InventoryErrors.QuantityCannotBeLowerThanThreshold);

            }

            if (lowStockThreshold < MinimumLowStockThreshold)
            {
                return Result<Inventory>.Failure(Errors.InventoryErrors.InvalidLowStockThreshold);
            }

            if (productId == Guid.Empty)
            {
                return Result<Inventory>.Failure(Errors.InventoryErrors.ProductIsRequired);
            }

            var newInventory = new Inventory(newId, quantity, lowStockThreshold, productId);

            return Result<Inventory>.Success(newInventory);
        }

        public Result Restock(int addedQuantity)
        {
            if (addedQuantity <= 0)
            {
                return Result.Failure(Errors.InventoryErrors.InvalidRestockQuantity);
            }

            Quantity += addedQuantity;

            // Action Taken: Inventory is Restocked, then the alert is gone.
            if (LowStockAlert != null && Quantity > LowStockAlert.Threshold)
                ResetLowStockAlert();



            return Result.Success();

        }

        public Result Ship(int shippedQuantity)
        {
            if (shippedQuantity <= 0)
            {
                return Result.Failure(Errors.InventoryErrors.InvalidShipQuantity);
            }

            if (shippedQuantity > Quantity)
            {
                return Result.Failure(Errors.InventoryErrors.InsufficientStock);
            }

            Quantity -= shippedQuantity;

            if (Quantity <= LowStockThreshold)
            {

                if (LowStockAlert == null)
                {
                    TriggerLowStockAlert(LowStockThreshold);
                    //Trigger a LowStockAlertTriggered Domain Event
                    AddDomainEvent(new LowStockAlertTriggeredDomainEvent(ProductId, Id, LowStockAlert.TriggeredAtUTC, LowStockThreshold, Quantity));
                }
                // Case 2: Means the alert is triggered but still not being sent.. (awaiting notification send confirmation)
                else if (!LowStockAlert.IsNotificationSent)
                {

                }

                // Case 3: Means the alert is already triggered once before and sent, but it hasn't been solved yet.
                // we ignore this case because according to BR, we should send the alert 1 time only,
                // till a new stock above the threshold is done..
            }



            return Result.Success();

        }

        public Result AdjustLowStockThreshold(int newLowStockThreshold)
        {
            if (newLowStockThreshold < MinimumLowStockThreshold)
            {
                return Result.Failure(Errors.InventoryErrors.InvalidLowStockThreshold);
            }

            if (LowStockAlert != null && LowStockAlert.HasPendingAction)
                return Result.Failure(Errors.InventoryErrors.CannotAdjustThresholdWithActiveAlert);

            LowStockThreshold = newLowStockThreshold;


            ResetLowStockAlert();

            if (Quantity <= LowStockThreshold)
            {
                TriggerLowStockAlert(newLowStockThreshold);
                AddDomainEvent(new LowStockAlertTriggeredDomainEvent(ProductId, Id, LowStockAlert.TriggeredAtUTC, LowStockThreshold, Quantity));

            }

            return Result.Success();
        }
        public void ConfirmNotificationSent()
        {
            if (LowStockAlert != null && !LowStockAlert.IsNotificationSent)
            {
                LowStockAlert = LowStockAlert.MarkAsSent();
            }
        }

        public void ResetLowStockAlert()
        {
            this.LowStockAlert = null;
        }
        public void DismissLowStockAlert()
        {
            if (LowStockAlert != null && !LowStockAlert.DismissedAt.HasValue)
                this.LowStockAlert = LowStockAlert.Dismiss();
        }

        public void TriggerLowStockAlert(int LowStockThreshold)
        {
            if (LowStockAlert == null)
                this.LowStockAlert = LowStockAlert.Trigger(LowStockThreshold);
        }

    }
}
