using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhSA.Util.Gsa
{
    class ResultHelper
    {
        public static List<double> SmartRounder(List<double> values)
        {
            double max = values.Max();
            double min = values.Min();

            double val = Math.Max(Math.Abs(max), Math.Abs(min));

            double scale = Math.Pow(50, Math.Floor(Math.Log10(Math.Abs(val))) + 1);

            List<double> roundedvals = new List<double>();
            foreach (double value in values)
            {
                if (value == 0)
                    roundedvals.Add(0);
                else
                {
                    double tempval = scale* Math.Round(value / (scale), 4);
                    if (value > 0)
                    {
                        tempval = Math.Ceiling(tempval * 1000) / 1000;
                    }
                    else
                    {
                        tempval = Math.Floor(tempval * 1000) / 1000;
                    }
                    roundedvals.Add(tempval);
                }
            }
            return roundedvals;
        }
    }
}
