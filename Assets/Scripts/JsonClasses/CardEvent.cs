using System;

namespace JsonClasses {

    [Serializable]
    public class CardEvent {

        public int id;
        public int type;
        public int[] targets;

        public CardEvent() {
            id = 0;
            type = 0;
            targets = new[] {0, 0, 0};
        }
    }
}