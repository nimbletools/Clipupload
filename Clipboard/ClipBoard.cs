using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using AddonHelper;
using System.Diagnostics;

namespace ClipBoard
{
  public class ClipBoard : Addon
  {
    public Settings settings;

    public string shortCutDragModifiers = "";
    public string shortCutDragKey = "";

    private Bitmap bmpIcon;

    public ClipBoard()
    {
      this.bmpIcon = new Icon(AddonPath + "/Icon.ico").ToBitmap();
    }

    public override AddonInfo GetInfo()
    {
      return new AddonInfo() {
        Name = "ClipBoard",
        Author = "Jed",
        Icon = bmpIcon,
        URL_Author = "https://github.com/jlippold"
      };
    }

    public override void Initialize()
    {
      this.settings = new Settings(AddonPath + "/settings.txt");

      LoadSettings();
    }

    public override void Uninitialize()
    {
    }

    public void LoadSettings()
    {
      shortCutDragModifiers = settings.GetString("ShortcutDragModifiers");
      shortCutDragKey = settings.GetString("ShortcutDragKey");
    }

    public override MenuEntry[] Menu()
    {
      List<MenuEntry> ret = new List<MenuEntry>();

      ret.Add(new MenuEntry() {
        IsDragItem = true,
        Text = "ClipBoard",
        Image = this.bmpIcon,
        Action = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallback)); }),
        ShortcutModifiers = this.shortCutDragModifiers,
        ShortcutKey = this.shortCutDragKey
      });

      return ret.ToArray();
    }

    public void DragCallback(DragCallback callback)
    {
      switch (callback.Type) {
        case DragCallbackType.Image:
          UploadImage(callback.Image);
          break;
      }
    }

    public void UploadImage(Image img)
    {
      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      img = ImagePipeline(img);

      Clipboard.SetImage(img);
      img.Dispose();

      Tray.ShowBalloonTip(1000, "Copy success!", "Image copied to clipboard.", ToolTipIcon.Info);

      Tray.Icon = defIcon;
    }

    public override void Settings()
    {
      new FormSettings(this).ShowDialog();
    }

    public void Upload()
    {
      if (Clipboard.ContainsImage())
        UploadImage(Clipboard.GetImage());
    }
  }
}
