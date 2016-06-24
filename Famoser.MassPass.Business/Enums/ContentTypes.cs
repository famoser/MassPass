namespace Famoser.MassPass.Business.Enums
{
    public enum ContentTypes
    {
        /// <summary>
        /// The root model, constructed at runtime from all ContentModels without parents
        /// </summary>
        Root,

        /// <summary>
        /// A folder (only collections are important)
        /// </summary>
        Folder,

        /// <summary>
        /// A Note
        /// </summary>
        Note,

        /// <summary>
        /// An unknown Content Type
        /// </summary>
        Unknown
    }
}
