using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Utilities
{
    public static class FileSystemExtensions
    {
        public static Task GetFileSystemAsync(string sampleDirectory)
        {
            var root = FindWorkableRootDirectory(sampleDirectory);

            return Task.CompletedTask;
        }

        private static string FindWorkableRootDirectory(string sampleDirectory)
        {
            var sampleDirectoryComponents = sampleDirectory.Split(Path.DirectorySeparatorChar);

            if (string.IsNullOrWhiteSpace(sampleDirectory) || !sampleDirectoryComponents.Any())
            {
                throw new ArgumentException("Invalid sample directory input.", nameof(sampleDirectory));
            }

            string currentDirectory = sampleDirectoryComponents.First();
            
            if(string.IsNullOrWhiteSpace(currentDirectory))
            {
                currentDirectory = new string(new[] { Path.DirectorySeparatorChar });
            }

            int iteration = 0;

            while (!IsFolderAccessible(currentDirectory))
            {
                currentDirectory = Path.Combine(currentDirectory, sampleDirectoryComponents[++iteration]);
            }

            return currentDirectory;
        }

        private static bool IsFolderAccessible(string directory)
        {
            // Return false if the directory is not valid.
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            {
                return false; 
            }

            try
            {
                // An acccessible folder will allow enumeration of its contents.
                Directory.EnumerateDirectories(directory);
            }
            catch (UnauthorizedAccessException)
            {
                // Return false if directory is unauthorized for user.
                return false;
            }

            return true;
        }
    }
}
