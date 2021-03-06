using System;
using System.Collections.Generic;
using Bot.Builder.Community.Cards.Management;
using Microsoft.Bot.Builder.Adapters;

namespace Bot.Builder.Community.Cards.Tests
{
    public static class TestExtensions
    {
        public static TestFlow Do(this TestFlow testFlow, Action action)
        {
            return new TestFlow(
                async () =>
                {
                    await testFlow.StartTestAsync().ConfigureAwait(false);

                    action?.Invoke();
                },
                testFlow);
        }

        public static Dictionary<string, object> WrapLibraryData(this object innerData)
        {
            return new Dictionary<string, object>
            {
                { PropertyNames.LibraryData, innerData },
            };
        }
    }
}
