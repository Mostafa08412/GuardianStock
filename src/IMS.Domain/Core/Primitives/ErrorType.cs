namespace IMS.Domain.Core.Primitives
{
    public enum ErrorType
    {
        Validation,          // A business rule or invariant was violated (e.g., negative price)
        NotFound,            // A required domain object does not exist
        Conflict,            // State conflict (e.g., trying to register an email that's taken)
        IdentityError,
        AccessDenied,        // The domain user lacks the specific permission for this action
        Failure,             // A general business process failure
        ConditionNotMet      // The system is in a state where this action is not allowed
    }
}