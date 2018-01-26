﻿namespace Eaxir.Library.Interfaces
{
    /// <summary>
    /// Interface for exposing methods that handle installation of packages.
    /// </summary>
    public interface IInstall
    {
        /// <summary>
        /// Install the package to the default location.
        /// </summary>
        void Install();

        /// <summary>
        /// Install the package to a custom location.
        /// </summary>
        /// <param name="path"></param>
        void Install(string path);
    }
}
