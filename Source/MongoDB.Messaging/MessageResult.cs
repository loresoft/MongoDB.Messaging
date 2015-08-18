using System;

namespace MongoDB.Messaging
{
    /// <summary>
    /// The processing result state
    /// </summary>
    public enum MessageResult
    {
        /// <summary>
        /// The message has not been processed.
        /// </summary>
        None,

        /// <summary>
        /// The message processed successfully.
        /// </summary>
        Successful,
        /// <summary>
        /// The message processed succesfully with warnings.
        /// </summary>
        Warning,
        /// <summary>
        /// The message processed with an error.
        /// </summary>
        Error,

    }
}