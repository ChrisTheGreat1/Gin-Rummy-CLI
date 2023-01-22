using _11242022_Gin_Rummy.Enums;
using _11242022_Gin_Rummy.Helpers;
using _11242022_Gin_Rummy.Models;

namespace GinRummyTests
{
    public class HandMethodsTests
    {
        private static readonly List<Card> HandWithGin = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Ace},
            new Card() { Suit = Suit.Spades, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Spades, Rank = Rank.Three},
            new Card() { Suit = Suit.Spades, Rank = Rank.Four},
            new Card() { Suit = Suit.Spades, Rank = Rank.Five},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Ace},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Three},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Four},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Five},
        };

        private static readonly List<Card> HandWithSingleRunMeld = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Jack},
            new Card() { Suit = Suit.Spades, Rank = Rank.Queen},
            new Card() { Suit = Suit.Spades, Rank = Rank.King},
            new Card() { Suit = Suit.Spades, Rank = Rank.Seven},
            new Card() { Suit = Suit.Spades, Rank = Rank.Nine},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Four},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Six},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Three},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Seven},
        };

        private static readonly List<Card> HandWithSingleSameRankMeld = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Ace},
            new Card() { Suit = Suit.Spades, Rank = Rank.Queen},
            new Card() { Suit = Suit.Spades, Rank = Rank.King},
            new Card() { Suit = Suit.Spades, Rank = Rank.Seven},
            new Card() { Suit = Suit.Spades, Rank = Rank.Nine},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Four},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Nine},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Three},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Nine},
        };

        private static readonly List<Card> HandWithoutGinOrAnyMeldsOrFaceCards = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Ace},
            new Card() { Suit = Suit.Spades, Rank = Rank.Three},
            new Card() { Suit = Suit.Spades, Rank = Rank.Five},
            new Card() { Suit = Suit.Spades, Rank = Rank.Seven},
            new Card() { Suit = Suit.Spades, Rank = Rank.Nine},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Four},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Six},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Three},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Seven},
        };

        private static readonly List<Card> HandWithoutGinOrAnyMelds_ButWith3FaceCards = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Ace},
            new Card() { Suit = Suit.Spades, Rank = Rank.Three},
            new Card() { Suit = Suit.Spades, Rank = Rank.Five},
            new Card() { Suit = Suit.Spades, Rank = Rank.Jack},
            new Card() { Suit = Suit.Spades, Rank = Rank.King},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Four},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Six},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Eight},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Queen},
        };

        private static readonly List<Card> HandWith_2ThreeOfAKind_And_1RunOfThree_ButNoGin = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Ace},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Ace},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Ace},
            new Card() { Suit = Suit.Spades, Rank = Rank.King},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.King},
            new Card() { Suit = Suit.Hearts, Rank = Rank.King},
            new Card() { Suit = Suit.Spades, Rank = Rank.Four},
            new Card() { Suit = Suit.Spades, Rank = Rank.Five},
            new Card() { Suit = Suit.Spades, Rank = Rank.Six},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Ten},
        };

        private static readonly List<Card> HandWith_2ThreeOfAKind_And_1RunOfThree_ButNoGin_NonOverlappingSuits = new List<Card>()
        {
            new Card() { Suit = Suit.Clubs, Rank = Rank.Ace},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Ace},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Ace},
            new Card() { Suit = Suit.Spades, Rank = Rank.King},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.King},
            new Card() { Suit = Suit.Hearts, Rank = Rank.King},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Four},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Five},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Six},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Ten},
        };

        private static readonly List<Card> HandWith_2ThreeOfAKind_ButNoGin = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Ace},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Ace},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Ace},
            new Card() { Suit = Suit.Spades, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Spades, Rank = Rank.Four},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Five},
            new Card() { Suit = Suit.Spades, Rank = Rank.Six},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Ten},
        };

        private static readonly List<Card> HandWith_1ThreeOfAKind_And_1RunOfThree_ButNoGin = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Ace},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Ace},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Ace},
            new Card() { Suit = Suit.Spades, Rank = Rank.King},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Three},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Jack},
            new Card() { Suit = Suit.Spades, Rank = Rank.Four},
            new Card() { Suit = Suit.Spades, Rank = Rank.Five},
            new Card() { Suit = Suit.Spades, Rank = Rank.Six},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Seven},
        };

        private static readonly List<Card> HandWith_KnockingCapability_AndInterferingMelds = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Ten},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Ten},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Ten},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Eight},
            new Card() { Suit = Suit.Spades, Rank = Rank.Eight},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Eight},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Three},
        };

        private static readonly List<Card> HandWith_NonKnockingCapability_AndInterferingMelds = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Ace},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Ace},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Ace},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Four},
            new Card() { Suit = Suit.Spades, Rank = Rank.Four},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Four},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Nine},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Nine},
            new Card() { Suit = Suit.Spades, Rank = Rank.King},
            new Card() { Suit = Suit.Clubs, Rank = Rank.King},
        };

        private static readonly List<Card> HandWith_NonKnockingCapability_AndKnocking4ofKindDiscard = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Ace},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Ace},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Ace},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Four},
            new Card() { Suit = Suit.Spades, Rank = Rank.Four},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Four},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Nine},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Nine},
            new Card() { Suit = Suit.Spades, Rank = Rank.Ten},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Eight},
        };

        private static readonly List<Card> HandWith_NonKnockingCapability_AndKnockingRunDiscard = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Ace},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Ace},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Ace},
            new Card() { Suit = Suit.Clubs, Rank = Rank.King},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.King},
            new Card() { Suit = Suit.Hearts, Rank = Rank.King},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Three},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Five},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Queen},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Eight},
        };

        private static readonly List<Card> HandWith_2Runs_AndKnockingCapability = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Six},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Seven},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Eight},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Nine},
            new Card() { Suit = Suit.Spades, Rank = Rank.Four},
            new Card() { Suit = Suit.Spades, Rank = Rank.Five},
            new Card() { Suit = Suit.Spades, Rank = Rank.Six},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Three},
        };

        private static readonly List<Card> HandWith_RunOf5_AndKnockingCapability = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Six},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Seven},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Eight},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Nine},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Ten},
            new Card() { Suit = Suit.Spades, Rank = Rank.Queen},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Queen},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Queen},
        };

        private static readonly List<Card> HandWith_NonKnockingCapability_AndRunOf5Discard_And4ofAKindDiscard = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Ace},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Ace},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Ace},
            new Card() { Suit = Suit.Clubs, Rank = Rank.King},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.King},
            new Card() { Suit = Suit.Hearts, Rank = Rank.King},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Three},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Five},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Queen},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Four},
        };

        private static readonly List<Card> HandThatShouldHaveGroupingsOf3_EdgeCase_1 = new List<Card>()
        {
            // Should have hand value of 6 (only 6h not in meld)
            new Card() { Suit = Suit.Spades, Rank = Rank.Four},
            new Card() { Suit = Suit.Spades, Rank = Rank.Five},
            new Card() { Suit = Suit.Spades, Rank = Rank.Six},
            new Card() { Suit = Suit.Spades, Rank = Rank.Seven},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Six},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Seven},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Eight},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Six},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Seven},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Seven},
        };

        private static readonly List<Card> HandThatShouldHaveGroupingsOf3_EdgeCase_2 = new List<Card>()
        {
            // Should have hand value of 1 (only Ac not in meld)
            new Card() { Suit = Suit.Clubs, Rank = Rank.Ace},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Three},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Queen},
            new Card() { Suit = Suit.Spades, Rank = Rank.Queen},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Queen},
            new Card() { Suit = Suit.Spades, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Spades, Rank = Rank.Three},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Three},
        };

        private static readonly List<Card> HandOfAllSameSuit_EdgeCase = new List<Card>()
        {
            new Card() { Suit = Suit.Clubs, Rank = Rank.Ace},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Three},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Four},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Five},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Six},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Seven},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Nine},
            new Card() { Suit = Suit.Clubs, Rank = Rank.Ten},
            new Card() { Suit = Suit.Clubs, Rank = Rank.King},
        };

        private static readonly List<Card> HandWith_2RunsOfSameSuit_EdgeCase = new List<Card>()
        {
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Ten},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Jack},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Queen},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.King},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Ten},
            new Card() { Suit = Suit.Spades, Rank = Rank.Ten},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Ace},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Five},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Six},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Seven},
        };

        private static readonly List<Card> Hand_SimpleAgent_Bug_1 = new List<Card>()
        {
            new Card() { Suit = Suit.Spades, Rank = Rank.Ace},
            new Card() { Suit = Suit.Spades, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Spades, Rank = Rank.Three},
            new Card() { Suit = Suit.Spades, Rank = Rank.Four},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Seven},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Eight},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Nine},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Ace},
            new Card() { Suit = Suit.Diamonds, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Deuce},
            new Card() { Suit = Suit.Hearts, Rank = Rank.Ace}
        };

        [Fact]
        public void SimpleAIAgentHandShouldOnlyHave10CardsInGin()
        {
            var hand1 = Hand_SimpleAgent_Bug_1;

            hand1 = HandMethods.DetermineMeldsInHand(hand1);

            int numOfCardsInMeld = 0;

            foreach (var card in hand1)
            {
                if (card.IsInMeld)
                {
                    numOfCardsInMeld++;
                }
            }

            numOfCardsInMeld.Should().Be(9);
        }

        [Fact]
        public void CardsShouldBeEqual()
        {
            var hand1 = HandWithGin;

            var card1 = HandWithGin.Last();
            var card2 = HandWithGin.Last();

            card1.Should().BeSameAs(card2);
        }

        [Fact]
        public void DetermineBestHandMeldCombination_MethodShouldReturnHandWithLowestPossibleValue()
        {
            var hand1 = HandMethods.DetermineMeldsInHand(HandOfAllSameSuit_EdgeCase);
            HandMethods.CalculateHandValue(hand1).Should().Be(29);

            var hand2 = HandMethods.DetermineMeldsInHand(HandThatShouldHaveGroupingsOf3_EdgeCase_1);
            HandMethods.CalculateHandValue(hand2).Should().Be(6);

            var hand3 = HandMethods.DetermineMeldsInHand(HandThatShouldHaveGroupingsOf3_EdgeCase_2);
            HandMethods.CalculateHandValue(hand3).Should().Be(1);

            var hand4 = HandMethods.DetermineMeldsInHand(HandWithoutGinOrAnyMeldsOrFaceCards);
            HandMethods.CalculateHandValue(hand4).Should().Be(47);

            var hand5 = HandMethods.DetermineMeldsInHand(HandWithSingleRunMeld);
            HandMethods.CalculateHandValue(hand5).Should().Be(38);

            var hand6 = HandMethods.DetermineMeldsInHand(HandWithSingleSameRankMeld);
            HandMethods.CalculateHandValue(hand6).Should().Be(37);
        }

        [Fact]
        public void TestKnockingHandWithInterferingMelds()
        {
            var handKnocker = HandWith_KnockingCapability_AndInterferingMelds;
            var handNonKnocker = HandWith_NonKnockingCapability_AndInterferingMelds;

            handKnocker = HandMethods.DetermineMeldsInHand(handKnocker);
            handNonKnocker = HandMethods.DetermineMeldsInHand(handNonKnocker);

            handNonKnocker = HandMethods.NonKnockerCombinesUnmatchedCardsWithKnockersMelds(handKnocker, handNonKnocker);

            int numOfCardsInMeld = 0;

            foreach (var card in handNonKnocker)
            {
                if (card.IsInMeld)
                {
                    numOfCardsInMeld++;
                }
            }

            numOfCardsInMeld.Should().Be(6);

            numOfCardsInMeld = 0;

            foreach (var card in handKnocker)
            {
                if (card.IsInMeld)
                {
                    numOfCardsInMeld++;
                }
            }

            numOfCardsInMeld.Should().Be(9);
        }

        [Fact]
        public void TestKnockingHandWithInterferingMelds2()
        {
            var handKnocker = HandWith_KnockingCapability_AndInterferingMelds;
            var handNonKnocker = HandWith_NonKnockingCapability_AndKnocking4ofKindDiscard;

            handKnocker = HandMethods.DetermineMeldsInHand(handKnocker);
            handNonKnocker = HandMethods.DetermineMeldsInHand(handNonKnocker);

            handNonKnocker = HandMethods.NonKnockerCombinesUnmatchedCardsWithKnockersMelds(handKnocker, handNonKnocker);

            int numOfCardsInMeld = 0;

            foreach (var card in handNonKnocker)
            {
                if (card.IsInMeld)
                {
                    numOfCardsInMeld++;
                }
            }

            numOfCardsInMeld.Should().Be(8);

            numOfCardsInMeld = 0;

            foreach (var card in handKnocker)
            {
                if (card.IsInMeld)
                {
                    numOfCardsInMeld++;
                }
            }

            numOfCardsInMeld.Should().Be(9);
        }

        [Fact]
        public void TestKnockingHandWithInterferingMelds3()
        {
            var handKnocker = HandWith_2Runs_AndKnockingCapability;
            var handNonKnocker = HandWith_NonKnockingCapability_AndKnockingRunDiscard;

            handKnocker = HandMethods.DetermineMeldsInHand(handKnocker);
            handNonKnocker = HandMethods.DetermineMeldsInHand(handNonKnocker);

            handNonKnocker = HandMethods.NonKnockerCombinesUnmatchedCardsWithKnockersMelds(handKnocker, handNonKnocker);

            int numOfCardsInMeld = 0;

            foreach (var card in handNonKnocker)
            {
                if (card.IsInMeld)
                {
                    numOfCardsInMeld++;
                }
            }

            numOfCardsInMeld.Should().Be(7);

            numOfCardsInMeld = 0;

            foreach (var card in handKnocker)
            {
                if (card.IsInMeld)
                {
                    numOfCardsInMeld++;
                }
            }

            numOfCardsInMeld.Should().Be(7);
        }

        [Fact]
        public void TestKnockingHandWithInterferingMelds4()
        {
            var handKnocker = HandWith_RunOf5_AndKnockingCapability;
            var handNonKnocker = HandWith_NonKnockingCapability_AndRunOf5Discard_And4ofAKindDiscard;

            handKnocker = HandMethods.DetermineMeldsInHand(handKnocker);
            handNonKnocker = HandMethods.DetermineMeldsInHand(handNonKnocker);

            handNonKnocker = HandMethods.NonKnockerCombinesUnmatchedCardsWithKnockersMelds(handKnocker, handNonKnocker);

            int numOfCardsInMeld = 0;

            foreach (var card in handNonKnocker)
            {
                if (card.IsInMeld)
                {
                    numOfCardsInMeld++;
                }
            }

            numOfCardsInMeld.Should().Be(8);

            numOfCardsInMeld = 0;

            foreach (var card in handKnocker)
            {
                if (card.IsInMeld)
                {
                    numOfCardsInMeld++;
                }
            }

            numOfCardsInMeld.Should().Be(8);
        }

        [Fact]
        public void TestKnockingHandDiscardMethod()
        {
            var handKnocker = HandWith_2ThreeOfAKind_And_1RunOfThree_ButNoGin_NonOverlappingSuits;
            var handNonKnocker = HandWithoutGinOrAnyMeldsOrFaceCards;

            handKnocker = HandMethods.DetermineMeldsInHand(handKnocker);
            handNonKnocker = HandMethods.DetermineMeldsInHand(handNonKnocker);

            handNonKnocker = HandMethods.NonKnockerCombinesUnmatchedCardsWithKnockersMelds(handKnocker, handNonKnocker);

            int numOfCardsInMeld = 0;

            foreach (var card in handNonKnocker)
            {
                if (card.IsInMeld)
                {
                    numOfCardsInMeld++;
                }
            }

            numOfCardsInMeld.Should().Be(3);
        }

        [Fact]
        public void PlayerShouldBeAbleToKnock()
        {
            var hand = HandWith_2ThreeOfAKind_And_1RunOfThree_ButNoGin;

            hand = HandMethods.DetermineMeldsInHand(hand);

            HandMethods.CanPlayerKnock(hand).Should().BeTrue();
        }

        [Fact]
        public void PlayerShouldNotBeAbleToKnock()
        {
            var hand = HandWithoutGinOrAnyMeldsOrFaceCards;

            hand = HandMethods.DetermineMeldsInHand(hand);

            HandMethods.CanPlayerKnock(hand).Should().BeFalse();
        }

        [Fact]
        public void HandValueShouldBe0()
        {
            var hand = HandWithGin;

            hand = HandMethods.DetermineMeldsInHand(hand);

            HandMethods.CalculateHandValue(hand).Should().Be(0);
        }

        [Fact]
        public void HandValueShouldBe1()
        {
            var hand = HandWith_2RunsOfSameSuit_EdgeCase;

            hand = HandMethods.DetermineMeldsInHand(hand);

            HandMethods.CalculateHandValue(hand).Should().Be(1);

            HandMethods.CanPlayerKnock(hand).Should().BeTrue();

            HandMethods.DetectGin(hand).Should().BeFalse();
        }

        [Fact]
        public void HandValueShouldBe10()
        {
            var hand = HandWith_2ThreeOfAKind_And_1RunOfThree_ButNoGin;

            hand = HandMethods.DetermineMeldsInHand(hand);

            HandMethods.CalculateHandValue(hand).Should().Be(10);
        }

        [Fact]
        public void HandValueShouldBe59()
        {
            var hand = HandWithoutGinOrAnyMelds_ButWith3FaceCards;

            hand = HandMethods.DetermineMeldsInHand(hand);

            HandMethods.CalculateHandValue(hand).Should().Be(59);
        }

        [Fact]
        public void HandShouldHaveGin()
        {
            var handWithGin = HandWithGin;

            handWithGin = HandMethods.DetermineMeldsInHand(handWithGin);

            bool handHasGin = HandMethods.DetectGin(handWithGin);

            handHasGin.Should().BeTrue();
        }

        [Fact]
        public void HandShouldNotHaveGin()
        {
            var handWithoutGin = HandWithoutGinOrAnyMeldsOrFaceCards;

            handWithoutGin = HandMethods.DetermineMeldsInHand(handWithoutGin);

            bool handHasGin = HandMethods.DetectGin(handWithoutGin);

            handHasGin.Should().BeFalse();
        }

        [Fact]
        public void AllCardsInHandExceptOneShouldBeInMeld()
        {
            var hand = HandWith_2ThreeOfAKind_And_1RunOfThree_ButNoGin;

            hand = HandMethods.DetermineMeldsInHand(hand);

            int numOfCardsInMeld = 0;

            foreach (var card in hand)
            {
                if (card.IsInMeld)
                {
                    numOfCardsInMeld++;
                }
            }

            numOfCardsInMeld.Should().Be(9);
        }
    }
}