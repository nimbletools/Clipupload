using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

/* Copyright (c) 2010 CodeScales.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

// files have been modified by Angelo Geels to form a MultipartData namespace
// for use with multipart data encodings.

namespace MultipartData
{
  [Serializable]
  public class NameValuePair
  {
    private string m_name;
    private string m_value;

    public NameValuePair()
    {
    }

    public NameValuePair(string name, string value)
    {
      this.m_name = name;
      this.m_value = value;
    }

    public string Name
    {
      get { return m_name; }
      set { m_name = value; }
    }

    public string Value
    {
      get { return this.m_value; }
      set { this.m_value = value; }
    }
  }

  internal class HTTPProtocol
  {
    internal static void AddPostParameters(List<NameValuePair> parameters, StringBuilder builder)
    {
      int counter = 0;
      foreach (NameValuePair pair in parameters) {
        builder.Append(pair.Name + "=" + HttpUtility.UrlEncode(pair.Value));
        if (counter < parameters.Count - 1) {
          builder.Append("&");
        }
        counter++;
      }
    }

    internal static string GetPostParameter(string name, string value, string boundry)
    {
      StringBuilder builder = new StringBuilder();
      string paramBoundry = "--" + boundry + "\r\n";
      string stringParam = "Content-Disposition: form-data; name=\"";
      string paramEnd = "\"\r\n\r\n";
      builder.Append(paramBoundry);
      builder.Append(stringParam + name + paramEnd + value + "\r\n");
      return builder.ToString();
    }

    internal static string AddPostParametersFile(string name, string fileName, string boundry, string contentType)
    {
      if (name == null) {
        name = string.Empty;
      }
      if (fileName == null) {
        fileName = string.Empty;
      }

      StringBuilder builder = new StringBuilder();
      string paramBoundry = "--" + boundry + "\r\n";
      string stringParam = "Content-Disposition: form-data; name=\"";
      string paramEnd = "\"; filename=\"" + fileName + "\"\r\nContent-Type: " + contentType + "\r\n\r\n";
      builder.Append(paramBoundry);
      builder.Append(stringParam + name + paramEnd);
      return builder.ToString();
    }

    internal static string AddPostParametersEnd(string boundry)
    {
      return "--" + boundry + "--\r\n\r\n";
    }
  }

  public interface Body
  {
    byte[] GetContent(string boundry);
  }

  public class FileBody : Body
  {
    private string m_name;
    private string m_fileName;
    private byte[] m_content;
    private string m_mimeType;

    public FileBody(string name, string fileName, FileInfo fileInfo, string mimeType)
      : this(name, fileName, fileInfo)
    {
      this.m_mimeType = mimeType;
    }

    public FileBody(string name, string fileName, FileInfo fileInfo)
    {
      this.m_name = name;
      this.m_fileName = fileName;
      this.m_content = null;

      if (fileInfo == null) {
        this.m_content = new byte[0];
      } else {
        using (BinaryReader reader = new BinaryReader(fileInfo.OpenRead())) {
          this.m_content = reader.ReadBytes((int)reader.BaseStream.Length);
        }
      }
    }

    public FileBody(string name, string fileName, MemoryStream ms)
    {
      this.m_name = name;
      this.m_fileName = fileName;
      this.m_content = ms.ToArray();
    }

    public byte[] GetContent(string boundry)
    {
      List<byte> bytes = new List<byte>();
      if (this.m_content.Length == 0 || this.m_mimeType == null || this.m_mimeType.Equals(string.Empty)) {
        bytes.AddRange(Encoding.ASCII.GetBytes(HTTPProtocol.AddPostParametersFile(this.m_name, this.m_fileName, boundry, "application/octet-stream")));
      } else {
        bytes.AddRange(Encoding.ASCII.GetBytes(HTTPProtocol.AddPostParametersFile(this.m_name, this.m_fileName, boundry, this.m_mimeType)));
      }
      bytes.AddRange(this.m_content);
      bytes.AddRange(Encoding.ASCII.GetBytes("\r\n"));
      return bytes.ToArray();
    }
  }

  public class StringBody : Body
  {
    private Encoding m_encoding;
    private string m_name;
    private string m_value;

    public StringBody(Encoding encoding, string name, string value)
    {
      this.m_encoding = encoding;
      this.m_name = name;
      this.m_value = value;
    }

    public byte[] GetContent(string boundry)
    {
      return this.m_encoding.GetBytes(HTTPProtocol.GetPostParameter(this.m_name, this.m_value, boundry));
    }
  }

  public class MultipartEntity
  {
    private List<Body> m_bodyList = new List<Body>();
    private string m_boundry;

    public MultipartEntity()
    {
      GenerateBoundry();
    }

    public void AddBody(Body body)
    {
      this.m_bodyList.Add(body);
    }

    public string ContentEncoding
    {
      get { return null; }
      set { }
    }

    public string ContentType
    {
      get { return "multipart/form-data; boundary=" + m_boundry; }
      set { }
    }

    public byte[] Content
    {
      get
      {
        List<byte> byteList = new List<byte>();
        foreach (Body body in this.m_bodyList) {
          byteList.AddRange(body.GetContent(this.m_boundry));
        }
        if (this.m_bodyList.Count > 0) {
          byteList.AddRange(Encoding.ASCII.GetBytes(HTTPProtocol.AddPostParametersEnd(this.m_boundry)));
        }
        return byteList.ToArray();
      }
      set { }
    }

    public long ContentLength
    {
      get
      {
        return this.Content.Length;
      }
      set { }
    }

    public bool IsChunked
    {
      get
      {
        return false;
      }
      set { }
    }

    private static string allowedCharacters = "abcdefghijklmnopqrstuvwxyz1234567890";
    private void GenerateBoundry()
    {
      Random random = new Random(DateTime.Now.Millisecond);
      StringBuilder sb = new StringBuilder();
      sb.Append("---------------------------");
      for (int i = 0; i < 14; i++) {
        sb.Append(allowedCharacters[random.Next(allowedCharacters.Length - 1)]);
      }
      this.m_boundry = sb.ToString();
    }
  }
}
