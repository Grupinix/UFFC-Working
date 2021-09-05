using System.Collections;
using System.Collections.Generic;
using Default;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UserData;

namespace Room {
    
    /**
     * Classe responsável pela interface
     * do usuário enquanto o mesmo
     * estiver em uma partida
     */
    public class UserInterface : MonoBehaviour {

        public bool canDropMana;
        
        private int _redManaGen;
        private int _redManaInPool;
        private int _blueManaGen;
        private int _blueManaInPool;
        private int _greenManaGen;
        private int _greenManaInPool;
            
        private int _actualCardId;

        public int life = 20;
        public int oldLife = 20;
        public int enemyLife = 20;
        public int oldEnemyLife = 20;
        [SerializeField] private List<Material> materials;
        [SerializeField] private GameObject cardInfoPanel;
        [SerializeField] private Image cardImage;
        [SerializeField] private Slider barraDeVida;
        [SerializeField] private Text textoVida;
        [SerializeField] private Slider barraDeVidaInimiga;
        [SerializeField] private Text textoVidaInimiga;
        [SerializeField] private Text redMana;
        [SerializeField] private Text blueMana;
        [SerializeField] private Text greenMana;

        [SerializeField] private GameObject deck;
        [SerializeField] private GameObject enemyImage;

        [SerializeField] private GameObject cardAttackPanel;
        public Button cardAttackButton;
        [SerializeField] private Image cardAttackImage;

        [SerializeField] private GameObject gamePanel;
        [SerializeField] private GameObject statusPanel;
        [SerializeField] private Texture2D[] images;

        private DeckController _deckController;
        private Turn _turn;

        private bool _status;

        private void Start() {
            _deckController = deck.GetComponent<DeckController>();
            _turn = GetComponent<Turn>();
        }

        private void Update() {
            if (enemyLife != oldEnemyLife) {
                if (enemyLife > 0) {
                    oldEnemyLife = enemyLife;
                    return;
                }

                if (!_status) {
                    StartCoroutine(victory());
                    _status = true;
                }
                return;
            } 
            if (oldLife != life && life <= 0) {
                if (life > 0) {
                    oldLife = life;
                    return;
                }

                if (!_status) {
                    StartCoroutine(defeat());
                    _status = true;
                }
            }
        }

        /**
         * Define que o usuário foi o
         * vencedor da partida
         */
        private IEnumerator victory() {
            statusPanel.SetActive(true);
            gamePanel.SetActive(false);
            statusPanel.GetComponent<RawImage>().texture = images[0];
            
            yield return new WaitForSeconds(5);

            PlayerPrefs.SetInt("playerWins", PlayerPrefs.GetInt("playerWins", 0) + 1);
            PlayerPrefs.Save();
            ProfileManager.updateUserFields(new Dictionary<string, object> {
                {"wins", long.Parse(PlayerPrefs.GetInt("playerWins").ToString())}
            });
            SceneManager.LoadScene("Lobby");
        }
        
        /**
         * Define que o usuário foi o
         * perdedor da partida
         */
        private IEnumerator defeat() {
            statusPanel.SetActive(true);
            gamePanel.SetActive(false);
            statusPanel.GetComponent<RawImage>().texture = images[1];
            
            yield return new WaitForSeconds(5);

            PlayerPrefs.SetInt("playerLoses", PlayerPrefs.GetInt("playerLoses", 0) + 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Lobby");
        }

        /** "ação" para abrir o painel de ataque de uma carta */
        public void openCardAttackView(int cardId) {
            if (cardId == 9999) {
                return;
            }
            cardAttackPanel.SetActive(true);
            cardInfoPanel.SetActive(false);
            enemyImage.SetActive(false);
            cardAttackImage.material = materials[cardId];
        }

        /** "ação" para abrir o painel visualização de uma carta do oponente */
        public void openCardEnemyView(int cardId) {
            if (cardId == 9999) {
                return;
            }
            enemyImage.SetActive(true);
            cardInfoPanel.SetActive(false);
            cardAttackPanel.SetActive(false);
            enemyImage.GetComponent<Image>().material = materials[cardId];
        }

        /** "ação" para abrir o painel visualização de uma carta da sua mão */
        public void openCardView(int cardId) {
            if (cardId == 9999) {
                return;
            }
            _actualCardId = cardId;
            cardInfoPanel.SetActive(true);
            enemyImage.SetActive(false);
            cardAttackPanel.SetActive(false);
            cardImage.material = materials[cardId];
        }

        /** "ação" para descartar uma carta da sua mão */
        public void descartar() {
            if (!_turn.userTurn) {
                return;
            }
            _deckController.removerCarta(_actualCardId);
            cardInfoPanel.SetActive(false);
        }
        
        /** "ação" para utilizar uma carta da sua mão */
        public void castCard() {
            if (!_turn.userTurn) {
                return;
            }
            CardStatus cardStatus = UserDeck.getCardStatus(_actualCardId);
            if (cardStatus.isTerrain) {
                if (!canDropMana) {
                    return;
                }
                canDropMana = false;
                if (cardStatus.manaGen[0] > 0) {
                    _redManaGen += cardStatus.manaGen[0];
                    _redManaInPool += cardStatus.manaGen[0];
                    redMana.text = _redManaInPool.ToString();
                }
                if (cardStatus.manaGen[1] > 0) {
                    _blueManaGen += cardStatus.manaGen[1];
                    _blueManaInPool += cardStatus.manaGen[1];
                    blueMana.text = _blueManaInPool.ToString();
                }
                if (cardStatus.manaGen[2] > 0) {
                    _greenManaGen += cardStatus.manaGen[2];
                    _greenManaInPool += cardStatus.manaGen[2];
                    greenMana.text = _greenManaInPool.ToString();
                }
                _deckController.removerCarta(_actualCardId);
                cardInfoPanel.SetActive(false);
                return;
            }

            if (_turn.campoCheio()) {
                return;
            }
            
            int[] newMana = hasMana(cardStatus);
            if (newMana[0] == 0) {
                return;
            }

            _redManaInPool = newMana[1];
            _blueManaInPool = newMana[2];
            _greenManaInPool = newMana[3];
            
            redMana.text = _redManaInPool.ToString();
            blueMana.text = _blueManaInPool.ToString();
            greenMana.text = _greenManaInPool.ToString();
            
            _turn.inserirCarta(_actualCardId);
            cardInfoPanel.SetActive(false);
            _deckController.removerCarta(_actualCardId);
        }

        /** Calcula os valores de mana exigidos por uma carta */
        private int[] hasMana(CardStatus cardStatus) {
            int redManaActual = _redManaInPool;
            int blueManaActual = _blueManaInPool;
            int greenManaActual = _greenManaInPool;
            redManaActual -= cardStatus.manaCost[0];
            blueManaActual -= cardStatus.manaCost[1];
            greenManaActual -= cardStatus.manaCost[2];
            
            if (redManaActual < 0 || blueManaActual < 0 || greenManaActual < 0) {
                return new [] {0, 0, 0, 0};
            }

            if (cardStatus.manaCost[3] == 0) {
                return new[] {
                    1,
                    redManaActual,
                    blueManaActual,
                    greenManaActual
                };
            }

            int genericOriginal = cardStatus.manaCost[3];
            if (redManaActual > 0) {
                genericOriginal -= redManaActual;
                if (genericOriginal < 0) {
                    redManaActual = 0;
                    redManaActual -= genericOriginal;
                    genericOriginal = 0;
                }
                else {
                    redManaActual = 0;
                }
            }
            if (blueManaActual > 0) {
                genericOriginal -= blueManaActual;
                if (genericOriginal < 0) {
                    blueManaActual = 0;
                    blueManaActual -= genericOriginal;
                    genericOriginal = 0;
                }
                else {
                    blueManaActual = 0;
                }
            }
            if (greenManaActual > 0) {
                genericOriginal -= greenManaActual;
                if (genericOriginal < 0) {
                    greenManaActual = 0;
                    greenManaActual -= genericOriginal;
                    genericOriginal = 0;
                }
                else {
                    greenManaActual = 0;
                }
            }

            if (genericOriginal > 0) {
                return new[] {0, 0, 0, 0};
            }

            return new[] {
                1,
                redManaActual,
                blueManaActual,
                greenManaActual
            };
        }

        /** "ação" para redefinir a mana atual do usuário */
        public void resetMana() {
            _redManaInPool = _redManaGen;
            redMana.text = _redManaInPool.ToString();
            _blueManaInPool = _blueManaGen;
            blueMana.text = _blueManaInPool.ToString();
            _greenManaInPool = _greenManaGen;
            greenMana.text = _greenManaInPool.ToString();
        }
        
        /** "ação" para atualizar a vida dos jogadores */
        public void attVidaDisplay() {
            textoVida.text = life.ToString();
            textoVidaInimiga.text = enemyLife.ToString();
            barraDeVida.value = life;
            barraDeVidaInimiga.value = enemyLife;
        }

        /** "ação" para encerrar o jogo */
        public void closeGame() {
            Application.Quit();
        }

        /** "ação" para fechar o painel de visualização de uma carta na mão */
        public void closePanel() {
            cardInfoPanel.SetActive(false);
        }
        
        /** "ação" para fechar o painel de ataque de uma carta */
        public void closeAttackPanel() {
            cardAttackPanel.SetActive(false);
        }
        
    }
}