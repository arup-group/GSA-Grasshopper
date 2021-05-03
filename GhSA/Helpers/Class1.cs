using DesignCheck2.Enums.Structural.EN;
namespace DesignCheck2.Enums.Structural.EN
{
    public enum NationalAnnex
    {
        BS,
        DIN,
        DS,
        EN,
        IS,
        NEN,
        NF,
        PN,
        SFS,
        UNE,
        UNI
    }
}

namespace DesignCheck2.Structural.EN.EN1993_1_1
{
    public static class PartialFactor
    {
        public static double Gamma_M0(NationalAnnex NA)
        {
            switch (NA)
            {
                case NationalAnnex.BS:
                    return 1.0;
                case NationalAnnex.DS:
                    return 1.1;
                default:
                    return 1.0;
            }
        }
        /// <summary>
        /// Use this method for Danish NA when gamma0 and gamma3 are not both = 1
        /// </summary>
        /// <param name="NA"></param>
        /// <param name="gamma_0"></param>
        /// <param name="gamma_3"></param>
        /// <returns></returns>
        public static double gamma_M0(NationalAnnex NA, double gamma_0 = 1, double gamma_3 = 1)
        {
            if (NA == NationalAnnex.DS)
                return Gamma_M0(NA) * gamma_0 * gamma_3;
            else
                return Gamma_M0(NA);
        }
    }
}


namespace DesignCheck2.Structural.EN.EN1993_1_1
{
    public static void MySteelCalc(NationalAnnex NA)
    {
        double gamma_m0 = gamma_m0(NA);
        double fyd = 355 / 
    }
}