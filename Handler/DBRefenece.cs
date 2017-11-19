using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace RFNEet.firebase {
    public interface DBRefenece {

        Action<DBResult> childAdded { get; set; }
        Action<DBResult> childRemoved { get; set; }
        Action<DBResult> ValueChanged { get; set; }

        void fetchValue(Action<DBResult> a);

        DBRefenece parent();
        DBRefenece Child(string pid);
        Task SetValueAsync(object value);
        Task SetRawJsonValueAsync(string s);
        void removeMe();
    }
}
