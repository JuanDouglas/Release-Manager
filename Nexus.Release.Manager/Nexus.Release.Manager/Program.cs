using Nexus.Github.Client;
using System;
using System.Diagnostics;
using System.Net;

namespace Nexus.Release.Manager
{
    public class Program
    {
        static GitAuthentication auth;
        const string ClientId = "12b0da7399309b6c911c";
        const string ClientSecret = "008926850e5ed94a0a988268153e87a9767f79df";
        const string UserAgent = "Nexus Release Manager";
        public static void Main(string[] args)
        {
            auth = new(UserAgent, ClientId, ClientSecret);
            Console.WriteLine();

            auth.RequestLoginAsync(TimeSpan.FromMinutes(5))
                   .Wait();
            Task<User> getUserTask = User.GetUserAsync(auth);
            getUserTask.Wait();

            User user = getUserTask.Result;
            Repositories repositories = new(auth, user);
            repositories.ListAsync(100, 1, RepositoryAssociationType.Owner).Wait();
            Console.ReadLine();
        }
    }
}