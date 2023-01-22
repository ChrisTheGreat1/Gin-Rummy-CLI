using _11242022_Gin_Rummy.Helpers;

namespace GinRummyTests
{
    public class DeckMethodsTests
    {
        [Fact]
        public void DeckSizeShouldBe52()
        {
            var deck = DeckMethods.CreateDeck();

            deck.Count.Should().Be(52);
        }

        [Fact]
        public void ShuffledDeckShouldBeDifferentFromOriginalDeck()
        {
            var deckUnshuffled = DeckMethods.CreateDeck();

            var deckShuffled = DeckMethods.CreateDeck();
            DeckMethods.ShuffleDeck(deckShuffled);

            deckShuffled.Should().NotBeSameAs(deckUnshuffled);
        }
    }
}