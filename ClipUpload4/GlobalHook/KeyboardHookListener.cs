using System;
using System.Windows.Forms;
using GlobalHook.WinApi;

namespace GlobalHook
{
  /// <summary>
  /// This class monitors all keyboard activities and provides appropriate events.
  /// </summary>
  public class KeyboardHookListener : BaseHookListener
  {
    /// <summary>
    /// Initializes a new instance of <see cref="KeyboardHookListener"/>.
    /// </summary>
    /// <param name="hooker">Depending on this parameter the listener hooks either application or global keyboard events.</param>
    /// <remarks>Hooks are not active after instantiation. You need to use either <see cref="BaseHookListener.Enabled"/> property or call <see cref="BaseHookListener.Start"/> method.</remarks>
    public KeyboardHookListener(Hooker hooker)
      : base(hooker)
    {
    }

    /// <summary>
    /// This method processes the data from the hook and initiates event firing.
    /// </summary>
    /// <param name="wParam">The first Windows Messages parameter.</param>
    /// <param name="lParam">The second Windows Messages parameter.</param>
    /// <returns>
    /// True - The hook will be passed along to other applications.
    /// <para>
    /// False - The hook will not be given to other applications, effectively blocking input.
    /// </para>
    /// </returns>
    protected override bool ProcessCallback(int wParam, IntPtr lParam)
    {
      KeyEventArgsExt e = KeyEventArgsExt.FromRawData(wParam, lParam, IsGlobal);

      InvokeKeyDown(e);
      //InvokeKeyPress(wParam, lParam); // Messes up certain keyboard layouts with characters such as `, ~, ", ', ^, etc.
      //InvokeKeyUp(e); // Not required for ClipUpload 4

      return !e.Handled;
    }

    /// <summary>
    /// Returns the correct hook id to be used for <see cref="Hooker.SetWindowsHookEx"/> call.
    /// </summary>
    /// <returns>WH_KEYBOARD (0x02) or WH_KEYBOARD_LL (0x13) constant.</returns>
    protected override int GetHookId()
    {
      return IsGlobal ?
          GlobalHooker.WH_KEYBOARD_LL :
          AppHooker.WH_KEYBOARD;
    }

    /// <summary>
    /// Occurs when a key is preseed. 
    /// </summary>
    public event KeyEventHandler KeyDown;

    private void InvokeKeyDown(KeyEventArgsExt e)
    {
      if (KeyDown == null) return;
      if (e.Handled) return;
      if (!e.IsKeyDown) return;

      KeyDown(this, e);
    }

    /// <summary>
    /// Release delegates, unsubscribes from hooks.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    public override void Dispose()
    {
      KeyDown = null;

      base.Dispose();
    }
  }
}
