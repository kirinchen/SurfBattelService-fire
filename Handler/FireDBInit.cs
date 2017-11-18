using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class FireDBInit : DBInit {

        private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

        public void createConnect() {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            string fireUrl = FireConfig.getInstance().getFirebaseUrl();
            app.SetEditorDatabaseUrl(fireUrl);
            if (app.Options.DatabaseUrl != null) app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);

        }

        public DBRefenece createRootRef(string roomId) {
            string root = FireConfig.getInstance().getRootNode();
            Debug.Log("root=" + root + " rid=" + roomId);
            return new FireDBRefence(FirebaseDatabase.DefaultInstance.GetReference(root).Child(roomId));
        }

        public void init( Action<string> onFailInitializeFirebase, Action initializeFirebase) {
            dependencyStatus = FirebaseApp.CheckDependencies();
            if (dependencyStatus != DependencyStatus.Available) {
                FirebaseApp.FixDependenciesAsync().ContinueWith(task => {
                    dependencyStatus = FirebaseApp.CheckDependencies();
                    if (dependencyStatus == DependencyStatus.Available) {
                        initializeFirebase();
                    } else {
                        onFailInitializeFirebase("Could not resolve all Firebase dependencies: " +
                          dependencyStatus);
                    }
                });
            } else {
                initializeFirebase();
            }
        }

        public bool isOK() {
            return dependencyStatus == DependencyStatus.Available;
        }
    }
}
