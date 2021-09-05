using System;
using System.Collections.Generic;
using APIs;
using JsonClasses;
using UnityEngine;
using UserData;
using Random = UnityEngine.Random;

namespace Room {
    
    /**
     * Classe responsável por controlar o
     * baralho do usuário enquanto
     * ele estiver em partida
     */
    public class DeckController : MonoBehaviour {

        private Vector3 _deckScale;
        private Vector3 _positionHand;
        private Vector3 _targetPosition;

        private int _totalInitialCards;
        private List<int> _deckListCards;

        [SerializeField] private GameObject manager;
        [SerializeField] private GameObject enemyDeck;
        
        [SerializeField] private Vector3 rangeCardPosition;
        [SerializeField] private Vector3 positionShowPlayer;
        
        [SerializeField] private GameObject camObj;
        [SerializeField] private GameObject hand;
        [SerializeField] private GameObject cardPrefab;
        
        [SerializeField] private float timeToShowPlayer;
        [SerializeField] private float dumbGetCard;

        private UserInterface _userInterface;
        private Turn _turn;
        private float _currentTimeToShowPlayer;
        private Camera _camera;

        private GameObject _tempCard;


        private Vector3 _minPosition;
        private Vector3 _maxPosition;
        private Vector3 _positionNextCard;

        private List<CardProperties> cards = new List<CardProperties>();

        public static bool canBuy;
        
        private void Start() {
            _camera = camObj.GetComponent<Camera>();
            _userInterface = manager.GetComponent<UserInterface>();
            _turn = manager.GetComponent<Turn>();
            _deckListCards = UserDeck.getDeckCards();
            _deckScale = transform.localScale;
            _totalInitialCards = _deckListCards.Count;

            Vector3 handPosition = hand.transform.position;
            _minPosition = handPosition - rangeCardPosition;
            _maxPosition = handPosition + rangeCardPosition;
            for (int i = 0; i < 4; i++) {
                getCard();
            }
        }

        private void Update() {
            if (_tempCard != null) {
                _currentTimeToShowPlayer += Time.deltaTime;
                if (_currentTimeToShowPlayer > timeToShowPlayer) {
                    _positionHand = _positionNextCard;
                    _targetPosition = _positionHand;
                }
                
                _tempCard.transform.position = Vector3.Lerp(_tempCard.transform.position, _targetPosition + new Vector3(0, 0.2f, 0), dumbGetCard * Time.deltaTime);
            }

            if (Input.touchCount == 1) {
                checkHit(_camera.ScreenPointToRay(Input.touches[0].position), false);
            }
            else if (Input.GetMouseButtonDown(0)) {
                checkHit(_camera.ScreenPointToRay(Input.mousePosition), true);
            }
        }
        
        /**
         * Verifica se o usuário está "clicando"
         * em algum objeto especifico do jogo
         * para assim executar uma ação como
         * por exemplo abrir um menu, comprar
         * uma carta do baralho, e etc.
         */
        private void checkHit(Ray ray, bool pc) {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (canBuy && hit.collider.gameObject.name == "Deck") {
                    if (pc) {
                        getCard();
                        return;
                    }

                    Touch screenTouch = Input.GetTouch(0);
                    if (screenTouch.phase == TouchPhase.Ended) {
                        getCard();
                    }
                }
                else if (hit.collider.gameObject.name == "Front") {
                    if (pc) {
                        _userInterface.openCardView(hit.collider.gameObject.GetComponentInParent<CardProperties>().cardId);
                        return;
                    }

                    Touch screenTouch = Input.GetTouch(0);
                    if (screenTouch.phase == TouchPhase.Ended) {
                        _userInterface.openCardView(hit.collider.gameObject.GetComponentInParent<CardProperties>().cardId);
                    }
                }
                else if (hit.collider.gameObject.name == "FieldEnemyFront") {
                    if (pc) {
                        _userInterface.openCardEnemyView(hit.collider.gameObject.GetComponentInParent<CardProperties>().cardId);
                        return;
                    }

                    Touch screenTouch = Input.GetTouch(0);
                    if (screenTouch.phase == TouchPhase.Ended) {
                        _userInterface.openCardEnemyView(hit.collider.gameObject.GetComponentInParent<CardProperties>().cardId);
                    }
                }
                else if (hit.collider.gameObject.name == "FieldFront") {
                    CardProperties cardProperties = hit.collider.gameObject.GetComponentInParent<CardProperties>();
                    if (!cardProperties.ataque) {
                        return;
                    }
                    if (pc) {
                        _userInterface.openCardAttackView(cardProperties.cardId);
                        _userInterface.cardAttackButton.onClick.RemoveAllListeners();
                        _userInterface.cardAttackButton.onClick.AddListener(() => {
                            if (!_turn.userTurn) {
                                return;
                            }
                            cardProperties.ataque = false;
                            _userInterface.closeAttackPanel();
                            int indice = -999;
                            int maisFraca = Int32.MaxValue;
                            for (int i = 0; i < 3; i++) {
                                CardProperties cardP = _turn.enemyCards[i].GetComponent<CardProperties>();
                                if (cardP.cardId != 9999 && cardP.cardDefense < maisFraca) {
                                    indice = i;
                                    maisFraca = cardP.cardDefense;
                                }
                            }

                            if (indice == -999) {
                                _userInterface.enemyLife -= cardProperties.cardPower;
                                _userInterface.attVidaDisplay();
                                DatabaseAPI.getDatabase().Child("rooms").Child(_turn._nameOfRoom).UpdateChildrenAsync(new Dictionary<string, object> {
                                    {"event", JsonUtility.ToJson(_turn.getField())}
                                });
                                return;
                            }
                            CardProperties cardEnemy = _turn.enemyCards[indice].GetComponent<CardProperties>();
                            cardEnemy.cardDefense -= cardProperties.cardPower;
                            cardProperties.cardDefense -= cardEnemy.cardPower;
                            if (cardEnemy.cardDefense <= 0) {
                                cardEnemy.cardId = 9999;
                                cardEnemy.setMaterial();
                            }

                            if (cardProperties.cardDefense <= 0) {
                                cardProperties.cardId = 9999;
                                cardProperties.setMaterial();
                            }
                            DatabaseAPI.getDatabase().Child("rooms").Child(_turn._nameOfRoom).UpdateChildrenAsync(new Dictionary<string, object> {
                                {"event", JsonUtility.ToJson(_turn.getField())}
                            });
                        });
                        return;
                    }
                    
                    Touch screenTouch = Input.GetTouch(0);
                    if (screenTouch.phase == TouchPhase.Ended) {
                        _userInterface.openCardAttackView(cardProperties.cardId);
                        _userInterface.cardAttackButton.onClick.RemoveAllListeners();
                        _userInterface.cardAttackButton.onClick.AddListener(() => {
                            if (!_turn.userTurn) {
                                return;
                            }
                            cardProperties.ataque = false;
                            _userInterface.closeAttackPanel();
                            int indice = -999;
                            int maisFraca = Int32.MaxValue;
                            for (int i = 0; i < 3; i++) {
                                CardProperties cardP = _turn.enemyCards[i].GetComponent<CardProperties>();
                                if (cardP.cardId != 9999 && cardP.cardDefense < maisFraca) {
                                    indice = i;
                                    maisFraca = cardP.cardDefense;
                                }
                            }

                            if (indice == -999) {
                                _userInterface.enemyLife -= cardProperties.cardPower;
                                _userInterface.attVidaDisplay();
                                DatabaseAPI.getDatabase().Child("rooms").Child(_turn._nameOfRoom).UpdateChildrenAsync(new Dictionary<string, object> {
                                    {"event", JsonUtility.ToJson(_turn.getField())}
                                });
                                return;
                            }
                            CardProperties cardEnemy = _turn.enemyCards[indice].GetComponent<CardProperties>();
                            cardEnemy.cardDefense -= cardProperties.cardPower;
                            cardProperties.cardDefense -= cardEnemy.cardPower;
                            if (cardEnemy.cardDefense <= 0) {
                                cardEnemy.cardId = 9999;
                                cardEnemy.setMaterial();
                            }

                            if (cardProperties.cardDefense <= 0) {
                                cardProperties.cardId = 9999;
                                cardProperties.setMaterial();
                            }
                            DatabaseAPI.getDatabase().Child("rooms").Child(_turn._nameOfRoom).UpdateChildrenAsync(new Dictionary<string, object> {
                                {"event", JsonUtility.ToJson(_turn.getField())}
                            });
                        });
                    }
                }
            }
        }
        
        /**
         * Realiza uma animação movendo
         * a carta do baralho até a
         * mão do usuário
         */
        private void getCard() {
            if (_deckListCards.Count > 0 && cards.Count < 5) {
                int randCardIndex = Random.Range(0, _deckListCards.Count);
                int id = _deckListCards[randCardIndex];
                _deckListCards.RemoveAt(randCardIndex);

                _tempCard = Instantiate(cardPrefab, hand.transform);
                _tempCard.transform.rotation = Quaternion.Euler(15, 180, 0);
                CardProperties cardProperties = _tempCard.GetComponent<CardProperties>();
                cardProperties.cardId = id;
                cardProperties.setMaterial();

                resizeDeck();

                _targetPosition = positionShowPlayer;
                _currentTimeToShowPlayer = 0;
                addCard(_tempCard.GetComponent<CardProperties>());
                canBuy = false;
            }
        }

        /**
         * Modifica o tamanho dos objetos
         * de baralho em jogo para simular
         * que o seu tamanho está diminuindo
         */
        private void resizeDeck() {
            resizeBoxSize(transform);
            resizeBoxSize(enemyDeck.transform);
        }

        /**
         * Diminui um objeto pela sua altura
         * se baseando na quantidade de cartas
         * restante no baralho do usuário
         */
        private void resizeBoxSize(Transform objTransform) {
            Vector3 newSize = objTransform.localScale;

            newSize.y = _deckListCards.Count * _deckScale.y / _totalInitialCards;
            objTransform.localScale = newSize;

            if (_deckListCards.Count == 0) {
                objTransform.GetComponent<Renderer>().enabled = false;
            }
        }

        /** Remove uma carta da mão do usuário */
        public void removerCarta(int cardId) {
            int remove = 9999;
            for (int i = 0; i < cards.Count; i++) {
                if (cards[i].cardId == cardId) {
                    remove = i;
                    break;
                }
            }

            if (remove == 9999) {
                return;
            }

            CardProperties cardProperties = cards[remove];
            if (_tempCard != null) {
                if (_tempCard.GetComponent<CardProperties>().cardId == cardProperties.cardId) {
                    _tempCard = null;
                }
            }
            Destroy(cardProperties.gameObject);
            cards.RemoveAt(remove);
            reorganizeCards();
        }
        
        /** Reorganiza as cartas na mão do usuário */
        private void reorganizeCards(){
            for(int i = 1; i < cards.Count; i++){
                Vector3 position = calcDistanceHandPosition(i, cards.Count + 1);
                if(i-1 < cards.Count){
                    cards[i-1].transform.position = position;
                }
            }
        
            _positionNextCard = calcDistanceHandPosition(cards.Count, cards.Count + 1);
        }

        private Vector3 calcDistanceHandPosition(int indice, int limit) {
            float distance = indice / (float) limit;

            return Vector3.Lerp(_minPosition, _maxPosition, distance);
        }

        /** Adiciona uma carta do baralho até a mão do usuário */
        private void addCard(CardProperties card){
            cards.Add(card);
            reorganizeCards();
        }
    }
}