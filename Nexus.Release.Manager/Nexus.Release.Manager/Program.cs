using Nexus.Github.Client;
using Nexus.Github.Client.Repositories;
using Nexus.Github.Client.Repositories.Enums;
using System;
using System.Diagnostics;
using System.Net;

namespace Nexus.Releases.Manager
{
    public class Program
    {
        static GitAuthentication auth;
        const string ClientId = "12b0da7399309b6c911c";
        const string ClientSecret = "65a7f8acccc2c7d4bc6509601f9fae05b79ff33e";
        const string UserAgent = "Nexus Release Manager";
        public static void Main(string[] args)
        {
            auth = new GitAuthentication(UserAgent, ClientId, ClientSecret);
            Console.WriteLine();

            auth.RequestLoginAsync(TimeSpan.FromMinutes(5))
                   .Wait();
            Task<User> getUserTask = User.GetUserAsync(auth);
            getUserTask.Wait();

            User user = getUserTask.Result;
            Task<Repository[]> listRepositoriesTask = Repository.ListAsync(user, auth, 100, 1, RepositoryAssociationType.Owner);
            listRepositoriesTask.Wait();

            Repository[] repositories = listRepositoriesTask.Result;

            foreach (Repository rep in repositories)
            {
                if (rep.Name == "Nexus-Validations-Tools")
                {
                    Task<Release[]> getReleaseTask = rep.GetReleasesAsync(100, 1);
                    getReleaseTask.Wait();

                    Release[] releases = getReleaseTask.Result;
                }
            }
            Console.ReadLine();
        }
    }
}