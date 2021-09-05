using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UserData;

namespace APIs {
    
    /**
     * Classe responsável pelo sistema
     * de login
     */
    public class LoginAuth : MonoBehaviour {

        [SerializeField] private InputField emailInputField;
        [SerializeField] private InputField passwordInputField;
        [SerializeField] private Text warningLoginText;
        [SerializeField] private Text confirmLoginText;
        
        [SerializeField] private string registerScene;
        [SerializeField] private string lobbyScene;

        /** "ação" para ser chamada pelo botão de "LOGIN" */
        public void loginButton() {
            StartCoroutine(startLogin(emailInputField.text, passwordInputField.text));
        }

        /** "ação" para ser chamada pelo botão de "REGISTRO" */
        public void register() {
            SceneManager.LoadScene(registerScene);
        }

        /**
         * Verifica de maneira assincrona se os
         * dados inseridos pelo usuário são
         * válidos
         *
         * @param   email       email do usuário
         * @param   password    senha do usuário
         */
        private IEnumerator startLogin(string email, string password) {
            Task<FirebaseUser> loginTask = DatabaseAPI.getAuth().SignInWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => loginTask.IsCompleted);

            if (loginTask.Exception != null) {
                handleLoginErrors(loginTask.Exception);
            }
            else {
                loginUser(loginTask);
            }
        }

        /** Salva a instância do usuário para acesso futuro */
        private void loginUser(Task<FirebaseUser> loginTask) {
            DatabaseAPI.user = loginTask.Result;

            warningLoginText.text = "";
            confirmLoginText.text = "Logado";

            StartCoroutine(goToLoged());
        }

        /** Atualiza o perfil do usuário e carrega a cena do "Lobby" */
        private IEnumerator goToLoged() {
            IDictionary<string, object> data = new Dictionary<string, object> {
                {"lastDataSeen", DateTime.Now.ToString("dd/MM/yyyy")}
            };
            ProfileManager.updateUserFields(data);
            yield return new WaitForSeconds(1);
            
            SceneManager.LoadScene(lobbyScene);
        }

        /** Converte um erro genérico em um erro do Firebase */
        private void handleLoginErrors(AggregateException loginException) {
            FirebaseException firebaseException = loginException.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError) firebaseException.ErrorCode;

            warningLoginText.text = defineLoginErrorMessage(errorCode);
        }

        /** Retorna o erro de forma amigável ao usuário */
        private string defineLoginErrorMessage(AuthError errorCode) {
            switch (errorCode) {
                case AuthError.MissingEmail:
                    return "Faltando email";
                case AuthError.InvalidEmail:
                    return "Email inválido";
                case AuthError.MissingPassword:
                    return "Faltando senha";
                case AuthError.UserNotFound:
                    return "Conta não existente";
                case AuthError.InvalidCredential:
                    return "Senha incorreta";
                default:
                    return "Credenciais inválidas";
            }
        }
    }
}