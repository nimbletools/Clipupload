using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace SelfHoster
{
  public class WebServer
  {
    public bool ws_bStarted = false;

    public string ws_strRootPath = "";
    public string ws_strAccessLog = "";

    public string ws_strInterface = "0.0.0.0";
    public int ws_iPort = 80;

    public TcpListener ws_listener;
    public Thread ws_thread;

    public Action<string, string, string, bool> ws_onAccess = null;

    public WebServer()
    {
    }

    public WebServer(string strInterface, int iPort)
    {
      ws_strInterface = strInterface;
      ws_iPort = iPort;
    }

    private void Loop()
    {
      while (true) {
        TcpClient client = ws_listener.AcceptTcpClient();
        new Thread(new ParameterizedThreadStart(HandleClient)).Start(client);
      }
    }

    private void HandleClient(object obj)
    {
      TcpClient client = (TcpClient)obj;
      NetworkStream ns = client.GetStream();

      string strIP = client.Client.RemoteEndPoint.ToString().Split(':')[0];
      string strURL = "";
      string strPath = "";
      bool bExists = false;

      string strLog = "[" + DateTime.Now.ToString() + "] " + strIP + " - ";

      using (StreamReader reader = new StreamReader(ns)) {
        try {
          string strGet = reader.ReadLine();
          if (strGet == null) {
            return;
          }
          string[] parse = strGet.Split(' ');

          string strMethod = parse[0];
          strURL = parse[1];

          strLog += strMethod + " \"" + strURL + "\" - ";

          if (!strURL.StartsWith("/")) {
            strURL = "/" + strURL;
          }

          while (strURL.Contains("..")) {
            strURL = strURL.Replace("..", "");
          }

          if (strURL.EndsWith("/")) {
            strURL += "index.html";
          }

          strPath = ws_strRootPath + strURL;
          bExists = File.Exists(strPath);

          int iReturnCode = 200;
          string strReturnText = "OK";
          string strContentType = "text/html";
          byte[] buffer = null;

          if (bExists) {
            string strExt = Path.GetExtension(strPath);
            switch (strExt) {
              case ".htm":
              case ".html": strContentType = "text/html"; break;
              case ".cs":
              case ".cpp":
              case ".h":
              case ".c":
              case ".css":
              case ".php":
              case ".lua":
              case ".py":
              case ".bs":
              case ".js":
              case ".txt": strContentType = "text/plain"; break;
              case ".png": strContentType = "image/png"; break;
              case ".jpg": strContentType = "image/jpeg"; break;
              case ".gif": strContentType = "image/gif"; break;
              default: strContentType = "application/octet-stream"; break;
            }
            buffer = File.ReadAllBytes(strPath);
          } else {
            iReturnCode = 404;
            strReturnText = "Not Found";
            buffer = Encoding.UTF8.GetBytes("<h1>File not found</h1>");
          }

          strLog += iReturnCode + " " + strReturnText + " (" + strContentType + ")";

          using (StreamWriter writer = new StreamWriter(ns)) {
            writer.WriteLine("HTTP/1.1 " + iReturnCode + " " + strReturnText);
            writer.WriteLine("Content-Type: " + strContentType);
            writer.WriteLine("Content-Length: " + buffer.Length);
            writer.WriteLine("Server: ClipUpload Self Hoster Addon");
            writer.WriteLine();
            writer.Flush();
            writer.BaseStream.Write(buffer, 0, buffer.Length);
          }

          client.Close();
        } catch (Exception ex) {
          Console.WriteLine("Exception: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
        }
      }

      if (ws_strAccessLog != "") {
        using (StreamWriter writer = new StreamWriter(File.Exists(ws_strAccessLog) ? File.Open(ws_strAccessLog, FileMode.Append) : File.Create(ws_strAccessLog))) {
          writer.WriteLine(strLog);
        }
      }

      if (ws_onAccess != null) {
        ws_onAccess(strIP, strURL, strPath, bExists);
      }
    }

    public void Start()
    {
      if (ws_listener != null) {
        Stop();
      }

      try {
        ws_listener = new TcpListener(IPAddress.Parse(ws_strInterface), ws_iPort);
        ws_listener.Start();

        ws_thread = new Thread(new ThreadStart(Loop));
        ws_thread.Start();

        ws_bStarted = true;
      } catch {
        ws_listener = null;
        ws_thread = null;

        throw new Exception("Failed to launch webserver. Port " + ws_iPort + " is probably already in use. Change in the settings!");
      }
    }

    public void Stop()
    {
      if (ws_listener == null) {
        return;
      }

      ws_listener.Stop();
      ws_listener = null;

      ws_thread.Abort();
      ws_thread = null;

      ws_bStarted = false;
    }
  }
}
