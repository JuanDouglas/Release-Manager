namespace Nexus.Github.Client.Repositories;

public class Asset
{
    public User Uploader { get; set; }
    public string Url { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Label { get; set; }
    public string State { get; set; }
    public long Size { get; set; }
    public int Downloads { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    internal Release Release { get; set; }
    internal GitAuthentication Auth { get; set; }
    internal Asset(in Release rel, in GitAuthentication authentication, AssetResponse asset)
    {
        Uploader = new(asset.Uploader);
        Url = asset.Url;
        Id = asset.Id;
        Name = asset.Name;
        Label = asset.Label;
        State = asset.State;
        Size = asset.Size;
        Downloads = asset.Download_count;
        Created = asset.Created_at;
        Updated = asset.Updated_at;
        Release = rel;
        Auth = authentication;
    }
    public byte[] DownloadAsync()
    {
        HttpClient httpClient = new();
        string url = Release.Repository.Url+"/";
        HttpRequestMessage requestMessage = Auth.AuthenticatedMessage;
        requestMessage.RequestUri = new Uri(Url);
        throw new NotImplementedException();
    }
    internal class AssetResponse
    {
        public string Url { get; set; }
        public string Browser_download_url { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string State { get; set; }
        public string Content_type { get; set; }
        public long Size { get; set; }
        public int Download_count { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public User.UserResponse Uploader { get; set; }
    }
}


