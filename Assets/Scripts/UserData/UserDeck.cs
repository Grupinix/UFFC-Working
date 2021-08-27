using System.Collections.Generic;
using Default;

namespace UserData {
    public static class UserDeck {
        private static readonly List<CardStatus> CardsAlmostMap = new List<CardStatus> {
            new CardStatus(true, new []{1, 0, 0}, 0, 0, 0, 4),
            new CardStatus(true, new []{1, 0, 0}, 0, 0, 0, 4),
            new CardStatus(true, new []{1, 0, 0}, 0, 0, 0, 4),
            new CardStatus(true, new []{0, 1, 0}, 0, 0, 0, 4),
            new CardStatus(true, new []{0, 1, 0}, 0, 0, 0, 4),
            new CardStatus(true, new []{0, 1, 0}, 0, 0, 0, 4),
            new CardStatus(true, new []{0, 0, 1}, 0, 0, 0, 4),
            new CardStatus(true, new []{0, 0, 1}, 0, 0, 0, 4),
            new CardStatus(true, new []{0, 0, 1}, 0, 0, 0, 4)
        };


        public static CardStatus getCardStatus(int id) {
            return CardsAlmostMap[id];
        }

        public static int getAmountOfCards() {
            return CardsAlmostMap.Count;
        }
    }
}