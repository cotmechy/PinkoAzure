﻿using System;
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
            switch (n)
            {
                case "IBM":
                    return 9.56789;
                case "MSFT":
                    return 8.98765;
                case "C":
                    return 1.05656101417568;
                case "KO":
                    return 50.1423658105276;
                case "MMM":
                    return 32.011093772953;
                case "AA":
                    return 67.882480317672;
                case "XOM":
                    return 71.101375655784;
                case "GE":
                    return 91.2611518480168;
                case "WMT":
                    return 8.64368761360817;
                case "GM":
                    return 23.9532272908619;
                case "BA":
                    return 6.27700682090456;
                case "INTC":
                    return 15.1996232639996;
                case "JNJ":
                    return 35.4111540296167;
                case "CAT":
                    return 23.1723893541714;
                case "MCD":
                    return 35.7637921514752;
                case "PG":
                    return 46.3334673765737;
                case "DIS":
                    return 51.6337814981275;
                case "DD":
                    return 30.828918996653;
                case "HD":
                    return 92.8524155136442;
                case "VZ":
                    return 46.8101916587028;
                case "AXP":
                    return 10.722345398144;
                case "HON":
                    return 70.5709556911937;
                case "HPQ":
                    return 74.2998928177636;
                default:
                    return double.NaN;
            }
        }
        // -------------------

        // -------------------
        // RHist("Symbol", "IBM", "Price.Bid", "Reuters", "Hour", 360)
        public double[] RHist(string s, string n, string t, string p, string w, int i)
        {
            switch (n)
            {
                case "IBM":
                    return IbmSeries;
                case "MSFT":
                    return MsftSeries;
                default:
                    return InvalidSeries;
            }
        }

        public readonly double[] IbmSeries = new double[] {  8.35,  8.72,   7.91,   9.21,   9.15,   8.93,   7.94,   8.01,   8.17,     8.04, 9.7987,   8.19,   7.93,     9.29 };
        public readonly double[] MsftSeries = new double[] { 9.34, 6.543, 8.2591, 7.2521,  8.634, 8.8693, 8.6235, 8.5134,   8.558, 8.45627, 8.9873, 7.9877, 8.2393, 9.513429 };
        public readonly double[] InvalidSeries = new double[] { double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN };
    }
}
