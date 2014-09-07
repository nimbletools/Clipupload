using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MultipartData;
using System.Net;
using System.IO;
using System.Threading;

namespace cszippy
{
  public class FileUploader
  {
    private static Regex URL_REGEX = new Regex("value=\\\"(.*file\\.html)\\\"");
    private string server;
    private string uploadId;
    private string filename;
    private MemoryStream ms;

    public WebProxy Proxy = null;

    public Action<long> OnStart;
    public Func<long, long, bool> OnProgess;
    public Action<bool, string> OnFinished;

    public FileUploader(string server, string uploadId, string filename, MemoryStream ms)
    {
      this.server = server;
      this.uploadId = uploadId;
      this.filename = filename;
      this.ms = ms;
    }

    public string Upload()
    {
      MultipartEntity entity = new MultipartEntity();
      entity.AddBody(new StringBody(Encoding.ASCII, "uploadId", uploadId));
      entity.AddBody(new FileBody("file_0", filename, ms));
      entity.AddBody(new StringBody(Encoding.ASCII, "private", "yes"));

      byte[] writeData = entity.Content;

      HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://" + server + ".zippyshare.com/upload");
      req.UserAgent = "Mozilla/3.0 (compatible; Indy Library)";
      req.Proxy = Proxy;
      req.Method = "POST";
      req.ContentType = entity.ContentType;
      req.ContentLength = writeData.Length;

      Stream stream = req.GetRequestStream();

      OnStart(writeData.LongLength);

      using (MemoryStream mss = new MemoryStream(writeData)) {
        int sr = 1024;
        for (int i = 0; i < mss.Length; i += 1024) {
          if (mss.Length - i < 1024)
            sr = (int)mss.Length - i;
          else
            sr = 1024;

          byte[] buffer = new byte[sr];
          mss.Seek((long)i, SeekOrigin.Begin);
          mss.Read(buffer, 0, sr);
          stream.Write(buffer, 0, sr);

          if (!OnProgess(i, writeData.LongLength)) {
            req.Abort();
            mss.Dispose();

            req = null;

            OnFinished(false, "Canceled");
            return "";
          }
        }
      }

      WebResponse response = req.GetResponse();
      StreamReader reader = new StreamReader(response.GetResponseStream());

      Match matchURL = URL_REGEX.Match(reader.ReadToEnd());
      if (!matchURL.Success) {
        throw new IOException("Unable to find URL");
      }

      OnFinished(true, "");
      return matchURL.Groups[1].Value;
    }
  }
}
