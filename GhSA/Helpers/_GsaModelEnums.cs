using System;
using System.Linq;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using GsaAPI;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace GhSA.Util.Gsa
{
    public class GsaToModel
    {
        public static ElementType Element1dType(int type)
        {
           switch(type)
            {
                case 1:
                    {
                        return ElementType.BAR;
                    }
                case 2:
                    {
                        return ElementType.BEAM;
                    }
                case 3:
                    {
                        return ElementType.SPRING;
                    }
                case 9:
                    {
                        return ElementType.LINK;
                    }
                case 10:
                    {
                        return ElementType.CABLE;
                    }
                case 19:
                    {
                        return ElementType.SPACER;
                    }
                case 20:
                    {
                        return ElementType.STRUT;
                    }
                case 21:
                    {
                        return ElementType.TIE;
                    }
                case 23:
                    {
                        return ElementType.ROD;
                    }
                case 24:
                    {
                        return ElementType.DAMPER;
                    }
            }
            return ElementType.BEAM;
        }
        public static AnalysisOrder Element2dType(int type)
        {
            switch (type)
            {
                case 0:
                    {
                        return AnalysisOrder.LINEAR;
                    }
                case 1:
                    {
                        return AnalysisOrder.QUADRATIC;
                    }
                case 2:
                    {
                        return AnalysisOrder.RIGID_DIAPHRAGM;
                    }
            }
            return AnalysisOrder.LINEAR;
        }


        public static MemberType Member1dType(int type)
        {
            switch (type)
            {
                case 2:
                    {
                        return MemberType.BEAM;
                    }
                case 6:
                    {
                        return MemberType.CANTILEVER;
                    }
                case 3:
                    {
                        return MemberType.COLUMN;
                    }
                case 8:
                    {
                        return MemberType.COMPOS;
                    }
                case 9:
                    {
                        return MemberType.PILE;
                    }
                case 11:
                    {
                        return MemberType.VOID_CUTTER_1D;
                    }
            }
            return MemberType.GENERIC_1D;
        }

        public static MemberType Member2dType(int type)
        {
            switch (type)
            {
                case 7:
                    {
                        return MemberType.RIBSLAB;
                    }
                case 4:
                    {
                        return MemberType.SLAB;
                    }
                case 12:
                    {
                        return MemberType.VOID_CUTTER_2D;
                    }
                case 5:
                    {
                        return MemberType.WALL;
                    }
            }
            return MemberType.GENERIC_2D;
        }
    }
}
