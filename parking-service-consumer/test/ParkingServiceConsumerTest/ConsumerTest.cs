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
            Assert.NotNull(response.StatusCode);
        }

        // Add this to test that the docker image build fails when tests fail
        [Fact]
        public void FakeTest()
        {
            Assert.True(false);
        }
    }
}
