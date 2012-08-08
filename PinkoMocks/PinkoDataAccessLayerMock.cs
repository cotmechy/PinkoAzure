using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkoExpressionCommon.Interface;

namespace PinkoMocks
{
    public class PinkoDataAccessLayerMock : IPinkoDataAccessLayer
    {
        // -------------------
        // RForm("Symbol", "IBM", "Price.Bid", "Reuters")
        public double RForm(string s, string n, string t, string p)
        {
            // replace hard-coded placeholder with real data fetch
            // offset to create bid-ask spread
            var offset = t.EndsWith(".Bid") ? 0.98 : t.EndsWith(".Ask") ? 1.02 : 1.00;
            switch (n)
            {
                case "IBM":
                    return 9.56789 * offset;
                case "MSFT":
                    return 8.98765 * offset;
                case "C":
                    return 1.05656101417568 * offset;
                case "KO":
                    return 50.1423658105276 * offset;
                case "MMM":
                    return 32.011093772953 * offset;
                case "AA":
                    return 67.882480317672 * offset;
                case "XOM":
                    return 71.101375655784 * offset;
                case "GE":
                    return 91.2611518480168 * offset;
                case "WMT":
                    return 8.64368761360817 * offset;
                case "GM":
                    return 23.9532272908619 * offset;
                case "BA":
                    return 6.27700682090456 * offset;
                case "INTC":
                    return 15.1996232639996 * offset;
                case "JNJ":
                    return 35.4111540296167 * offset;
                case "CAT":
                    return 23.1723893541714 * offset;
                case "MCD":
                    return 35.7637921514752 * offset;
                case "PG":
                    return 46.3334673765737 * offset;
                case "DIS":
                    return 51.6337814981275 * offset;
                case "DD":
                    return 30.828918996653 * offset;
                case "HD":
                    return 92.8524155136442 * offset;
                case "VZ":
                    return 46.8101916587028 * offset;
                case "AXP":
                    return 10.722345398144 * offset;
                case "HON":
                    return 70.5709556911937 * offset;
                case "HPQ":
                    return 74.2998928177636 * offset;
                default:
                    return 0.0;
            }
        }
        // -------------------

        // -------------------
        // RHist("Symbol", "IBM", "Price.Bid", "Reuters", "Hour", 360)
        public double[] RHist(string s, string n, string t, string p, string w, int i)
        {
            // replace hard-coded placeholder with real data fetch
            // offset to create bid-ask spread
            var offset = t.EndsWith(".Bid") ? 0.98 : t.EndsWith(".Ask") ? 1.02 : 1.00;
            switch (n)
            {
                case "IBM":
                    return (new double[] { 8.34, 8.72, 7.91, 9.21, 9.15, 8.93, 7.94, 8.01, 8.17, 8.04, 9.7987, 8.19, 7.93, 9.29 }).Select(v => v * offset).ToArray();
                case "MSFT":
                    return (new double[] { 9.34, 6.543, 8.2591, 7.2521, 8.634, 8.8693, 8.6235, 8.5134, 8.558, 8.45627, 8.9873, 7.9877, 8.2393, 9.513429 }).Select(v => v * offset).ToArray();
                default:
                    return (new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 });
            }
        }
        // -------------------

    }
}
