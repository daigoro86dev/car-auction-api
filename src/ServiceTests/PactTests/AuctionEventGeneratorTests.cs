using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using PactNet.Verifier;
using Contracts;
using Xunit.Abstractions;
using System.ComponentModel;

namespace ServiceTests.PactTests
{
    public class AuctionEventGeneratorTests : IDisposable
    {
        private readonly PactVerifier _verifier;
        private readonly ITestOutputHelper _output;

        private readonly JsonSerializerSettings _defaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public AuctionEventGeneratorTests(ITestOutputHelper output)
        {
            _verifier = new PactVerifier("Auction Events Producer");
            _output = output;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _verifier.Dispose();
        }

        [Fact]
        [Trait("Category", "PactTests")]
        public void EnsureAuctionEventApiHonoursPactWithConsumer()
        {
            _verifier
                .WithMessages(scenarios =>
                {
                    scenarios.Add("auction deleted event", () => new AuctionDeleted[]
                    {
                        new() { Id = "1bca3539-357a-4111-bc52-57e4ff55ece0"}
                    });
                    scenarios.Add("auction finished event", () => new AuctionFinished[]
                    {
                        new ()
                        {
                            ItemSold = true,
                            AuctionId = "",
                            Winner = "",
                            Seller = "",
                            Amount = 1000
                        }
                    });
                    scenarios.Add("auction created event", () => new AuctionCreated[]
                    {
                        new ()
                        {
                            Id = Guid.Parse("881969a0-ca06-4a9b-b243-8264711f8881"),
                            Status = "Live",
                            ReservePrice = 20000,
                            Seller = "bob",
                            Winner = "jane",
                            SoldAmount = 30000,
                            CurrentHighBid = 30000,
                            CreatedAt = DateTime.Parse("2024-01-24"),
                            AuctionEnd = DateTime.Parse("2024-01-24"),
                            UpdatedAt = DateTime.Parse("2024-01-24"),
                            Make = "Ford",
                            Model = "GT",
                            Color = "White",
                            Mileage = 50000,
                            Year = 2020,
                            ImageUrl = "https://cdn.pixabay.com/photo/2016/05/06/16/32/car-1376190_960_720.jpg"
                        }
                    });
                }, _defaultSettings)
               .WithUriSource(new Uri("http://localhost:9292/pacts/provider/AuctionEventsProducer/consumer/AuctionEventsConsumer/latest"))
               .Verify();

        }
    }
}
