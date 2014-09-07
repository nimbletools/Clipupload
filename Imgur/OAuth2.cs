using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Drawing;
using System.Windows.Forms;

namespace MrAG.OAuth
{
  /// <summary>
  /// Provices basic OAuth 2.0 functionality
  /// </summary>
  public class OAuth2
  {
    private string _EndPointURL;

    /// <summary>
    /// Gets or sets the OAuth endpoint URL
    /// </summary>
    public string EndPointURL
    {
      get { return this._EndPointURL; }
      set
      {
        this._EndPointURL = value;
        if (this._EndPointURL.EndsWith("/"))
          this._EndPointURL = value.Substring(0, value.Length - 1);
      }
    }

    /// <summary>
    /// Proxy to use
    /// </summary>
    public WebProxy Proxy { get; set; }

    /// <summary>
    /// Gets or sets the consumer key
    /// </summary>
    public string ClientID { get; set; }

    /// <summary>
    /// Gets or sets the consumer secret
    /// </summary>
    public string ClientSecret { get; set; }

    /// <summary>
    /// Gets the OAuth access token
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// Time by which an access token refresh is needed
    /// </summary>
    public DateTime RefreshNeeded { get; set; }

    /// <summary>
    /// Gets the OAuth refresh token
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the icon used for the authorization form. If no icon is provided, the form will have a default icon.
    /// </summary>
    public Icon ServiceIcon { get; set; }

    /// <summary>
    /// Gets or sets the service name used for the authorization form. If no name is provided, the form will have a default title.
    /// </summary>
    public string ServiceName { get; set; }

    /// <summary>
    /// Create an OAuth object with the given endpoint URL
    /// </summary>
    /// <param name="endpoint">The endpoint URL</param>
    public OAuth2(string endpoint, string clientID, string clientSecret)
    {
      this.Proxy = null;
      this.EndPointURL = endpoint;
      this.ClientID = clientID;
      this.ClientSecret = clientSecret;
      this.RefreshNeeded = new DateTime(0);
    }

    private Random rnd = new Random();
    /// <summary>
    /// Generates a random string
    /// </summary>
    /// <param name="len">Length of the random string</param>
    /// <param name="chars">Characters used to generate the string</param>
    /// <returns>The generated random string based on length and characters</returns>
    private string randomString(int len, string chars = "abcdefghijklmnopqrstuvwxyz0123456789")
    {
      string ret = "";
      for (int i = 0; i < len; i++)
        ret += chars[rnd.Next(chars.Length)].ToString();
      return ret;
    }

    /// <summary>
    /// Short hand alias for Uri.EscapeDataString
    /// </summary>
    /// <param name="str">The string to escape</param>
    /// <returns>An escaped string</returns>
    private string eds(string str)
    {
      return Uri.EscapeDataString(str);
    }

    /// <summary>
    /// Get the current epoch time
    /// </summary>
    /// <returns>The current epoch time</returns>
    private long epoch()
    {
      return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
    }

    /// <summary>
    /// Parses URI scheme data params
    /// </summary>
    /// <param name="input">The data string</param>
    /// <returns>Dictionary with parsed data</returns>
    private Dictionary<string, string> parseDataParams(string input)
    {
      Dictionary<string, string> ret = new Dictionary<string, string>();

      string currentName = "";
      string currentValue = "";
      bool readingValue = false;

      for (int i = 0; i < input.Length; i++) {
        switch (input[i]) {
          case '=':
            readingValue = true;
            break;

          case '&':
            ret[currentName] = Uri.UnescapeDataString(currentValue);
            currentName = "";
            currentValue = "";
            readingValue = false;
            break;

          default:
            if (!readingValue)
              currentName += input[i];
            else
              currentValue += input[i];
            break;
        }
      }

      if (currentName != "" && currentValue != "")
        ret[currentName] = Uri.UnescapeDataString(currentValue);

      return ret;
    }

    /// <summary>
    /// Internal function to download a URL with a maximum of 4 retries
    /// </summary>
    /// <param name="url">The URL</param>
    /// <returns>The result</returns>
    private string downloadURL(string url)
    {
      WebClient wc = new WebClient() { Proxy = this.Proxy };
      for (int i = 0; i < 4; i++) {
        try { return wc.DownloadString(url); } catch { }
      }
      return "failed";
    }

    /// <summary>
    /// Internal function to upload a URL with a maximum of 4 retries
    /// </summary>
    /// <param name="url">The URL</param>
    /// <param name="data">The data</param>
    /// <returns>The result</returns>
    private string uploadURL(string url, string data)
    {
      WebClient wc = new WebClient();
      wc.Proxy = this.Proxy;
      wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
      for (int i = 0; i < 4; i++) {
        try { return wc.UploadString(url, data); } catch { }
      }
      return "";
    }

    /// <summary>
    /// Get the user authorization URL
    /// </summary>
    /// <returns>The URL for user authorization</returns>
    public string GetAuthorizeURL()
    {
      return this.EndPointURL + "/authorize?client_id=" + ClientID + "&response_type=pin";
    }

    /// <summary>
    /// Authorize access tokens through the given grant type and code
    /// </summary>
    /// <param name="strGrantType">The grant type (refresh_token or pin)</param>
    /// <param name="strCode">The grant value</param>
    /// <returns>Whether it succeeded or not</returns>
    public bool AuthorizeToken(string strGrantType, string strCode)
    {
      string strResult = uploadURL(this.EndPointURL + "/token", "client_id=" + this.ClientID + "&client_secret=" + this.ClientSecret + "&grant_type=" + strGrantType + "&" + strGrantType + "=" + strCode);
      bool bSuccess = false;
      //TODO: Move Imgur.JSON into AddonHelper.JSON?
      dynamic result = Imgur.JSON.JsonDecode(strResult, ref bSuccess);
      if (bSuccess) {
        this.AccessToken = result["access_token"];
        this.RefreshNeeded = DateTime.Now + TimeSpan.FromSeconds(result["expires_in"]);
        this.RefreshToken = result["refresh_token"];
      }
      return bSuccess;
    }

    /// <summary>
    /// Authorize with pin code
    /// </summary>
    /// <param name="strPin">The pin code</param>
    /// <returns>Whether it succeeded or not</returns>
    public bool AuthorizePin(string strPin)
    {
      return AuthorizeToken("pin", strPin);
    }

    /// <summary>
    /// Authorize with refresh token
    /// </summary>
    /// <param name="strRefreshToken">The refresh token</param>
    /// <returns>Whether it succeeded or not</returns>
    public bool AuthorizeRefreshToken(string strRefreshToken)
    {
      return AuthorizeToken("refresh_token", strRefreshToken);
    }

    /// <summary>
    /// Authorize automatically using an authorization form
    /// </summary>
    /// <returns>The access token</returns>
    public string Authorize()
    {
      OAuth2Form newForm = new OAuth2Form(this);
      newForm.ShowDialog();
      return this.AccessToken;
    }

    /// <summary>
    /// Authorize automatically using an authorization form
    /// </summary>
    /// <param name="owner">The owner form</param>
    /// <returns>The access token</returns>
    public string Authorize(IWin32Window formOwner)
    {
      OAuth2Form newForm = new OAuth2Form(this);
      newForm.ShowDialog(formOwner);
      return this.AccessToken;
    }

    /// <summary>
    /// Handle checking if a refresh is needed and perform if so
    /// </summary>
    public void CheckRefresh()
    {
      if (DateTime.Now > this.RefreshNeeded) {
        AuthorizeRefreshToken(this.RefreshToken);
      }
    }

    /// <summary>
    /// Get an authenticated WebClient object to use for requests
    /// </summary>
    /// <returns>An authenticated WebClient object</returns>
    public WebClient AuthenticatedWebClient()
    {
      CheckRefresh();

      WebClient wc = new WebClient();
      wc.Proxy = this.Proxy;
      wc.Headers[HttpRequestHeader.UserAgent] = "MrAG.OAuth";
      wc.Headers[HttpRequestHeader.Authorization] = "Bearer " + AccessToken;

      return wc;
    }

    /// <summary>
    /// Get an authenticated HttpWebRequest object to use for requests
    /// </summary>
    /// <param name="url">The base URL</param>
    /// <returns>An authenticated HttpWebRequest object</returns>
    public HttpWebRequest AuthenticatedWebRequest(string url)
    {
      CheckRefresh();

      HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.Create(url);
      wr.Proxy = this.Proxy;
      wr.UserAgent = "MrAG.OAuth";
      wr.Headers[HttpRequestHeader.Authorization] = "Bearer " + AccessToken;

      return wr;
    }
  }
}
