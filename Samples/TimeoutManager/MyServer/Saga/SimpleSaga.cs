﻿namespace MyServer.Saga
{
    using System;
    using NServiceBus.Saga;

    public class SimpleSaga:Saga<SimpleSagaData>,
        IAmStartedByMessages<StartSagaMessage>,
        IHandleTimeouts<MyTimeOutState>
    {
        public void Handle(StartSagaMessage message)
        {
            Data.OrderId = message.OrderId;
            var someState = new Random().Next(10);

            RequestUtcTimeout(TimeSpan.FromSeconds(10), someState);
            LogMessage("v2.6 Timeout (10s) requested with state: " + someState);
        }

        public override void Timeout(object state)
        {
            LogMessage("v2.6 Timeout fired, with state: " + state);
            LogMessage("Requesting a custom timeout v3.0 style");
            RequestTimeout(TimeSpan.FromSeconds(10), new MyTimeOutState
                                                        {
                                                            SomeValue = "Custom state"
                                                        });
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<StartSagaMessage>(s => s.OrderId, m => m.OrderId);
        }

        void LogMessage(string message)
        {
            Console.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToLongTimeString(),message));
        }

        public void Handle(MyTimeOutState state)
        {
            LogMessage("v3.0 Timeout fired, with state: " + state.SomeValue);
            MarkAsComplete();
        }
    }
}