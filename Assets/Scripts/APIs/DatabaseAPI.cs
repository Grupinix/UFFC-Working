using System;
using Firebase.Auth;
using Firebase.Database;

namespace APIs  {
    public static class DatabaseAPI {
        private static DatabaseReference _reference;
        private static FirebaseAuth _auth;
        
        public static FirebaseUser user;
        
        public static long currentTimeMillis() {
            return (long) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        } 
        
        public static FirebaseAuth getAuth() {
            if (_auth != null) {
                return _auth;
            }
            _auth = FirebaseAuth.DefaultInstance;
            return _auth;
        }
        
        public static DatabaseReference getDatabase() {
            if (_reference != null) {
                return _reference;
            }

            _reference = FirebaseDatabase.DefaultInstance.RootReference;
            return _reference;
        }
    }
}