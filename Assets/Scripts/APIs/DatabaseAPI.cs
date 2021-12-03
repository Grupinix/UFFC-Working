using System;
using Firebase.Auth;
using Firebase.Database;

namespace APIs  {
    
    /**
     * Classe responsável por armazenar os
     * utilitários do Firebase e referências
     * requeridas constantemente
     */
    public static class DatabaseAPI {

        private static DatabaseReference _reference;
        private static FirebaseAuth _auth;
        
        /** Acesso rápido para os dados do usuário */
        public static FirebaseUser user;
        
        /**
         * Uma adaptação do java do método
         * System.currentTimeMillis()
         */
        public static long currentTimeMillis() {
            return (long) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        } 
        
        public static void setAuth(FirebaseAuth auth) {
            _auth = auth;
        }

        /**
         * Retorna uma instância do
         * FirebaseAuth caso já exista e
         * instância uma caso não
         */
        public static FirebaseAuth getAuth() {
            if (_auth != null) {
                return _auth;
            }
            _auth = FirebaseAuth.DefaultInstance;
            return _auth;
        }
        
        /**
         * Retorna uma instância da raiz do
         * FirebaseDatabase caso já exista e
         * instância uma casa não
         */
        public static DatabaseReference getDatabase() {
            if (_reference != null) {
                return _reference;
            }

            _reference = FirebaseDatabase.DefaultInstance.RootReference;
            return _reference;
        }
    }
}