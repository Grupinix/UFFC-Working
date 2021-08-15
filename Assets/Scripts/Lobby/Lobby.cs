using UnityEngine;
using UnityEngine.UI;

namespace Lobby {
    public class Lobby : MonoBehaviour {

        public Text textField;
        
        private void Start() {
            Text text = textField.GetComponent<Text>();
            text.text = PlayerPrefs.GetString("playerName", "error");
        }

    }
}