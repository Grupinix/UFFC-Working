using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby {
    public class DeckChange : MonoBehaviour {

        [SerializeField] private GameObject parent;
        [SerializeField] private GameObject element;

        public void loadLobbyScene() {
            SceneManager.LoadScene("Lobby");
        }

        private void Start() {
            for (int i = 0; i < 20; i++) {
                GameObject card = Instantiate(element, parent.transform);
                card.transform.localScale = new Vector3(25, 1, 25);
                card.transform.rotation = Quaternion.Euler(90, 0, 180);
            }
        }
    }
}