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
        /// Update partial identifier
        /// </summary>
        [TestMethod]
        public void TestPinkoExpressionSubscribersUpdatePartialIdentifier()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var subscribers = pinkoContainer.Resolve<PinkoExpressionSubscribers<double[]>>();

            // 1st one - new
            var origExp = SampleMockData.GetPinkoMsgCalculateExpressionResult(1)[0];
            subscribers.UpdateSubscriber(origExp, () => null);

            // Assure it was added
            Assert.IsTrue(subscribers.Subscribers.Count == 1);
            Assert.IsTrue(subscribers.Subscribers[origExp.DataFeedIdentifier.SubscribtionId].Subcribers.Count == 1);

            // Create new  one with diff connecting ids
            var expSecond = SampleMockData.GetPinkoDataFeedIdentifier(2)[1];

            // Assure they are not the same
            Assert.IsFalse(subscribers.Subscribers[origExp.DataFeedIdentifier.SubscribtionId].Subcribers[0].Item2.DataFeedIdentifier.IsEqual(expSecond));
            Assert.IsTrue(subscribers.Subscribers[origExp.DataFeedIdentifier.SubscribtionId].Subcribers[0].Item2.DataFeedIdentifier.IsEqual(origExp.DataFeedIdentifier));
            
            // Force update to these keys
            expSecond.SubscribtionId = origExp.DataFeedIdentifier.SubscribtionId;
            expSecond.ClientCtx = origExp.DataFeedIdentifier.ClientCtx;
            expSecond.ClientId = origExp.DataFeedIdentifier.ClientId;

            // Update partial
            subscribers.UpdateIdentifier(expSecond);

            // Assure members did not change
            Assert.IsTrue(subscribers.Subscribers.Count == 1);
            Assert.IsTrue(subscribers.Subscribers[origExp.DataFeedIdentifier.SubscribtionId].Subcribers.Count == 1);

            var subscribedIdentifier = subscribers.Subscribers[origExp.DataFeedIdentifier.SubscribtionId].Subcribers[0].Item2.DataFeedIdentifier;
            
            // Assure the mixed update happened
            Assert.IsTrue(subscribedIdentifier.IsEqual(expSecond));
            Assert.IsFalse(subscribedIdentifier.IsEqual(origExp.DataFeedIdentifier));

            // Assure all have values
            Assert.IsFalse(string.IsNullOrEmpty(subscribedIdentifier.SubscribtionId));
            Assert.IsFalse(string.IsNullOrEmpty(subscribedIdentifier.ClientCtx));
            Assert.IsFalse(string.IsNullOrEmpty(subscribedIdentifier.ClientId));
            Assert.IsFalse(string.IsNullOrEmpty(subscribedIdentifier.PreviousSignalRId));
            Assert.IsFalse(string.IsNullOrEmpty(subscribedIdentifier.PreviousWebRoleId));
            Assert.IsFalse(string.IsNullOrEmpty(subscribedIdentifier.SignalRId));
            Assert.IsFalse(string.IsNullOrEmpty(subscribedIdentifier.WebRoleId));
        }

        /// <summary>
        /// REmove Identifier subscription
        /// </summary>
        [TestMethod]
        public void TestPinkoExpressionSubscribersRemoveIdentifier()
        {
            var pinkoContainer = PinkoContainerMock.GetMockContainer();
            var subscribers = pinkoContainer.Resolve<PinkoExpressionSubscribers<double[]>>();

            // 1st one - new
            var exp = SampleMockData.GetPinkoMsgCalculateExpressionResult()[0];
            subscribers.UpdateSubscriber(exp, () => null);

            Assert.IsTrue(subscribers.Subscribers.Count == 1);
            Assert.IsTrue(subscribers.Subscribers[exp.DataFeedIdentifier.SubscribtionId].Subcribers.Count == 1);

            // remove identifier
            subscribers.RemoveIdentifier(SampleMockData.GetPinkoMsgCalculateExpressionResult()[0].DataFeedIdentifier);

            Assert.IsTrue(subscribers.Subscribers.Count == 0);
        }


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
            subscribers.UpdateSubscriber(exp, () => null);

            exp = exp = SampleMockData.GetPinkoMsgCalculateExpressionResult()[0];
            exp.DataFeedIdentifier.SubscribtionId = "SubId1";
            exp.DataFeedIdentifier.ClientCtx = "ClientCtx1";
            subscribers.UpdateSubscriber(exp, () => null);

            // Add  second one
            exp = exp = SampleMockData.GetPinkoMsgCalculateExpressionResult()[0];
            exp.DataFeedIdentifier.SubscribtionId = "SubId1";
            exp.DataFeedIdentifier.ClientCtx = "ClientCtx2";
            subscribers.UpdateSubscriber(exp, () => null);

            // Add  second one
            exp = exp = SampleMockData.GetPinkoMsgCalculateExpressionResult()[0];
            exp.DataFeedIdentifier.SubscribtionId = "SubId3";
            exp.DataFeedIdentifier.ClientCtx = "ClientCtx3";
            subscribers.UpdateSubscriber(exp, () => null);

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
    }
}
