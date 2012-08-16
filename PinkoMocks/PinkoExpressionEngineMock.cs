using System;
using PinkoExpressionCommon;

namespace PinkoMocks
{
    public class PinkoExpressionEngineMock : IPinkoExpressionEngine
    {
        public Func<IPinkoMarketEnvironment, T> ParseAndCompile<T>(string formula)
        {
            return new Func<IPinkoMarketEnvironment, T>(environment => default(T));
        }

        public T Invoke<T>(IPinkoMarketEnvironment marketEnv, Func<IPinkoMarketEnvironment, T> paramFunc)
        {
            return paramFunc(marketEnv);
        }
    }
}
