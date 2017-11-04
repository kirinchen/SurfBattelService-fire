using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public interface DBRefenece {

        Action<DBResult> childAdded { get; set; }
        Action<DBResult> ValueChanged { get; set; }

        void fetchValue(Action<DBResult> a);

        DBRefenece Child(string pid);
        void SetRawJsonValueAsync(string s);
        void removeMe();
    }
}
