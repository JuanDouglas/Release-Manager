using Nexus.Github.Client;
using System;
using System.Diagnostics;
using System.Net;

namespace Nexus.Release.Manager
{
    public class Program
    {
        static OAuth auth;
        const string ClientId = "12b0da7399309b6c911c";
        const string ClientSecret = "008926850e5ed94a0a988268153e87a9767f79df";

        public static void Main(string[] args)
        {
            auth = new(ClientId, ClientSecret);
            Console.WriteLine();

            auth.RequestLoginAsync(TimeSpan.FromMinutes(5))
                   .Wait();
            Console.ReadLine();
        }
    }
}