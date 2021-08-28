using System;

namespace JsonClasses {

    [Serializable]
    public class CardEvent {

        public int id;
        public int type;
        public int[,] cardsPlayerOne;
        public int[,] cardsPlayerTwo;

        public CardEvent() {
            id = 0;
            type = 0;
            cardsPlayerOne = new [,] {
                {9999, 0, 0},
                {9999, 0, 0},
                {9999, 0, 0}
            };
            cardsPlayerTwo = new [,] {
                {9999, 0, 0},
                {9999, 0, 0},
                {9999, 0, 0}
            };
        }
    }
}