using System.Collections.Generic;
using Default;
using UnityEngine;

namespace UserData {
    public static class UserDeck {

        private static readonly List<CardStatus> CardsAlmostMap = new List<CardStatus> {
            new CardStatus(true, false, new[] {1, 0, 0}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, false, new[] {1, 0, 0}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, false, new[] {1, 0, 0}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, false, new[] {0, 1, 0}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, false, new[] {0, 1, 0}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, false, new[] {0, 1, 0}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, false, new[] {0, 0, 1}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, false, new[] {0, 0, 1}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, false, new[] {0, 0, 1}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, false, new[] {1, 1, 0}, new[] {0, 0, 0, 0}, 0, 0, 2),
            new CardStatus(true, false, new[] {1, 1, 0}, new[] {0, 0, 0, 0}, 0, 0, 2),
            new CardStatus(true, false, new[] {1, 1, 1}, new[] {0, 0, 0, 0}, 0, 0, 1),
            new CardStatus(true, false, new[] {0, 1, 1}, new[] {0, 0, 0, 0}, 0, 0, 2),
            new CardStatus(false, false, new[] {0, 0, 0}, new[] {0, 0, 0, 2}, 3, 2, 1),
            new CardStatus(false, false, new[] {0, 0, 0}, new[] {0, 0, 0, 1}, 2, 1, 2),
            new CardStatus(false, false, new[] {0, 0, 0}, new[] {0, 0, 0, 2}, 2, 3, 1),
            new CardStatus(false, false, new[] {0, 0, 0}, new[] {1, 0, 0, 1}, 3, 3, 1),
            new CardStatus(false, false, new[] {0, 0, 0}, new[] {3, 0, 0, 0}, 4, 6, 1),
            new CardStatus(false, false, new[] {0, 0, 0}, new[] {0, 2, 0, 0}, 2, 4, 2),
            new CardStatus(false, false, new[] {0, 0, 0}, new[] {0, 0, 0, 3}, 0, 8, 1),
            new CardStatus(false, false, new[] {0, 0, 0}, new[] {0, 2, 0, 3}, 9, 5, 1),
            new CardStatus(false, false, new[] {0, 0, 0}, new[] {2, 0, 0, 2}, 7, 5, 1),
        };

        public static readonly List<int> DeckCardIds = new List<int>();

        static UserDeck() {
            for (int i = 0; i < getAmountOfCards(); i++) {
                int value = PlayerPrefs.GetInt($"card_{i}", 1);
                DeckCardIds.Add(value);
            }
        }

        public static CardStatus getCardStatus(int id) {
            return CardsAlmostMap[id];
        }

        public static int getAmountOfCards() {
            return CardsAlmostMap.Count;
        }
    }
}