using System.Linq;
using Atarashii.CLI.Common;

namespace Atarashii.CLI
{
    public class Args
    {
        /// <summary>
        ///     Removes the command (first argument) from an arguments array.
        /// </summary>
        /// <param name="command">
        ///     Arguments array to remove the command from.
        /// </param>
        /// <returns>
        ///     Arguments array without the command.
        /// </returns>
        public static string[] FromCommand(string[] command)
        {
            return command.Skip(1).ToArray();
        }

        /// <summary>
        ///     Exits the program if the inbound arguments are empty.
        /// </summary>
        /// <param name="args">
        ///     Arguments to check the length of.
        /// </param>
        public static void ExitWhenNone(string[] args)
        {
            if (args.Length - 1 <= 0) Exit.WithError("No arguments provided.", 1);
        }
    }
}