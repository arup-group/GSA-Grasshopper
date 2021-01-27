using Grasshopper.GUI;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GhSA.UI
{
    /// <summary>
    /// Class holding custom UI graphical buttons/boxes
    /// </summary>
    public class ComponentUI
    {
        public static float MaxTextWidth(List<string> spacerTxts, Font font)
        {
            float sp = new float(); //width of spacer text

            // adjust fontsize to high resolution displays
            font = new Font(font.FontFamily, font.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);

            for (int i = 0; i < spacerTxts.Count; i++)
            {
                if (GH_FontServer.StringWidth(spacerTxts[i], font) + 8 > sp)
                    sp = GH_FontServer.StringWidth(spacerTxts[i], font) + 8;
            }
            return sp;
        }
    }
}
