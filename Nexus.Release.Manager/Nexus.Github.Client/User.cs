using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Github.Client
{
    public class User
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string URL { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        public int Id { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string Company { get; set; }
        public string TwitterUsername { get; set; }
        private User(UserResponse user)
        {
            Login = user.Login;
            Name = user.Name;
            Bio = user.Bio;
            URL = user.Url;
            Created = user.Created_at;
            Updated = user.Updated_at;
            Followers = user.Followers;
            Following = user.Following;
            Id = user.Id;
            Email = user.Email;
            Location = user.Location;
            Company = user.Company;
            TwitterUsername = user.Twitter_username;
        }
        public static async Task<User> GetUserAsync(GitAuthentication oAuth)
        {
            HttpClient httpClient = new();
            HttpRequestMessage requestMessage = oAuth.AuthenticatedMessage;
            HttpResponseMessage responseMessage = null;
            string responseString = string.Empty;
            UserResponse userResponse = null;

            requestMessage.RequestUri = new(oAuth.baseApiUrl+"user");
            responseMessage = await httpClient.SendAsync(requestMessage);
            responseString = await responseMessage.Content.ReadAsStringAsync();
            userResponse = JsonConvert.DeserializeObject<UserResponse>(responseString);

            return new(userResponse);
        }

        internal class UserResponse
        {
            public string Login { get; set; }
            public int Id { get; set; }
            public string Node_id { get; set; }
            public string Avatar_url { get; set; }
            public string Url { get; set; }
            public string Html_url { get; set; }
            public string Followers_url { get; set; }
            public string Following_url { get; set; }
            public string Gists_url { get; set; }
            public string starred_url { get; set; }
            public string Subscriptions_url { get; set; }
            public string Organizations_url { get; set; }
            public string Repos_url { get; set; }
            public string Events_url { get; set; }
            public string Received_events_url { get; set; }
            public string Type { get; set; }
            public bool Site_admin { get; set; }
            public string Name { get; set; }
            public string Company { get; set; }
            public string Blog { get; set; }
            public string Location { get; set; }
            public string Email { get; set; }
            public bool Hireable { get; set; }
            public string Bio { get; set; }
            public string Twitter_username { get; set; }
            public int Public_repos { get; set; }
            public int Public_gists { get; set; }
            public int Followers { get; set; }
            public int Following { get; set; }
            public DateTime Created_at { get; set; }
            public DateTime Updated_at { get; set; }
            public int Private_gists { get; set; }
            public int Total_private_repos { get; set; }
            public int Owned_private_repos { get; set; }
            public int Disk_usage { get; set; }
            public int Collaborators { get; set; }
            public bool Two_factor_authentication { get; set; }

        }
    }
}
