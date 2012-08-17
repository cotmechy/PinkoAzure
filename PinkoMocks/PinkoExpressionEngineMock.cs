using System;
using PinkoExpressionCommon;

namespace PinkoMocks
{
    public class PinkoExpressionEngineMock : IPinkoExpressionEngine
    {
        public Func<IPinkoMarketEnvironment, T> ParseAndCompile<T>(string formula)
        {
            if (null != ExceptionCall)
                ExceptionCall();

            return environment => default(T);
        }

        public T Invoke<T>(IPinkoMarketEnvironment marketEnv, Func<IPinkoMarketEnvironment, T> paramFunc)
        {
            return paramFunc(marketEnv);
        }

        public Action ExceptionCall = null;
    }
}
