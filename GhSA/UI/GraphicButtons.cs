using System.Drawing;

namespace GhSA.UI.ButtonsUI
{
    /// <summary>
    /// Class holding custom UI graphical buttons/boxes
    /// </summary>
    public class CheckBox
    {
        /// <summary>
        /// Method to draw a check box with GSA-styling
        /// 
        /// Call this method when overriding Render method
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="center"></param>
        /// <param name="check"></param>
        /// <param name="activeFill"></param>
        /// <param name="activeEdge"></param>
        /// <param name="passiveFill"></param>
        /// <param name="passiveEdge"></param>
        /// <param name="size"></param>
        public static void DrawCheckButton(Graphics graphics, PointF center, bool check, Brush activeFill, Color activeEdge, Brush passiveFill, Color passiveEdge, int size)
        {
            // draws the check-button GSA styled
            //add scaler?

            if (check)
            {
                graphics.FillRectangle(activeFill, center.X - size / 2, center.Y - size / 2, size, size);
                Pen pen = new Pen(activeEdge);
                graphics.DrawRectangle(pen, center.X - size / 2, center.Y - size / 2, size, size);
                pen.Color = Color.White;
                pen.Width = size / 8;
                graphics.DrawLines(pen, new PointF[] { new PointF(center.X - size / 2 + pen.Width, center.Y), new PointF(center.X - pen.Width, center.Y + size / 2 - pen.Width), new PointF(center.X + size / 2 - pen.Width, center.Y - size / 2 + pen.Width) });
            }
            else
            {
                graphics.FillRectangle(passiveFill, center.X - size / 2, center.Y - size / 2, size, size);
                Pen pen = new Pen(passiveEdge)
                {
                    Width = size / 8
                };
                graphics.DrawRectangle(pen, center.X - size / 2, center.Y - size / 2, size, size);
            }

        }
    }

    /// <summary>
    /// Method to draw a dropdown arrow
    /// 
    /// Call this method when overriding Render method
    /// </summary>
    public class DropDownArrow
    {
        public static void DrawDropDownButton(Graphics graphics, PointF center, Color colour, int rectanglesize)
        {
            Pen pen = new Pen(new SolidBrush(colour))
            {
                Width = rectanglesize / 8
            };

            graphics.DrawLines(
                pen, new PointF[]
                {
                new PointF(center.X - rectanglesize / 4, center.Y - rectanglesize / 8),
                new PointF(center.X, center.Y + rectanglesize / 6),
                new PointF(center.X + rectanglesize / 4, center.Y - rectanglesize / 8)
                });

        }
    }
}
