namespace EFKit.Contexts
{
    /// <summary>
    /// Indicates how an entity should be attached to a DbContext.
    /// </summary>
    public enum AttachAs
    {
        /// <summary>
        /// Attach the entity as Inserted.
        /// </summary>
        Inserted,

        /// <summary>
        /// Attach the entity as Modified.
        /// </summary>
        Modified
    }
}