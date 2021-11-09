using System;
using Xunit;
using RestSharp;
using ParkingService;

namespace ParkingServiceTest
{
    public class ServiceTest
    {
        [Fact]
        public void Test()
        {
            Service service = new Service(false);
            IRestResponse response = service.Serve();
            Assert.NotNull(response.StatusCode);
        }

        // Add this to test that the docker image build fails when tests fail
        // [Fact]
        // public void FakeTest()
        // {
        //     Assert.True(false);
        // }
    }
}
