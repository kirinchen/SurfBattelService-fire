using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public interface DBResult {

        IEnumerable<DBResult> children();
        string key();
        string getRawJsonValue();
    }
}
