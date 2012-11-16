using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoCommon
{
    /// <summary>
    /// Error codes
    /// </summary>
    public class PinkoErrorCode
    {
        public const int UnexpectedException = -1;
        public const int Success = 0;
        public const int InvalidFormula = 1000;
        public const int FormulaTypeNotSupported = 1001;
        public const int ActionNotSupported = 1002;
    }
}
