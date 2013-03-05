using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Glipho.OAuth.Providers.WebApiSample.Code
{
    using System.Configuration;
    using System.Web.Security;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public class Users
    {
        /// <summary>
        /// The users collection name.
        /// </summary>
        private const string UsersCollectionName = "users";

        /// <summary>
        /// Indicated if the indexes used by this class have been created.
        /// </summary>
        private static bool indexesCreated;

        /// <summary>
        /// The users collection.
        /// </summary>
        private MongoCollection<User> usersCollection;

        /// <summary>
        /// Initialises a new instance of the <see cref="Users" /> class.
        /// </summary>
        public Users()
        {
            // Get the default connection string and instantiate the client with it
            this.ConnectionString = ConfigurationManager.ConnectionStrings["Glipho.OAuth.Providers.Database.ConnectionString"].ConnectionString;
            this.InitialiseClass();
        }

        /// <summary>
        /// Gets the connection string for the client.
        /// </summary>
        public string ConnectionString { get; private set; }

        public void Create(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.InvalidUserName);
            }

            if (this.Exists(username))
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateUserName);
            }

            try
            {
                this.usersCollection.Save(new User
                {
                    Username = username
                });
            }
            catch (Exception)
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.ProviderError);
            }
        }

        public bool Exists(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            var query = Query<User>.EQ(u => u.Username, username.Trim().ToLower());
            return this.usersCollection.FindOne(query) != null;
        }

        /// <summary>
        /// Initialises the profiles collection from MongoDB.
        /// </summary>
        /// <typeparam name="T">The type of documents to initialise the collection with.</typeparam>
        /// <param name="collectionName">The name of the collection to initialise.</param>
        /// <returns>Initialised <see cref="MongoCollection"/> of <see cref="T"/>.</returns>
        /// <exception cref="OAuthException">Thrown if an error occurs while executing the requested command.</exception>
        private MongoCollection<T> InitialiseCollection<T>(string collectionName)
        {
            try
            {
                // Load up the database and collection.
                var url = new MongoUrl(this.ConnectionString);
                var client = new MongoClient(url.Url);
                var server = client.GetServer();
                var database = server.GetDatabase(url.DatabaseName);
                return database.GetCollection<T>(collectionName);
            }
            catch (MongoException ex)
            {
                throw new OAuthException("Unable initialise connection to the database.", ex, ErrorCode.Database);
            }
        }

        /// <summary>
        /// Initialise the class.
        /// </summary>
        private void InitialiseClass()
        {
            this.usersCollection = this.InitialiseCollection<User>(UsersCollectionName);
            this.EnsureIndexesExist();
        }

        /// <summary>
        /// Ensure the relevant indexes exist.
        /// </summary>
        private void EnsureIndexesExist()
        {
            if (indexesCreated)
            {
                return;
            }

            lock (UsersCollectionName)
            {
                if (indexesCreated)
                {
                    return;
                }

                var userUnique = IndexKeys<User>.Ascending(u => u.Username);
                var indexOptions = IndexOptions.SetBackground(true).SetUnique(true);
                this.usersCollection.EnsureIndex(userUnique, indexOptions);
                indexesCreated = true;
            }
        }
    }
}