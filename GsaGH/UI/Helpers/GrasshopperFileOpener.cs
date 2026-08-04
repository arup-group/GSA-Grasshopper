using System;

using Grasshopper;
using Grasshopper.Kernel;

namespace GsaGH.UI.Helpers {
  internal static class GrasshopperFileOpener {
    internal static bool Open(string path) {
      var io = new GH_DocumentIO();
      if (!io.Open(path)) {
        return false;
      }

      Instances.DocumentEditor.Invoke((Action)(() => Instances.ActiveCanvas.Document = io.Document));
      return true;
    }
  }
}
