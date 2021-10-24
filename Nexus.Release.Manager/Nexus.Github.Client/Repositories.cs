using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Github.Client
{
    public class Repositories
    {
        public GitAuthentication OAuth { get; set; }
        public User User { get; set; }
        public Repositories(GitAuthentication oAuth, User user)
        {
            OAuth=oAuth ?? throw new NullReferenceException(nameof(oAuth));
            User=user ?? throw new NullReferenceException(nameof(user));
        }
        /// <summary>
        /// Obtem uma lista dos repositorios associados a este usuario.
        /// </summary>
        /// <param name="per_page">Por pagina (Maximo de 100)</param>
        /// <param name="page">Pagina atual</param>
        /// <param name="associationType">Tipo de associacao</param>
        /// <returns></returns>
        public async Task ListAsync(int per_page, int page, RepositoryAssociationType associationType)
        {
            HttpClient client = new();
            string url = User.URL + $"/repos?page={page}&per_page={per_page}&type={Enum.GetName(typeof(RepositoryAssociationType), associationType)}";
            string responseString = string.Empty;
            HttpRequestMessage requestMessage = OAuth.AuthenticatedMessage;
            requestMessage.RequestUri = new Uri(url);

            HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
            responseString = await responseMessage.Content.ReadAsStringAsync();

            RepositoryResponse[] repositories = JsonConvert.DeserializeObject<RepositoryResponse[]>(responseString);
        }

        private class RepositoryResponse
        {
            public int Id { get; set; }
            public User.UserResponse Owner { get; set; }
            public string Node_id { get; set; }
            public string Name { get; set; }
            public string Full_name { get; set; }
            public bool Private { get; set; }
            public bool Fork { get; set; }
            public string Url { get; set; }
        }
    }

    public enum RepositoryAssociationType : byte
    {
        All,
        Owner,
        Member
    }
}
