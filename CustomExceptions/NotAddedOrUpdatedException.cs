namespace E_CommerceApp.CustomExceptions
{
    public class NotAddedOrUpdatedException : Exception
    {
        public NotAddedOrUpdatedException()
        : base($"Failed to create or update your entities.") { }
    }
}
