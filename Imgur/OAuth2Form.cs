using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MrAG.OAuth
{
  public partial class OAuth2Form : Form
  {
    private OAuth2 oauth;
    public string AccessToken;

    public OAuth2Form(OAuth2 oauth)
    {
      InitializeComponent();

      this.oauth = oauth;

      if (oauth.ServiceIcon != null)
        this.Icon = oauth.ServiceIcon;

      if (oauth.ServiceName != "")
        this.Text = "Authorize " + oauth.ServiceName;

      string authURL = oauth.GetAuthorizeURL();
      if (authURL != "")
        this.webAuth.Navigate(authURL);
      else
        this.webAuth.DocumentText = "<!DOCTYPE html><html><body><center><br/><br/><h1>" + this.oauth.ServiceName + " appears to be offline.</h1><p>Try again in a minute!</p><p><a href=\"" + authURL + "\">Retry now</a></p></center></body></html>";
    }

    private void webAuth_Navigated(object sender, WebBrowserNavigatedEventArgs e)
    {
      string strPinURL = oauth.EndPointURL + "/pin?pin=";
      if (e.Url.AbsoluteUri.StartsWith(strPinURL)) {
        string strPin = e.Url.AbsoluteUri.Substring(strPinURL.Length);

        this.webAuth.Visible = false;
        this.labelInfo.Visible = true;
        Application.DoEvents();

        oauth.AuthorizePin(strPin);

        this.Close();
      }
    }
  }
}
