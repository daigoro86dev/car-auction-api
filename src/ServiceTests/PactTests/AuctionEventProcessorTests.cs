using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using PactNet;
using PactNet.Output.Xunit;
using Xunit.Abstractions;
using PactNet.Matchers;
using Contracts;
using FluentAssertions;

namespace ServiceTests.PactTests;

public class AuctionEventProcessorTests
{
    private readonly IMessagePactBuilderV3 _messagePact;

    public AuctionEventProcessorTests(ITestOutputHelper output)
    {
        IPactV3 v3 = Pact.V3("AuctionEventsConsumer", "AuctionEventsProducer", new PactConfig
        {
            PactDir = "../../../pacts/",
            DefaultJsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            },
            Outputters = new[]
            {
                new XunitOutput(output)
            }
        });

        _messagePact = v3.WithMessageInteractions();
    }

    [Fact]
    [Trait("Category", "PactTests")]
    public void AuctionDeletedEvent()
    {
        _messagePact
            .ExpectsToReceive("auction deleted event")
            .Given("auction deleted event is pushed into the bus")
            .WithJsonContent(Match.MinType(new
            {
                Id = Match.Type(""),
            }, 1))
            .Verify<ICollection<AuctionDeleted>>(events =>
            {
                events.Should().BeEquivalentTo(new[]
                {
                    new AuctionDeleted
                    {
                        Id = "",
                    }
                });
            });
    }

    [Fact]
    [Trait("Category", "PactTests")]
    public void AuctionFinishedEvent()
    {
        _messagePact
            .ExpectsToReceive("auction finished event")
            .Given("auction finished event is pushed into the bus")
            .WithJsonContent(Match.MinType(new
            {
                ItemSold = Match.Type(true),
                AuctionId = Match.Type(""),
                Winner = Match.Type(""),
                Seller = Match.Type(""),
                Amount = Match.Integer(1000)
            }, 1))
            .Verify<ICollection<AuctionFinished>>(events =>
            {
                events.Should().BeEquivalentTo(new[]
                {
                    new AuctionFinished
                    {
                        ItemSold = true,
                        AuctionId = "",
                        Winner = "",
                        Seller = "",
                        Amount = 1000
                    }
                });
            });
    }

    [Fact]
    [Trait("Category", "PactTests")]
    public void AuctionCreatedEvent()
    {
        _messagePact
            .ExpectsToReceive("auction created event")
            .Given("auction created event is pushed into the bus")
            .WithJsonContent(Match.MinType(new
            {
                Id = Match.Type(Guid.Parse("881969a0-ca06-4a9b-b243-8264711f8881")),
                Status = Match.Type(""),
                ReservePrice = Match.Type(0),
                Seller = Match.Type(""),
                Winner = Match.Type(""),
                SoldAmount = Match.Integer(0),
                CurrentHighBid = Match.Integer(0),
                CreatedAt = Match.Type(DateTime.Parse("2024-01-24")),
                AuctionEnd = Match.Type(DateTime.Parse("2024-01-24")),
                UpdatedAt = Match.Type(DateTime.Parse("2024-01-24")),
                Make = Match.Type(""),
                Model = Match.Type(""),
                Color = Match.Type(""),
                Mileage = Match.Type(0),
                Year = Match.Integer(2024),
                ImageUrl = Match.Type("")
            }, 1))
            .Verify<ICollection<AuctionCreated>>(events =>
            {
                events.Should().BeEquivalentTo(new[]
                {
                    new AuctionCreated
                    {
                        Id = Guid.Parse("881969a0-ca06-4a9b-b243-8264711f8881"),
                        Status = "",
                        ReservePrice = 0,
                        Seller = "",
                        Winner = "",
                        SoldAmount = 0,
                        CurrentHighBid = 0,
                        CreatedAt = DateTime.Parse("2024-01-24"),
                        AuctionEnd = DateTime.Parse("2024-01-24"),
                        UpdatedAt = DateTime.Parse("2024-01-24"),
                        Make = "",
                        Model = "",
                        Color = "",
                        Mileage = 0,
                        Year = 2024,
                        ImageUrl = ""
                    }
                });
            });
    }
}