using System;
using Xunit;
using RestSharp;
using ParkingServiceConsumer;

namespace ParkingServiceConsumerTest
{
    public class ConsumerTest
    {
        [Fact]
        public void ConsumeTest()
        {
            IRestResponse response = Consumer.Consume();
            //FIXME: Shouldn't assert the status code, the API may fail
            Assert.Equal(200, (int)response.StatusCode);
        }
    }
}
