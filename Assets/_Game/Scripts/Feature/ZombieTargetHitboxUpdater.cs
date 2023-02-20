using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieTargetHitboxUpdater : MonoBehaviour
{
    private static ZombieTargetHitboxUpdater instance = null;
    public static ZombieTargetHitboxUpdater Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ZombieTargetHitboxUpdater>();
            return instance;
        }
    }
    private List<ZombieTargetHitbox> hitboxes = new();

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < hitboxes.Count; i++)
        {
            ZombieTargetHitbox hitbox = hitboxes[i];
            if (hitbox.gameObject.activeSelf)
                hitbox.SetPosition();
        }
    }

    public void Register(ZombieTargetHitbox hitbox) { hitboxes.Add(hitbox); }
}
