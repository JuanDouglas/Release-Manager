using Newtonsoft.Json;
using Nexus.Github.Client.Repositories.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Github.Client.Repositories;
public class Repository
{
    public int Id { get; set; }
    public User Owner { get; set; }
    public string FullName { get; set; }
    public string Name { get; set; }
    public bool Private { get; set; }
    public bool Fork { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; }
    public DateTime Pushed { get; set; }
    public DateTime Updated { get; set; }
    public string[] Topics { get; set; }
    public long Size { get; set; }
    public string Language { get; set; }
    public string HomePage { get; set; }
    public int Forks { get; set; }
    public int Watchers { get; set; }
    public bool IsTemplate { get; set; }
    public bool HasIssues { get; set; }
    public bool HasProjects { get; set; }
    public bool HasWiki { get; set; }
    public bool HasDownloads { get; set; }
    public Repository? TemplateRepository { get; set; }
    internal GitAuthentication OAuth { get; set; }
    internal User User { get; set; }

    internal Repository(in GitAuthentication oAuth, in User user, RepositoryResponse rep) : this(oAuth, user)
    {
        Owner = new(rep.Owner);
        TemplateRepository = (rep.Template_repository!=null) ? new(oAuth, user, rep.Template_repository) : null;
        Fork = rep.Fork;
        Url = rep.Url;
        Description = rep.Description;
        Private = rep.Private;
        Name = rep.Name;
        FullName = rep.Full_name;
        Id = rep.Id;
        Forks  = rep.Forks_count;
        Watchers = rep.Watchers_count;
        HomePage = rep.Homepage;
        Language = rep.Language;
        Topics = rep.Topics;
        Updated = rep.Updated_at;
        Created =  rep.Created_at;
        Pushed = rep.Pushed_at;
        Size = rep.Size;
        IsTemplate = rep.Is_template;
        HasDownloads = rep.Has_downloads;
        HasWiki = rep.Has_wiki;
        HasIssues = rep.Has_issues;
        HasProjects = rep.Has_projects;

        OAuth = oAuth;
        User = user;
    }
    /// <summary>
    /// Incia um novo repositorio.
    /// </summary>
    /// <param name="oAuth">Git authentication</param>
    /// <param name="user">Inforemacoes do usuario</param>
    /// <exception cref="NullReferenceException">Excao gerada quando um dos parametros e nulo ou vazio</exception>
    public Repository(GitAuthentication oAuth, User user)
    {
        OAuth = oAuth ?? throw new NullReferenceException(nameof(oAuth));
        User = user ?? throw new NullReferenceException(nameof(user));
    }

    /// <summary>
    /// Obtem uma lista dos repositorios associados a este usuario.
    /// </summary>
    /// <param name="per_page">Quantidade maxima de repositorios por pagina (Maximo de 100).</param>
    /// <param name="page">Pagina atual</param>
    /// <param name="associationType">Tipo de associacao</param>
    /// <returns></returns>
    public static async Task<Repository[]> ListAsync(User user, GitAuthentication auth, int per_page, int page, RepositoryAssociationType associationType)
    {
        HttpClient client = new();
        string url = user.URL + $"/repos?page={page}&per_page={per_page}&type={Enum.GetName(typeof(RepositoryAssociationType), associationType)}";
        string responseString = string.Empty;
        HttpRequestMessage requestMessage = auth.AuthenticatedMessage;
        requestMessage.RequestUri = new Uri(url);

        HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        responseString = await responseMessage.Content.ReadAsStringAsync();

        RepositoryResponse[] repositoriesResponse = JsonConvert.DeserializeObject<RepositoryResponse[]>(responseString);

        List<Repository> repositories = new();

        foreach (RepositoryResponse item in repositoriesResponse)
        {
            repositories.Add(new(auth, user, item));
        }

        return repositories.ToArray();
    }
    /// <summary>
    /// Obtem as releases poor pagina 
    /// </summary>
    /// <param name="per_page">Quantidade maxima por pagina (Maximo de 100)</param>
    /// <param name="page">Pagina atual</param>
    /// <returns></returns>
    public async Task<Release[]> GetReleasesAsync(int per_page, int page)
    {
        HttpClient client = new();
        string url = Url + $"/releases?per_page={per_page}&page={page}";
        string responseString = string.Empty;
        HttpRequestMessage requestMessage = OAuth.AuthenticatedMessage;
        requestMessage.RequestUri = new Uri(url);
        HttpResponseMessage response = await client.SendAsync(requestMessage);

        responseString = await response.Content.ReadAsStringAsync();
        Release.ReleaseResponse[] releasesResponse = JsonConvert.DeserializeObject<Release.ReleaseResponse[]>(responseString);

        List<Release> releases = new();

        foreach (Release.ReleaseResponse relResponse in releasesResponse)
        {
            releases.Add(new(this, OAuth, relResponse));
        }

        return releases.ToArray();
    }
    internal class RepositoryResponse
    {
        public int Id { get; set; }
        public User.UserResponse Owner { get; set; }
        public string Node_id { get; set; }
        public string Name { get; set; }
        public string Full_name { get; set; }
        public bool Private { get; set; }
        public bool Fork { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Archive_url { get; set; }
        public string Releases_url { get; set; }
        public int Watchers_count { get; set; }
        public string Open_issues_count { get; set; }
        public bool Is_template { get; set; }
        public string[] Topics { get; set; }
        public bool Has_issues { get; set; }
        public bool Has_projects { get; set; }
        public bool Has_wiki { get; set; }
        public bool Has_downloads { get; set; }
        public int Forks_count { get; set; }
        public string Language { get; set; }
        public long Size { get; set; }
        public bool Archived { get; set; }
        public string Homepage { get; set; }
        public string Default_branch { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public DateTime Pushed_at { get; set; }
        public RepositoryResponse Template_repository { get; set; }
    }
}

