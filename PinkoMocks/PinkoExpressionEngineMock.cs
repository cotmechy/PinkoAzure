using System;
using PinkoExpressionCommon;

namespace PinkoMocks
{
    public class PinkoExpressionEngineMock : IPinkoExpressionEngine
    {
        public Func<IPinkoMarketEnvironment, T> ParseAndCompile<T>(string formula)
        {
            if (null != ExceptionParseAction)
                ExceptionParseAction();

            return environment => default(T);
        }

        public T Invoke<T>(IPinkoMarketEnvironment marketEnv, Func<IPinkoMarketEnvironment, T> paramFunc)
        {
            //if (typeof (T) == typeof (double[]))
            //    return new double[] {0.0};

            //if (typeof(T) == typeof(double[][]))
            //    return new double[] { new double[] {0.0} };

            if (null != ExceptionInvokeAction)
                ExceptionInvokeAction();

            return paramFunc(marketEnv);
        }

        public Action ExceptionParseAction = null;
        public Action ExceptionInvokeAction = null;
    }
}
