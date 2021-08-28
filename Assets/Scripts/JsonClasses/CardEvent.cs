using System;

namespace JsonClasses {

    [Serializable]
    public class CardEvent {

        public int id;
        public int type;
        public string cardsPlayerOne;
        public string cardsPlayerTwo;

        public CardEvent() {
            id = 0;
            type = 0;

            cardsPlayerOne = "9999:0:0x9999:0:0x9999:0:0";
            cardsPlayerTwo = "9999:0:0x9999:0:0x9999:0:0";
        }
    }
}