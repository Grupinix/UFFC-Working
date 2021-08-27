using System;

namespace JsonClasses {

    [Serializable]
    public class CardEvent {

        public int id;
        public int type;
        public int[] targets = new int[3];

        public CardEvent() {
            id = 0;
            type = 0;
            for (int i = 0; i < 3; i++) {
                targets[i] = 0;
            }
        }
    }
}