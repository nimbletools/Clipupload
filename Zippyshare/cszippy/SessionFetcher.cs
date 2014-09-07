using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace cszippy
{
  public class SessionFetcher
  {
    private static string DEFAULT_FETCH_URL = "http://zippyshare.com";
    private Regex uploadIdRegex = new Regex("var uploadId = '(.*)';");
    private Regex serverRegex = new Regex("var server = '(.*)';");
    private string url;
    private string uploadId;
    private string server;

    public SessionFetcher()
    {
      this.url = DEFAULT_FETCH_URL;
    }

    public SessionFetcher(string url)
    {
      this.url = url;
    }

    public void Fetch()
    {
      WebClient wc = new WebClient();
      wc.Proxy = null;

      string strResult = wc.DownloadString(url);

      Match matchID = uploadIdRegex.Match(strResult);
      if (!matchID.Success) {
        throw new IOException("Unable to find uploadId");
      }

      Match matchServer = serverRegex.Match(strResult);
      if (!matchServer.Success) {
        throw new IOException("Unable to find server");
      }

      uploadId = matchID.Groups[1].Value;
      server = matchServer.Groups[1].Value;
    }

    public string GetUploadID()
    {
      return uploadId;
    }

    public string GetServer()
    {
      return server;
    }
  }
}
