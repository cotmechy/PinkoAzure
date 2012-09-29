namespace PinkDao
{
    /// <summary>
    /// Outgoing Calculation Result message
    /// </summary>
    public class PinkoMsgCalculateExpressionResult 
    {
        public PinkoDataFeedIdentifier DataFeedIdentifier = new PinkoDataFeedIdentifier();
        public int ResultType;    
        public ResultsTuppleWrapper[] ResultsTupple = PinkoCalculateExpressionDaoExtensions.DefaultResultTupple;
    }

    /// <summary>
    /// Wrapper to use as tuple.  Tuple cannot be serialized due to missing default constructor.
    /// </summary>
    public struct ResultsTuppleWrapper
    {
        public PinkoUserExpressionFormula OriginalFormula;
        public PinkoFormPoint[] PointSeries;
    }

}