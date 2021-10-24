using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;

namespace Nexus.Github.Client;
/// <summary>
/// Credenciais OAuth do usuario no Github.
/// </summary>
public class GitAuthentication
{
    /// <summary>
    /// Id do client que ira usar o OAuth (Id da empresa).
    /// </summary>
    public string ClientId { get; set; }
    /// <summary>
    /// Token secreto do Client que 
    /// </summary>
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
    /// <summary>
    /// Codigo do usuario que se conectou a sua aplicacao com o Github.
    /// </summary>
    public string UserCode { get; set; }
    /// <summary>
    /// Token de autenticao unico gerado para essa sessao (E enviado no header http Authorization).
    /// </summary>
    internal string ClientToken { get; set; }
    /// <summary>
    /// Nome do agente que este cliente esta usando.
    /// </summary>
    public string UserAgent { get; set; }
    /// <summary>
    /// Inicia as credenciais com as informacoes do cliente.
    /// </summary>
    /// <param name="userAgent">Nome do cliente que ira se conectar.</param>
    /// <param name="clientId">Id do Cliente</param>
    /// <param name="clientSecret">Token secreto do cliente</param>
    public GitAuthentication(string userAgent, string clientId, string clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
        UserAgent = userAgent;

        requestUrl = $"https://github.com/login/oauth/authorize?client_id={ClientId}";
    }
    /// <summary>
    /// Obtem as credenciais do usuario que se conectou no servidor OAuth
    /// </summary>
    /// <param name="maxAwait">Tempo maximo de espera</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Essa excecao e lancada quando o usuario informa credeciais invalidas.</exception>
    public async Task RequestLoginAsync(TimeSpan maxAwait)
    {
        HttpClient httpClient = new();
        HttpRequestMessage requestMessage;
        string responseString = string.Empty;
        AccesTokenResponse tokenResponse = null;

        //Inicia um listner local para esperar o redirecionamento do github com o codigo do usuario 
        Task awaitResponse = Task.Run(() =>
        {
            AwaitGetCode("http://localhost:1337/", htmlClose);
        });

        ProcessStartInfo info = new(requestUrl);
        info.UseShellExecute = true;

        //Envia uma requisicao para o sevidor OAuth do github usando o navegador padrao 
        Process.Start(info);
        await awaitResponse.WaitAsync(maxAwait);

        //Obtem o codigo de acesso da API 
        string getAcessTokenUrl = $"https://github.com/login/oauth/access_token?client_id={ClientId}&client_secret={ClientSecret}&code={UserCode}";
        requestMessage = new(HttpMethod.Post, getAcessTokenUrl);
        requestMessage.Headers.Add("Accept", "application/json");
        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

        if (response.StatusCode != HttpStatusCode.OK)
            throw new ArgumentException("User Invalid Authentication!");

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
