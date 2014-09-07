using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using AddonHelper;

namespace Zippyshare
{
  public class ZippyshareSettings
  {
    [Description("Example string")]
    public string ExampleString = "example";

    [Description("Example boolean")]
    public bool ExampleBool = true;

    [Description("Example integer"), NumericSetting(0, 100)]
    public int ExampleInt = 5;
  }
}
