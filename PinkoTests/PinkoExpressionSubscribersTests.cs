using System;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoMocks;
using PinkoWorkerCommon.Utility;

namespace PinkoTests
{
    [TestClass]
    public class PinkoExpressionSubscribersTests
    {
        /// <summary>
        /// Update
        /// </summary>
        [TestMethod]
        public void TestPinkoExpressionSubscribersRemove()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var subscribers = pinkoContainer.Resolve<PinkoExpressionSubscribers<double[]>>();

            // 1st one - new
            var exp = SampleMockData.GetPinkoMsgCalculateExpressionResult()[0];
            exp.DataFeedIdentifier.SubscribtionId = "SubId1";
            exp.DataFeedIdentifier.ClientCtx = "ClientCtx1";
            subscribers.UpdateSubscriber(exp, null);

            exp = exp = SampleMockData.GetPinkoMsgCalculateExpressionResult()[0];
            exp.DataFeedIdentifier.SubscribtionId = "SubId1";
            exp.DataFeedIdentifier.ClientCtx = "ClientCtx1";
            subscribers.UpdateSubscriber(exp, null);

            // Add  second one
            exp = exp = SampleMockData.GetPinkoMsgCalculateExpressionResult()[0];
            exp.DataFeedIdentifier.SubscribtionId = "SubId1";
            exp.DataFeedIdentifier.ClientCtx = "ClientCtx2";
            subscribers.UpdateSubscriber(exp, null);

            // Add  second one
            exp = exp = SampleMockData.GetPinkoMsgCalculateExpressionResult()[0];
            exp.DataFeedIdentifier.SubscribtionId = "SubId3";
            exp.DataFeedIdentifier.ClientCtx = "ClientCtx3";
            subscribers.UpdateSubscriber(exp, null);

            Assert.IsTrue(subscribers.Subscribers.Count == 2);
            Assert.IsTrue(subscribers.Subscribers["SubId1"].Subcribers.Count == 2);
            Assert.IsTrue(subscribers.Subscribers["SubId3"].Subcribers.Count == 1);

            // Remove tests
            exp.DataFeedIdentifier.SubscribtionId = "SubId3";
            exp.DataFeedIdentifier.ClientCtx = "ClientCtx3";
            Assert.IsNotNull(subscribers.RemoveSubscriber(exp));
            Assert.IsTrue(subscribers.Subscribers.Count == 1);
            Assert.IsTrue(subscribers.Subscribers.ContainsKey("SubId1"));
            Assert.IsTrue(subscribers.Subscribers["SubId1"].Subcribers.Exists(x => x.Item2.DataFeedIdentifier.ClientCtx == "ClientCtx1"));
            Assert.IsTrue(subscribers.Subscribers["SubId1"].Subcribers.Exists(x => x.Item2.DataFeedIdentifier.ClientCtx == "ClientCtx2"));
            Assert.IsFalse(subscribers.Subscribers.ContainsKey("SubId3"));

            // Remove tests
            exp.DataFeedIdentifier.SubscribtionId = "SubId1";
            exp.DataFeedIdentifier.ClientCtx = "ClientCtx2";
            Assert.IsNotNull(subscribers.RemoveSubscriber(exp));
            Assert.IsTrue(subscribers.Subscribers.Count == 1);
            Assert.IsTrue(subscribers.Subscribers.ContainsKey("SubId1"));
            Assert.IsTrue(subscribers.Subscribers["SubId1"].Subcribers.Exists(x => x.Item2.DataFeedIdentifier.ClientCtx == "ClientCtx1"));
            Assert.IsFalse(subscribers.Subscribers["SubId1"].Subcribers.Exists(x => x.Item2.DataFeedIdentifier.ClientCtx == "ClientCtx2"));

            // Remove tests
            exp.DataFeedIdentifier.SubscribtionId = "SubId1";
            exp.DataFeedIdentifier.ClientCtx = "ClientCtx1";
            Assert.IsNotNull(subscribers.RemoveSubscriber(exp));
            Assert.IsTrue(subscribers.Subscribers.Count == 0);
            Assert.IsFalse(subscribers.Subscribers.ContainsKey("SubId1"));

            // Remove tests
            exp.DataFeedIdentifier.SubscribtionId = "non exists";
            exp.DataFeedIdentifier.ClientCtx = "non exists";
            Assert.IsNull(subscribers.RemoveSubscriber(exp));
            Assert.IsTrue(subscribers.Subscribers.Count == 0);
        }


        ///// <summary>
        ///// Update
        ///// </summary>
        //[TestMethod]
        //public void TestPinkoExpressionSubscribersAdd()
        //{
        //    var subscribers = new PinkoExpressionSubscribers<double[]>();

        //    // 1st one - new
        //    var exp = SampleMockData.GetPinkoSubscription<double[]>()[0];
        //    exp.PinkoExpression.DataFeedIdentifier.SubscribtionId = "SubId1";
        //    exp.PinkoExpression.DataFeedIdentifier.ClientCtx = "ClientCtx1";

        //    Assert.IsFalse(subscribers.UpdateSubscriber(exp));
        //    Assert.IsTrue(subscribers.Subscribers.Count == 1);
        //    Assert.IsTrue(subscribers.Subscribers[exp.PinkoExpression.DataFeedIdentifier.SubscribtionId].Count == 1);
        //    Assert.IsNotNull(subscribers.Subscribers[exp.PinkoExpression.DataFeedIdentifier.SubscribtionId][exp.PinkoExpression.DataFeedIdentifier.ClientCtx]);
        //    Assert.IsTrue(subscribers.Subscribers[exp.PinkoExpression.DataFeedIdentifier.SubscribtionId][exp.PinkoExpression.DataFeedIdentifier.ClientCtx].PinkoExpression.DataFeedIdentifier.ClientCtx.Equals(exp.PinkoExpression.DataFeedIdentifier.ClientCtx));

        //    // 2nd one - Update
        //    exp = SampleMockData.GetPinkoSubscription<double[]>()[0];
        //    exp.PinkoExpression.DataFeedIdentifier.SubscribtionId = "SubId1";
        //    exp.PinkoExpression.DataFeedIdentifier.ClientCtx = "ClientCtx1";
        //    exp.PinkoExpression.ExpressionFormulasStr = "2nd expression";

        //    Assert.IsTrue(subscribers.UpdateSubscriber(exp));
        //    Assert.IsTrue(subscribers.Subscribers.Count == 1);
        //    Assert.IsTrue(subscribers.Subscribers[exp.PinkoExpression.DataFeedIdentifier.SubscribtionId].Count == 1);
        //    Assert.IsNotNull(subscribers.Subscribers[exp.PinkoExpression.DataFeedIdentifier.SubscribtionId][exp.PinkoExpression.DataFeedIdentifier.ClientCtx]);
        //    Assert.IsTrue(subscribers.Subscribers[exp.PinkoExpression.DataFeedIdentifier.SubscribtionId][exp.PinkoExpression.DataFeedIdentifier.ClientCtx].PinkoExpression.DataFeedIdentifier.ClientCtx.Equals(exp.PinkoExpression.DataFeedIdentifier.ClientCtx));
        //    Assert.IsTrue(subscribers.Subscribers[exp.PinkoExpression.DataFeedIdentifier.SubscribtionId][exp.PinkoExpression.DataFeedIdentifier.ClientCtx].PinkoExpression.ExpressionFormulasStr.Equals(exp.PinkoExpression.ExpressionFormulasStr));

        //    // Add  second one
        //    exp = SampleMockData.GetPinkoSubscription<double[]>()[0];
        //    exp.PinkoExpression.DataFeedIdentifier.SubscribtionId = "SubId1";
        //    exp.PinkoExpression.DataFeedIdentifier.ClientCtx = "ClientCtx2";
        //    exp.PinkoExpression.ExpressionFormulasStr = "3rd expression";

        //    Assert.IsFalse(subscribers.UpdateSubscriber(exp));
        //    Assert.IsTrue(subscribers.Subscribers.Count == 1);
        //    Assert.IsTrue(subscribers.Subscribers[exp.PinkoExpression.DataFeedIdentifier.SubscribtionId].Count == 2);
        //    Assert.IsNotNull(subscribers.Subscribers[exp.PinkoExpression.DataFeedIdentifier.SubscribtionId][exp.PinkoExpression.DataFeedIdentifier.ClientCtx]);
        //    Assert.IsTrue(subscribers.Subscribers[exp.PinkoExpression.DataFeedIdentifier.SubscribtionId][exp.PinkoExpression.DataFeedIdentifier.ClientCtx].PinkoExpression.DataFeedIdentifier.ClientCtx.Equals(exp.PinkoExpression.DataFeedIdentifier.ClientCtx));
        //    Assert.IsTrue(subscribers.Subscribers[exp.PinkoExpression.DataFeedIdentifier.SubscribtionId][exp.PinkoExpression.DataFeedIdentifier.ClientCtx].PinkoExpression.ExpressionFormulasStr.Equals(exp.PinkoExpression.ExpressionFormulasStr));
        //}
    }
}
