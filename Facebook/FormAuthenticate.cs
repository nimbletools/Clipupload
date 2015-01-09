using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Facebook
{
  public partial class FormAuthenticate : Form
  {
    public Facebook mainClass;

    public FormAuthenticate(Facebook mainClass, string url)
    {
      InitializeComponent();

      this.mainClass = mainClass;
      this.web.Navigate(url);
    }

    private void web_Navigating(object sender, WebBrowserNavigatingEventArgs e)
    {
      string url = e.Url.AbsoluteUri;
      if (url.Contains("/login_success.html?error=")) {
        this.Close();
      } else if (url.Contains("/login_success.html#access")) {
        try {
          this.Hide();

          string[] urlParse = url.Split(new string[] { "#access_token=", "&expires_in=" }, StringSplitOptions.None);
          this.mainClass.facebookClient.AccessToken = urlParse[1];
          this.mainClass.facebookName = (this.mainClass.facebookClient.Get("/me") as dynamic).name;

          // TODO: Use expires_in to shedule a re-authenticate when it's required

          this.Close();
        } catch { }
      }
    }
  }
}
