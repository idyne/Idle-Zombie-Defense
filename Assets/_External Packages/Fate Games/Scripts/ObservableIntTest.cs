using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservableIntTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ObservableInt a = new(5);
        print(a.Value);
        a.OnChange.AddListener((val) => print(val));

        a.Value = 7;
        a.Value = 18;
    }


}
