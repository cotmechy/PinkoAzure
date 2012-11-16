using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Practices.Unity;
using PinkoCommon.Extension;
using PinkoCommon.Interface;
using PinkoCommon.Utility;

namespace PinkoCommon.BaseMessageHandlers
{
    /// <summary>
    /// Handler Ping message response
    /// </summary>
    public class InboundMessageReactiveListener<T>
    {
        /// <summary>
        /// Constructor - InboundMessageHandler 
        /// </summary>
        public InboundMessageReactiveListener()
        {
            Trace.TraceInformation("InboundMessageReactiveListener(): {0} - {1}", GetType(), MsgHandlerId);
        }

        ///// <summary>
        ///// Register Handler with RxMesagebus. Subscribe for incoming messages to process in the Rx Bus 
        ///// </summary>
        //public InboundMessageHandler<T> Register()
        //{
        //    return Register(string.Empty);
        //}

        /// <summary>
        /// Register Handler with RxMesagebus. Subscribe for incoming messages to process in the Rx Bus 
        /// </summary>
        public InboundMessageReactiveListener<T> Register()
        {
            Trace.TraceInformation("Registering message handler: {0} - {1}", GetType(), MsgHandlerId);

            //_webRoleId = webRoleId;
            HandlerPublisher = PinkoApplication.GetBus<Tuple<IBusMessageInbound, T>>();

            // subscribe to messages coming from IBusMessageQueue
            PinkoApplication
                .GetSubscriber<Tuple<IBusMessageInbound, T>>()
                //.Where(FilterIncomingMsg)
                .Do(x => Trace.TraceInformation("{0}: {1} - {2}", GetType(), x.Item1.Verbose(), x.Item2.ToString()))
                .Subscribe(x => HandlerAction(x.Item1, x.Item2));

            ReplyQueue = PinkoApplication.GetBus<IBusMessageOutbound>();

            return this;
        }

        /// <summary>
        /// Handle adhoc request
        /// </summary>
        public void HandlerAction(IBusMessageInbound msg, T expression)
        {
            Subscribers
                .Where(subscriber => (null == subscriber.FilterCallback) || subscriber.FilterCallback(msg, expression))
                .ForEach(subscriber =>
                    {
                        var response = (IBusMessageOutbound)msg;

                        var ex = TryCatch.RunInTry(() =>
                            {
                                TryTimer.RunInTimer(() => expression.ToString(),
                                                    () => response = subscriber.Callback(msg, expression));
                            });


                        if (response.IsNotNull())
                            response.ErrorCode = PinkoErrorCode.Success;

                        // Exception ? 
                        if (null != ex)
                        {
                            response.ErrorCode = PinkoErrorCode.UnexpectedException;
                            response.ErrorDescription = ex.Message;
                            response.ErrorSystem = ex.ToString();
                            //resultMsg.ResultType = PinkoErrorCode.UnexpectedException;
                        }

                        // No response required
                        if (response.IsNull()) return;

                        response.PinkoProperties[PinkoMessagePropTag.MessageHandlerId] = MsgHandlerId;
                        response.PinkoProperties[PinkoMessagePropTag.SenderName] = PinkoApplication.MachineName;

                        if (string.IsNullOrEmpty(response.ReplyTo))
                        {
                            Trace.TraceInformation("Missing ReplyTo. Message processed but not sending a response for: {0}", msg.Verbose());
                        }
                        else
                        {
                            response.QueueName = response.ReplyTo;
                            response.ReplyTo = string.Empty;
                            ReplyQueue.Publish(response);
                        }
                    });
        }


        //Create ahndler subcription object 

        ///// <summary>
        ///// Run()
        ///// <param name="brodcastTopic">Non filtered topic to broadcast to all instance of the worker role</param>
        ///// <param name="directTopic">direct filter topic to specific client</param>
        ///// </summary>
        //public void ConnectToStandardMessageTopics(string brodcastTopic, string directTopic)
        //{
        //    //// Register base handlers
        //    //MessageReceiveHandlers.AddRange(new object[]
        //    //{
        //    //    PinkoContainer.Resolve<BusListenerPinkoPingMessage>().Register(),
        //    //});

        //    PinkoApplication.RunInWorkerThread(PinkoConfiguration.PinkoMessageBusToAllWorkersTopic,
        //        () =>
        //        PinkoContainer
        //            .Resolve<IBusMessageServer>()
        //            .GetTopic(PinkoConfiguration.PinkoMessageBusToAllWorkersTopic)
        //            .Listen()
        //        );

        //    PinkoApplication.RunInWorkerThread(directTopic,
        //        () =>
        //        PinkoContainer
        //            .Resolve<IBusMessageServer>()
        //            .GetTopic(directTopic, _webRoleId)
        //            .Listen()
        //        );

        //    PinkoApplication.RunInWorkerThread(brodcastTopic,
        //        () =>
        //        PinkoContainer
        //            .Resolve<IBusMessageServer>()
        //            .GetTopic(brodcastTopic)
        //            .Listen());
        //}


        ///// <summary>
        ///// MessageHandlers 
        ///// </summary>
        //public List<object> MessageReceiveHandlers
        //{
        //    get { return _messageReceiveHandlers; }
        //}
        //private readonly List<object> _messageReceiveHandlers = new List<object>();


        ///// <summary>
        ///// Connect to topics 
        ///// </summary>
        //public abstract void ConnectToTopics();

        /// <summary>
        /// Handler
        /// </summary>
        //public abstract IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, T expression);
        public List<Handlesubscriber<T>> Subscribers = new List<Handlesubscriber<T>>();

        ///// <summary>
        ///// Derived type can override to filter Rx Where().  All message are included by default.
        ///// </summary>
        //protected virtual bool FilterIncomingMsg(Tuple<IBusMessageInbound, T> msg)
        //{
        //    return true; // include all (default)
        //}
        
        /// <summary>
        /// HandlerId
        /// </summary>
        public readonly string MsgHandlerId = Guid.NewGuid().ToString();

        /// <summary>
        /// IPinkoConfiguration
        /// </summary>
        [Dependency]
        public IPinkoConfiguration PinkoConfiguration { private get; set; }

        /// <summary>
        /// REply queue publisher
        /// </summary>
        protected IRxMemoryBus<IBusMessageOutbound> ReplyQueue;

        /// <summary>
        /// Rx Subscriber for this handler
        /// </summary>
        public IRxMemoryBus<Tuple<IBusMessageInbound, T>> HandlerPublisher;

        ///// <summary>
        ///// role id
        ///// </summary>
        //private string _webRoleId;

        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }

        /// <summary>
        /// IPinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }
    }

    /// <summary>
    /// Messahe hnadler subscriber
    /// </summary>
    public class Handlesubscriber<T>
    {
        public Func<IBusMessageInbound, T, bool> FilterCallback;
        public Func<IBusMessageInbound, T, IBusMessageOutbound> Callback;
    }
}
