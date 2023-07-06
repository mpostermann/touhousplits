using System;
using System.Configuration;
using System.IO;

namespace TouhouSplits.Service.Exceptions
{
    /// <summary>
    /// The exception thrown when an IO error occurs while saving or loading configuration files
    /// </summary>
    public class ConfigurationIOException : ConfigurationErrorsException
    {
        /// <summary>
        /// The file which failed the operation
        /// </summary>
        public FileInfo Filepath { get; }

        public ConfigurationIOException(string message, FileInfo filepath, Exception innerException) :
            base(message, innerException)
        {
            Filepath = filepath;
        }
    }
}
