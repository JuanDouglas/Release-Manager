using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Github.Client.Repositories;

public class Release
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string HtmlUrl { get; set; }
    public string TagName { get; set; }
    public bool Draft { get; set; }
    public string Body { get; set; }
    public string Name { get; set; }
    public User Author { get; set; }
    public bool PreRelease { get; set; }
    public DateTime Created { get; set; }
    public DateTime Published { get; set; }
    public Asset[] Assets { get; set; }
    internal Repository Repository { get; set; }
    internal GitAuthentication Auth { get; set; }
    internal Release(in Repository rep, in GitAuthentication authentication, ReleaseResponse rel)
    {
        Created = rel.Created_at;
        Published = rel.Published_at;
        PreRelease = rel.Prerelease;
        Author = new(rel.Author);
        Name = rel.Name;
        TagName = rel.Tag_name;
        Draft = rel.Draft;
        Body = rel.Body;
        Id= rel.Id;
        Repository = rep;
        Auth = authentication;

        List<Asset> assets = new();

        foreach (Asset.AssetResponse asset in rel.Assets)
        {
            assets.Add(new(this, Auth, asset));
        }

        Assets = assets.ToArray();
    }
    internal class ReleaseResponse
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Html_url { get; set; }
        public string Tag_name { get; set; }
        public bool Draft { get; set; }
        public string Body { get; set; }
        public string Name { get; set; }
        public User.UserResponse Author { get; set; }
        public bool Prerelease { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Published_at { get; set; }
        public Asset.AssetResponse[] Assets { get; set; }

    }
}


