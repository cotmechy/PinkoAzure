using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoWorkerCommon.ExceptionTypes
{
    /// <summary>
    /// Data requested is not available in dictionary
    /// </summary>
    public class PinkoExceptionDataNotSubscribed : Exception
    {
        /// <summary>
        /// Constructor - PinkoExceptionDataNotSubscribed 
        /// </summary>
        public PinkoExceptionDataNotSubscribed(string symbol, string ticker, string field, string source)
        {
            Symbol = symbol;
            Ticker = ticker;
            Field = field;
            FldSource = source;
        }

        public string Symbol;
        public string Ticker;
        public string Field;
        public string FldSource;
    }
}
