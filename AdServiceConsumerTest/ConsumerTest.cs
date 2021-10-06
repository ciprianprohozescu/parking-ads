using System;
using Xunit;
using RestSharp;
using AdServiceConsumer;

namespace AdServiceConsumerTest
{
    public class ConsumerTest
    {
        [Fact]
        public void ConsumeTest()
        {
            IRestResponse response = Consumer.Consume();
            Assert.Equal(200, (int)response.StatusCode);
        }
    }
}
