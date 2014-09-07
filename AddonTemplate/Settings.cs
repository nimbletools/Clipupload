using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using AddonHelper;

namespace $safeprojectname$
{
  public class $safeprojectname$Settings
  {
    [Description("Example string")]
    public string ExampleString = "example";

    [Description("Example boolean")]
    public bool ExampleBool = true;

    [Description("Example integer"), NumericSetting(0, 100)]
    public int ExampleInt = 5;
  }
}
