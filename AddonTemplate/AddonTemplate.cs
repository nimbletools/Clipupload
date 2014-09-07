using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Specialized;
using System.IO;
using AddonHelper;

namespace $safeprojectname$
{
  public class $safeprojectname$ : Addon
  {
    private AddonTemplateSettings m_Settings = new AddonTemplateSettings();
    private Bitmap m_BmpIcon;

    /// <summary>
    /// The constructor is always called when ClipUpload starts, whether the
    /// addon is enabled or disabled.
    /// </summary>
    public AddonTemplate()
    {
      // Either load an icon..
      //this.bmpIcon = new Icon(AddonPath + "/Icon.ico").ToBitmap();
      // Or generate one..
      m_BmpIcon = new Bitmap(16, 16);
      using (Graphics g = Graphics.FromImage(m_BmpIcon)) {
        g.FillRectangle(Brushes.LightBlue, new Rectangle(0, 0, 8, 8));
        g.FillRectangle(Brushes.LightGreen, new Rectangle(8, 0, 8, 8));
        g.FillRectangle(Brushes.LightPink, new Rectangle(0, 8, 8, 8));
        g.FillRectangle(Brushes.LightYellow, new Rectangle(8, 8, 8, 8));
      }
    }

    /// <summary>
    /// The deconstructor will only be called when ClipUpload closes. When
    /// a user disables the addon, the same instance will be used, and only
    /// Initialize() and Uninitialize() will be called.
    /// </summary>
    ~AddonTemplate()
    {

    }

    /// <summary>
    /// Get basic information about the addon here. This will be called when
    /// the addon is disabled as well.
    /// </summary>
    public override AddonInfo GetInfo()
    {
      return new AddonInfo() {
        Name = "$safeprojectname$",
        Author = "Your name here",
        Icon = m_BmpIcon,
        URL = "http://example.com/",
        URL_Author = "http://example.com/author/"
      };
    }

    /// <summary>
    /// Called when the addon intializes. This is called when ClipUpload
    /// starts, or when a user right-clicks the addon and clicks "Enable".
    /// Do not rely on this function to initialize values for GetInfo().
    /// </summary>
    public override void Initialize()
    {
      DeserializeSettings(m_Settings, "settings.txt");
    }

    /// <summary>
    /// Called when the addon uninitializes. This is called when ClipUpload
    /// closes, or when a user right-clicks the addon and clicks "Disable".
    /// Note that the instance of the object will not be destroyed, it will
    /// remain in memory. When a user re-enables the addon, Initialize() will
    /// be called on the same instance.
    /// </summary>
    public override void Uninitialize()
    {

    }

    /// <summary>
    /// Get entries for the context menu when a user right clicks the
    /// application icon.
    /// </summary>
    public override MenuEntry[] Menu()
    {
      List<MenuEntry> ret = new List<MenuEntry>();

      // entry for drag animation
      ret.Add(new MenuEntry() {
        IsAnimateItem = true,
        Text = "$safeprojectname$",
        Action = new Action(DragAnimation),
        Image = this.m_BmpIcon
      });

      // entry for drag screenshot
      ret.Add(new MenuEntry() {
        IsDragItem = true,
        Text = "$safeprojectname$",
        Action = new Action(DragScreenshot),
        Image = this.m_BmpIcon
      });

      // entry for upload paste
      ret.Add(new MenuEntry() {
        Visible = ClipboardContainsText
          || ClipboardContainsImage
          || ClipboardContainsFileList,
        Text = "$safeprojectname$",
        Action = new Action(UploadClipboard),
        Image = this.m_BmpIcon
      });

      return ret.ToArray();
    }

    /// <summary>
    /// Called when a user clicks on the "Settings" entry when right-clicking
    /// the addon in the main window.
    /// </summary>
    public override void Settings()
    {
      OpenQuickSettings(m_Settings, AddonPath + "/settings.txt", GetInfo().Name + " settings");
    }

    /// <summary>
    /// Function referenced from the Menu() function for dragging an animation.
    /// </summary>
    void DragAnimation()
    {
      Drag(new Action<DragCallback>(DragFinished), true);
    }

    /// <summary>
    /// Function referenced from the Menu() function for dragging a screenshot.
    /// </summary>
    void DragScreenshot()
    {
      Drag(new Action<DragCallback>(DragFinished));
    }

    /// <summary>
    /// Function referenced as the callback function for dragging a screesnhot
    /// and animation.
    /// </summary>
    void DragFinished(DragCallback cb)
    {
      if (cb.Type == DragCallbackType.Animation) {
        //TODO: Do something with cb.Animation (System.IO.MemoryStream)

        Uploaded("Image", "http://example.com/image.gif", "Dragged animation");

      } else if (cb.Type == DragCallbackType.Image) {
        //TODO: Do something with cb.Image (System.Drawing.Image)

        Uploaded("Image", "http://example.com/image.png", "Dragged image");

      }
    }

    /// <summary>
    /// Function referenced from the Menu() function for uploading the clipboard.
    /// </summary>
    void UploadClipboard()
    {
      if (ClipboardContainsImage) {
        Image image = Clipboard.GetImage();

        //TODO: Do something with image

        Uploaded("Image", "http://example.com/image.png", "Pasted image");

      } else if (ClipboardContainsText) {
        string text = Clipboard.GetText();

        //TODO: Do something with text

        Uploaded("Text", "http://example.com/text.txt", "Pasted text");

      } else if (ClipboardContainsFileList) {
        StringCollection files = Clipboard.GetFileDropList();

        List<UploadedFile> uploadedFiles = new List<UploadedFile>();

        foreach (string file in files) {
          //TODO: Do something with files

          uploadedFiles.Add(new UploadedFile() {
            URL = "http://example.com/" + Path.GetFileName(file),
            Info = (new FileInfo(file).Length / 1000) + " kB"
          });
        }

        UploadedFiles(uploadedFiles);
      }
    }
  }
}
