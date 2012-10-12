using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using PinkDao;
using PinkDao.Extensions;
using PinkoCommon;

namespace PinkoMocks
{
    /// <summary>
    /// Adhoc mock data
    /// </summary>
    static public class SampleMockData
    {
        /// <summary>
        /// PinkoMsgCalculateExpressionResult list
        /// </summary>
        static public List<PinkoMsgCalculateExpressionResult> GetPinkoMsgCalculateExpressionResult(int amt = 100)
        {
            return
                GetPinkoMsgCalculateExpression(amt)
                    .Select(x =>
                        {
                            var result = new PinkoMsgCalculateExpressionResult().FromRequest(x);
                            result.ResultsTupple = GetResultsTuppleWrapper().ToArray();

                            return result;
                        })
                    .ToList();

        }

        /// <summary>
        /// get sample PinkoFormPoint
        /// </summary>
        /// <returns></returns>
        static public List<ResultsTuppleWrapper> GetResultsTuppleWrapper()
        {
            return
                GetPinkoUserExpressionFormula()
                    .Select(x => new ResultsTuppleWrapper()
                        {
                            OriginalFormula = x,
                            PointSeries = GetPinkoFormPoint()
                        })
                    .ToList();
        }


        /// <summary>
        /// Get resulting tuple with envelop and expression
        /// </summary>
        static public Tuple<PinkoMsgCalculateExpressionResult, PinkoServiceMessageEnvelop, PinkoMsgCalculateExpression> GetResultTuple()
        {
            var expression = GetPinkoMsgCalculateExpression()[0];
            var resultMsg = new PinkoMsgCalculateExpressionResult().FromRequest(expression);
            var envelop = new PinkoServiceMessageEnvelop() { Message = expression };

            return new Tuple<PinkoMsgCalculateExpressionResult, PinkoServiceMessageEnvelop, PinkoMsgCalculateExpression>(resultMsg, envelop, expression);
        }

        /// <summary>
        /// get sample PinkoMsgCalculateExpression
        /// </summary>
        /// <returns></returns>
        static public List<PinkoMsgCalculateExpression> GetPinkoMsgCalculateExpression(int amt = 100)
        {
            var list = new List<PinkoMsgCalculateExpression>();

            for (var i = 0; i < amt; i++)
                list.Add(
                    new PinkoMsgCalculateExpression
                        {
                            ResultType = PinkoCalculateExpressionDaoExtensions.ResultDouble,
                            ExpressionFormulas = GetPinkoUserExpressionFormula(10),
                            //ExpressionFormulasStr = GetPinkoUserExpressionFormula(10).Select(x => string.Format("{0} = "))
                            DataFeedIdentifier =
                                {
                                    MaketEnvId = PinkoMarketEnvironmentMock.MockMarketEnvId,
                                    SubscribtionId = "subscription_" + i,
                                    SignalRId = "SignalRId_" + i,
                                    ClientCtx = "ClientCtx_" + i,
                                    WebRoleId = "WebRoleId_" + i,
                                    ClientId = "ClientId_" + i
                                }
                        });


            return list.Take(amt).ToList();
        }


        /// <summary>
        /// get sample expression tuple
        /// </summary>
        /// <returns></returns>
        static public PinkoUserExpressionFormula[] GetPinkoUserExpressionFormula(int amt = 100)
        {
            var list = new List<PinkoUserExpressionFormula>();

            for (var i2 = 0; i2 < amt; i2++)
                list.Add(new PinkoUserExpressionFormula
                    {
                        RuntimeId = i2,
                        FormulaId =
                            string.Format("{0}{1}D0-E0F1-4F00-B8B2-9EE03E92FE9B{2}", "{", i2.ToString("00000"), "}"),
                        ExpressionLabel = "Lbl_" + i2,
                        ExpressionFormula = string.Format("{0}.1 * {0}.2", i2)
                    });

            return list.Take(amt).ToArray();
        }

        /// <summary>
        /// get sample PinkoFormPoint
        /// </summary>
        /// <returns></returns>
        static public PinkoFormPoint[] GetPinkoFormPoint()
        {
            var list = new List<PinkoFormPoint>();

            Observable.Generate(1000, i => i <= 1200, i => i + 1, i => new PinkoFormPoint
                {
                    PointTime = i,
                    PointValue = i/10.0
                })
                .Subscribe(list.Add)
                ;

            return list.ToArray();
        }
        
        /// <summary>
        /// GetEntitytFormulaExpressionMocks
        /// </summary>
        /// <returns></returns>
        static public List<EntitytFormulaExpression> GetEntitytFormulaExpressionMocks()
        {
            return GetPinkoFormulaMocks()
                .Select(x => new EntitytFormulaExpression
                    {
                        Id = x.Id,
                        FormulaExp = x.ToExpressionStr(), 
                        DateTimeStamp = DateTime.Now.ToUniversalTime(),
                        LastUpdateStamp = DateTime.Now.ToUniversalTime()
                    })
                .ToList();
        }

        /// <summary>
        /// GetPinkoFormulaMocks
        /// </summary>
        /// <returns></returns>
        static public List<PinkoFormula> GetPinkoFormulaMocks()
        {
            var list = new List<PinkoFormula>();

            Observable.Generate(1000, i => i <= 1100, i => i+1, i => new PinkoFormula
            {
                Id = string.Format("{0}{1}8D0-E0F1-4F00-B8B2-9EE03E92FE9B{2}", "{", i.ToString("0000"), "}"),
                IdType = "Symbol",
                IdName = i.ToString(CultureInfo.InvariantCulture),
                FieldName = "Price.Bid",
                PinkoFormulaDictionaryId = string.Empty,
                Source = "Reuters"
            })
                .Subscribe(list.Add)
                ;

            return list;
        }


    }
}
