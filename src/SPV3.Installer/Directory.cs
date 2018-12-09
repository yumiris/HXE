namespace SPV3.Installer
{
    /// <summary>
    ///     Represents an SPV3-related directory on the filesystem.
    /// </summary>
    public class Directory
    {
        /// <summary>
        ///     Name of the directory on the filesystem.
        /// </summary>
        /// <example>
        ///     maps
        /// </example>
        public Name Name { get; set; }

        /// <summary>
        ///     Checks if the directory exists on the filesystem using the Path value.
        /// </summary>
        /// <returns>
        ///     True if directory exists, otherwise false.
        /// </returns>
        public bool Exists()
        {
            return System.IO.Directory.Exists(Name.Value);
        }

        /// <summary>
        ///     Creates the directory at the given path value if it does not exist on the filesystem.
        /// </summary>
        public void Create()
        {
            if (!Exists())
                System.IO.Directory.CreateDirectory(Name.Value);
        }
    }
}