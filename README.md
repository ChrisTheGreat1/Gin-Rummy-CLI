# 11242022-Gin-Rummy

Command-line game of gin rummy. Three options of play exist:

- Two humans play against each other
- One human plays against a simple AI agent
- Two simple AI agents play against each other

Play option is determined by commenting/uncommenting the relevant code section in [Program.cs](https://github.com/ConkyTheGreat/11242022-Gin-Rummy/blob/master/Program.cs)

The command line always shows both players hands purely for demonstration and verification purposes. If wanting to play a game where you cannot see the opponents
hand, the "WriteLine()" lines of the relevant PrintHandsToConsole() methods should be commented out. 

Everything in this repo was written from scratch by myself. Algorithms are implemented to ensure that the optimal meld combinations are always selected such that
both player's hand values are minimized. 

The simple AI agent operates on this ruleset:
- If the card on the discard pile completes a meld, pick it up. Otherwise pick up from the deck.
- Always discard the card that has the highest value that is not currently in a meld.  
- During the turn immediately after the cards are dealt out, if the discard pile card has a lower value than the highest value card currently in the agent's hand, then pick it up and discard the highest value card.
