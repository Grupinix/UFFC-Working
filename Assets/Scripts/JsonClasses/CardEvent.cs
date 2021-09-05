using System;

namespace JsonClasses {

    /**
     * Classe serializavel responsável
     * por definir as cartas em jogo,
     * a vida dos ojogadores e o tipo
     * de ação que ocorreu
     */
    [Serializable]
    public class CardEvent {

        public int id;
        public int type;
        public int vidaPlayerOne;
        public int vidaPlayerTwo;
        public string cardsPlayerOne;
        public string cardsPlayerTwo;

        public CardEvent() {
            id = 0;
            type = 0;
            vidaPlayerOne = 20;
            vidaPlayerTwo = 20;

            cardsPlayerOne = "9999:0:0x9999:0:0x9999:0:0";
            cardsPlayerTwo = "9999:0:0x9999:0:0x9999:0:0";
        }
    }
}