using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkDao;
using PinkoWorkerCommon.Interface;

namespace PinkoWorkerCommon.Utility
{
    public class PinkoFormulaDictionary : IPinkoFormulaDictionary
    {
        /// <summary>
        /// Update expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>True if updated. False if added.</returns>
        public bool UpdateFormula(PinkoMsgCalculateExpression expression)
        {
            return false;
        }

        //private ConcurrentDictionary<string, > 

        private ConcurrentDictionary<string, PinkoMsgCalculateExpression> _hash;
    }
}
