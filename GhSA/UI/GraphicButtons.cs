using System.Drawing;
using System.Drawing.Drawing2D;
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
    /// Method to draw a rounded rectangle
    /// 
    /// Call this method when overriding Render method
    /// </summary>
    public class Button
    {
        public static GraphicsPath RoundedRect(RectangleF bounds, int radius, bool overlay = false)
        {
            RectangleF b = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            RectangleF arc = new RectangleF(b.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (overlay)
                b.Height = diameter;

            if (radius == 0)
            {
                path.AddRectangle(b);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = b.Right - diameter;
            path.AddArc(arc, 270, 90);

            if (!overlay)
            {
                // bottom right arc  
                arc.Y = b.Bottom - diameter;
                path.AddArc(arc, 0, 90);

                // bottom left arc 
                arc.X = b.Left;
                path.AddArc(arc, 90, 90);
            }
            else
            {
                path.AddLine(new PointF(b.X + b.Width, b.Y + b.Height), new PointF(b.X, b.Y + b.Height));
            }

            path.CloseFigure();
            return path;
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
