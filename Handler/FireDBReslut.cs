using Firebase.Database;
using RFNEet.realtimeDB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class FireDBReslut : DBResult {

        private DataSnapshot body;

        public FireDBReslut(DataSnapshot body) {
            this.body = body;
        }

        public IEnumerable<DBResult> children() {
            return new List<DataSnapshot>(body.Children).
                ConvertAll<DBResult>(d => { return new FireDBReslut(d); });
        }

        public string getRawJsonValue() {
            return body.GetRawJsonValue();
        }

        public object GetValue() {
            return body.Value;
        }

        public string key() {
            return body.Key;
        }
    }
}
