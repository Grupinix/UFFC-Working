using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace APIs {
    
    /**
     * Classe responsável por iniciar a
     * conexão com o Firebase e resolver
     * todas as dependencias existentes
     */
    public class FirebaseInit : MonoBehaviour {

        public UnityEvent OnFirebaseInitialized = new UnityEvent();

        private void Start() {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                if (DependencyStatus.Available != task.Result) {
                    return;
                }
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                OnFirebaseInitialized.Invoke();
            });
        }
    }
}