using System;
using System.Drawing;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects;
using Rhino.Display;

namespace GhSA.UI
{
    /// <summary>
    /// Colour class holding the main colours used in colour scheme. 
    /// Make calls to this class to be able to easy update colours.
    /// 
    /// </summary>
    public class Colour
    {
        // General colour scheme
        public static Color GsaGreen
        {
            get { return Color.FromArgb(255, 48, 170, 159); }
        }
        public static Color GsaDarkGreen
        {
            get { return Color.FromArgb(255, 27, 141, 133); }
        }
        public static Color GsaBlue
        {
            get { return Color.FromArgb(255, 99, 148, 237); }
        }
        public static Color GsaDarkBlue
        {
            get { return Color.FromArgb(255, 0, 92, 175); }
        }

        public static Color GsaLightGrey
        {
            get { return Color.FromArgb(255, 244, 244, 244); }
        }

        public static Color GsaDarkPurple
        {
            get { return Color.FromArgb(255, 136, 0, 136); }
        }

        public static Color GsaGold
        {
            get { return Color.FromArgb(255, 255, 183, 0); }
        }

        //Set colours for Component UI
        public static Brush ButtonColor
        {
            get { return new SolidBrush(GsaDarkBlue); }
        }

        public static Color BorderColour
        {
            get {  return GsaLightGrey; }
        }

        public static Color SpacerColour
        {
            get { return GsaDarkBlue; }
        }

        public static Brush AnnotationTextDark
        {
            get { return Brushes.Black; }
        }

        public static Brush AnnotationTextBright
        {
            get { return Brushes.White; }
        }
        public static Color ActiveColour
        {
            get { return GsaDarkBlue; }
        }

        public static Brush ActiveBrush
        {
            get { return new SolidBrush(ActiveColour); }
        }


        //Set colours for custom geometry
        public static Color Node
        {
            get { return GsaGreen; }
        }
        public static Color NodeSelected
        {
            get { return GsaDarkPurple; }
        }

        public static Color Member1dNode
        {
            get { return GsaDarkGreen; }
        }

        public static Color Member1dNodeSelected
        {
            get { return GsaGold; }
        }

        public static Color Element1dNode
        {
            get { return GsaDarkGreen; }
        }
        public static Color Element1dNodeSelected
        {
            get { return GsaDarkGreen; }
        }

        public static Color Member1d
        {
            get { return GsaGreen; }
        }

        public static Color Element1d
        {
            get { return GsaGreen; }
        }

        public static Color Member1dSelected
        {
            get { return GsaDarkPurple; }
        }

        public static Color Element1dSelected
        {
            get { return GsaDarkPurple; }
        }

        public static Color Member2dEdge
        {
            get { return GsaBlue; }
        }

        public static Color Member2dEdgeSelected
        {
            get { return GsaDarkPurple; }
        }

        public static Color Element2dEdge
        {
            get { return GsaBlue; }
        }

        public static Color Element2dEdgeSelected
        {
            get { return GsaDarkPurple; }
        }


        public static DisplayMaterial Member2dFace
        {
            get 
            {
                DisplayMaterial material = new DisplayMaterial
                {
                    Diffuse = Color.FromArgb(50, 150, 150, 150),
                    Emission = Color.FromArgb(50, 45, 45, 45),
                    Transparency = 0.1
                };
                return material;
            } 
        }

        public static DisplayMaterial Member2dFaceSelected
        {
            get
            {
                DisplayMaterial material = new DisplayMaterial
                {
                    Diffuse = Color.FromArgb(5, 150, 150, 150),
                    Emission = Color.FromArgb(5, 5, 5, 5),
                    Transparency = 0.2
                };
                return material;
            }
        }

        public static Color Member2dInclPt
        {
            get { return GsaGold; }
        }

        public static Color Member2dInclLn
        {
            get { return GsaGold; }
        }

    }
}
