using System.Collections.Generic;
using Default;
using UnityEngine;

namespace UserData {
    public static class UserDeck {

        private static readonly List<CardStatus> CardsAlmostMap = new List<CardStatus> {
            new CardStatus(true, new[] {1, 0, 0}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, new[] {1, 0, 0}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, new[] {1, 0, 0}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, new[] {0, 1, 0}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, new[] {0, 1, 0}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, new[] {0, 1, 0}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, new[] {0, 0, 1}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, new[] {0, 0, 1}, new[] {0, 0, 0, 0}, 0, 0, 4),
            new CardStatus(true, new[] {0, 0, 1}, new[] {0, 0, 0, 0}, 0, 0, 4)
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