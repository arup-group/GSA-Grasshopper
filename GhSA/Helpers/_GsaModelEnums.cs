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
    /// <summary>
    /// Class to hold enums from GSA for Types for
    /// 1D Elements (GsaAPI.ElementType)
    /// 2D Elements (GsaAPI.AnalysisOrder)
    /// 1D Members (GsaAPI.MemberType)
    /// 2D Members (GsaAPI.MemberType)
    /// </summary>
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
        public static ElementType Element2dType(int type)
        {
            switch (type)
            {
                case 5:
                    {
                        return ElementType.QUAD4;
                    }
                case 6:
                    {
                        return ElementType.QUAD8;
                    }
                case 7:
                    {
                        return ElementType.TRI3;
                    }
                case 8:
                    {
                        return ElementType.TRI6;
                    }
                case 28:
                    {
                        return ElementType.TWO_D;
                    }
                case 31:
                    {
                        return ElementType.TWO_D_FE;
                    }
                case 32:
                    {
                        return ElementType.TWO_D_LOAD;
                    }
            }
            return ElementType.TWO_D;
        }
        public static AnalysisOrder AnalysisOrder(int type)
        {
            switch (type)
            {
                case 0:
                    {
                        return GsaAPI.AnalysisOrder.LINEAR;
                    }
                case 1:
                    {
                        return GsaAPI.AnalysisOrder.QUADRATIC;
                    }
                case 2:
                    {
                        return GsaAPI.AnalysisOrder.RIGID_DIAPHRAGM;
                    }
            }
            return GsaAPI.AnalysisOrder.LINEAR;
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
