namespace RealEstate.Domain.Enums
{
    public enum PropertyStatus
    {
        /// <summary>
        /// The property is available for sale or rent.
        /// </summary>
        AVAILABLE,

        /// <summary>
        /// The property is under contract or pending sale/rent.
        /// </summary>
        PENDING,

        /// <summary>
        /// The property has been sold or rented.
        /// </summary>
        SOLD,
    }
}