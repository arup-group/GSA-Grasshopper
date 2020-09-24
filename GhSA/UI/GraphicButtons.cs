using System.Drawing;

namespace GhSA.UI.ButtonsUI
{
    public class CheckBox
    {
        public static void DrawCheckButton(Graphics graphics, PointF center, bool check, Brush activeFill, Color activeEdge, Brush passiveFill, Color passiveEdge, int s)
        {
            // draws the check-button GSA styled
            //add scaler?

            if (check)
            {
                graphics.FillRectangle(activeFill, center.X - s / 2, center.Y - s / 2, s, s);
                Pen pen = new Pen(activeEdge);
                graphics.DrawRectangle(pen, center.X - s / 2, center.Y - s / 2, s, s);
                pen.Color = Color.White;
                pen.Width = s / 8;
                graphics.DrawLines(pen, new PointF[] { new PointF(center.X - s / 2 + pen.Width, center.Y), new PointF(center.X - pen.Width, center.Y + s / 2 - pen.Width), new PointF(center.X + s / 2 - pen.Width, center.Y - s / 2 + pen.Width) });
            }
            else
            {
                graphics.FillRectangle(passiveFill, center.X - s / 2, center.Y - s / 2, s, s);
                Pen pen = new Pen(passiveEdge);
                pen.Width = s / 8;
                graphics.DrawRectangle(pen, center.X - s / 2, center.Y - s / 2, s, s);
            }

        }
    }

    public class DropDownArrow
    {
        public static void DrawDropDownButton(Graphics graphics, PointF center, Color colour, int rectanglesize)
        {
            Pen pen = new Pen(new SolidBrush(colour));
            pen.Width = rectanglesize / 8;

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
