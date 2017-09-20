﻿using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Unity.Editor;

namespace RFNEet.firebase {
    public class FirebaseManager : MonoBehaviour {

        private static FirebaseManager instance;
        public string roomId { get; private set; }
        private FireRepo repo = new FireRepo();
        private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        public void init(string rid) {
            roomId = rid;
            dependencyStatus = FirebaseApp.CheckDependencies();
            if (dependencyStatus != DependencyStatus.Available) {
                FirebaseApp.FixDependenciesAsync().ContinueWith(task => {
                    dependencyStatus = FirebaseApp.CheckDependencies();
                    if (dependencyStatus == DependencyStatus.Available) {
                        InitializeFirebase();
                    } else {
                        Debug.LogError(
                            "Could not resolve all Firebase dependencies: " + dependencyStatus);
                        doneInited(false);
                    }
                });
            } else {
                InitializeFirebase();
            }
        }

        private List<Action<bool>> initedActions = new List<Action<bool>>();
        private void addInitedAction(Action<bool> a) { initedActions.Add(a); }

        private void InitializeFirebase() {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            app.SetEditorDatabaseUrl(FireConfig.getInstance().firebaseUrl);
            if (app.Options.DatabaseUrl != null) app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
            repo.initFire(roomId);
            doneInited(true);

        }

        public FireRepo getRepo() {
            return repo;
        }

        private void doneInited(bool b) {
            initedActions.ForEach(a => { a(b); });
            initedActions = null;
        }

        public static FirebaseManager getInstance() {
            if (instance == null) {
                instance = FindObjectOfType<FirebaseManager>();
            }
            return instance;
        }
    }
}
