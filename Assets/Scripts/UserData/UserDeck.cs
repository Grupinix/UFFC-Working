using System.Collections.Generic;
using Default;
using UnityEngine;

namespace UserData {
    
    /**
     * Classe responsável por definir as
     * cartas existentes em jogo
     */
    public static class UserDeck {

        /** Lista de cartas existentes */
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
            new CardStatus(false, false, new[] {0, 0, 0}, new[] {0, 0, 0, 1}, 1, 2, 4),
            new CardStatus(false, false, new[] {0, 0, 0}, new[] {0, 0, 1, 0}, 3, 1, 2),
            new CardStatus(false, false, new[] {0, 0, 0}, new[] {0, 0, 2, 0}, 5, 4, 1),
        };

        public static readonly List<int> DeckCardIds = new List<int>();

        static UserDeck() {
            for (int i = 0; i < getAmountOfCards(); i++) {
                int value = PlayerPrefs.GetInt($"card_{i}", 1);
                DeckCardIds.Add(value);
            }
        }

        /**
         * Retorna uma lista contendo
         * as cartas do baralho de um
         * usuário
         */
        public static List<int> getDeckCards() {
            List<int> userDeck = new List<int>();

            for (int i = 0; i < getAmountOfCards(); i++) {
                for (int j = 0; j < DeckCardIds[i]; j++) {
                    userDeck.Add(i);
                }
            }

            return userDeck;
        }

        /**
         * Retorna o CardStatus de uma
         * carta com o id informado
         */
        public static CardStatus getCardStatus(int id) {
            return CardsAlmostMap[id];
        }

        /**
         * Retorna a quantidade de
         * cartas existentes
         */
        public static int getAmountOfCards() {
            return CardsAlmostMap.Count;
        }
    }
}