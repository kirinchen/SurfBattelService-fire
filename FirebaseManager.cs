using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace RFNEet.firebase {
    public class FirebaseManager : MonoBehaviour {

        private static FirebaseManager instance;
        public string roomId { get; private set; }
        public FirePlayerQueuer playerQueuer { get; private set; }
        private FireRepo repo;
        //private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        private DBInit initer = new FireDBInit();


        void Awake() {
            playerQueuer = GetComponent<FirePlayerQueuer>();
            if (playerQueuer == null) {
                playerQueuer = gameObject.AddComponent<FirePlayerQueuer>();
            }
        }

        public void init(string rid, Action<bool> icb = null) {
            addInitedAction(icb);
            roomId = rid;

            initer.init(onFailInitializeFirebase, InitializeFirebase);


        }

        private void onFailInitializeFirebase(string msg) {
            Debug.LogError(
                          msg);
            doneInited(false);
        }

        private List<Action<bool>> initedActions = new List<Action<bool>>();
        public void addInitedAction(Action<bool> a) {
            if (initedActions == null) {
                a(initer.isOK());
            } else {
                initedActions.Add(a);
            }
        }

        private void InitializeFirebase() {
            Debug.Log("InitializeFirebase");
            initer.createConnect();
            repo = gameObject.AddComponent<FireRepo>();
            repo.initFire(initer.createRootRef(roomId), () => {
                doneInited(true);
            });

        }

        public static FireRepo getRepo() {
            return getInstance().repo;
        }

        public static DBRefenece getDBRef() {
            return getRepo().dbRef;
        }

        public static string getMePid() {
            return getInstance().playerQueuer.meId;
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
