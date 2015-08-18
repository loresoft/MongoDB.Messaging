using System;
using System.Configuration;
using MongoDB.Driver;

namespace MongoDB.Messaging.Storage
{
    /// <summary>
    /// A helper class for getting MongoDB database connection.
    /// </summary>
    public static class MongoConnection
    {
        /// <summary>
        /// Gets the <see cref="IMongoDatabase"/> with the specified connection string.
        /// </summary>
        /// <param name="connectionString">The MongoDB connection string.</param>
        /// <returns>An instance of <see cref="IMongoDatabase"/>.</returns>
        public static IMongoDatabase GetConnection(string connectionString)
        {
            var mongoUrl = new MongoUrl(connectionString);
            return GetDatabase(mongoUrl);
        }

        /// <summary>
        /// Gets the <see cref="IMongoDatabase" /> with the specified connection name.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns>
        /// An instance of <see cref="IMongoDatabase" />.
        /// </returns>
        public static IMongoDatabase GetDatabase(string connectionName)
        {
            var mongoUrl = GetMongoUrl(connectionName);
            return GetDatabase(mongoUrl);
        }

        /// <summary>
        /// Gets the <see cref="IMongoDatabase" /> with the specified <see cref="MongoUrl" />.
        /// </summary>
        /// <param name="mongoUrl">The mongo URL.</param>
        /// <returns>
        /// An instance of <see cref="IMongoDatabase" />.
        /// </returns>
        public static IMongoDatabase GetDatabase(MongoUrl mongoUrl)
        {
            var client = new MongoClient(mongoUrl);
            var mongoDatabase = client.GetDatabase(mongoUrl.DatabaseName);
            return mongoDatabase;
        }

        /// <summary>
        /// Gets the <see cref="MongoUrl" /> with the specified connection name.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns>
        /// An instance of <see cref="MongoUrl" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="connectionName"/> is <see langword="null" />.</exception>
        /// <exception cref="ConfigurationErrorsException">No connection string could be found in the application configuration file.</exception>
        public static MongoUrl GetMongoUrl(string connectionName)
        {
            if (connectionName == null)
                throw new ArgumentNullException("connectionName");

            var settings = System.Configuration.ConfigurationManager.ConnectionStrings[connectionName];
            if (settings == null)
                throw new ConfigurationErrorsException(
                    string.Format("No connection string named '{0}' could be found in the application configuration file.", connectionName));

            string connectionString = settings.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                throw new ConfigurationErrorsException(
                    string.Format("The connection string '{0}' in the application's configuration file does not contain the required connectionString attribute.", connectionName));

            var mongoUrl = new MongoUrl(settings.ConnectionString);
            return mongoUrl;
        }
    }
}
