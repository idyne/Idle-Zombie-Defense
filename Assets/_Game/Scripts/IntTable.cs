using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IntTable", menuName = "Fate/IntTable", order = 1)]
public class IntTable : ScriptableObject
{
    [SerializeField] private List<int> table = new();

    public int this[int key]
    {
        get => table[key];
        set => table[key] = value;
    }
}
