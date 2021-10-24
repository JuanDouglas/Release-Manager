using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;

namespace Nexus.Github.Client;

public class GitAuthentication
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    internal string baseApiUrl => "https://api.github.com/";
    internal HttpRequestMessage AuthenticatedMessage
    {
        get
        {
            HttpRequestMessage requestMessage = new();
            requestMessage.Headers.Authorization = new("token", ClientToken);
            requestMessage.Headers.Add("User-Agent", UserAgent);
            return requestMessage;
        }
    }
    private readonly string requestUrl;
    private const string htmlClose = "<!DOCTYPE html><html><head></head><body onload=\"window.close()\">You can now close this page!</body></html>";
    public string UserCode { get; set; }
    internal string ClientToken { get; set; }
    public string UserAgent { get; set; }
    public GitAuthentication(string userAgent, string clientId, string clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
        UserAgent = userAgent;

        requestUrl = $"https://github.com/login/oauth/authorize?client_id={ClientId}";
    }

    public async Task RequestLoginAsync(TimeSpan maxAwait)
    {
        HttpClient httpClient = new();
        HttpRequestMessage requestMessage;
        string responseString = string.Empty;
        AccesTokenResponse tokenResponse = null;

        Task awaitResponse = Task.Run(() =>
        {
            AwaitGetCode("http://localhost:1337/", htmlClose);
        });

        ProcessStartInfo info = new(requestUrl);
        info.UseShellExecute = true;

        Process.Start(info);
        await awaitResponse.WaitAsync(maxAwait);

        string getAcessTokenUrl = $"https://github.com/login/oauth/access_token?client_id={ClientId}&client_secret={ClientSecret}&code={UserCode}";
        requestMessage = new(HttpMethod.Post, getAcessTokenUrl);
        requestMessage.Headers.Add("Accept", "application/json");
        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

        if (response.StatusCode != HttpStatusCode.OK)
            throw new ArgumentException("User Invalid Authentication");

        responseString = await response.Content.ReadAsStringAsync();
        tokenResponse = JsonConvert.DeserializeObject<AccesTokenResponse>(responseString);
        ClientToken = tokenResponse.access_token;
    }

    private void AwaitGetCode(string prefix, string serverResponse)
    {
        bool authenticated = false;
        HttpListener listener = new();
        listener.Prefixes.Add(prefix);
        listener.Start();

        while (!authenticated)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;


            //
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(serverResponse);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            Uri? uri = request.Url;

            if (uri == null)
                break;

            if (uri.LocalPath != "/Authentication/Github/Callback")
                break;

            NameValueCollection queryCollection = HttpUtility.ParseQueryString(uri.Query);

            if (queryCollection == null)
                break;

            string code = queryCollection["code"];

            if (code == null)
                break;

            UserCode = code;
            authenticated = true;
        }

        listener.Stop();
    }

    private class AccesTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
    }
}
