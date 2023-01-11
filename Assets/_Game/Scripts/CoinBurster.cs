using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public static class CoinBurster
{
    public static void Burst(int gain, int numberOfCoins, Vector3 position, Vector3 direction, float forceMultiplier = 7, bool time = true)
    {
        float delay;
        if (time)
            delay = 0.3f / numberOfCoins;
        else delay = 0;
        Debug.Log("gain: " + gain + " count: " + numberOfCoins);
        GameManager.Instance.StartCoroutine(BurstCoin(gain, numberOfCoins, delay, position, direction, forceMultiplier));
    }

    private static IEnumerator BurstCoin(int gain, int count, float delay, Vector3 position, Vector3 direction, float forceMultiplier = 7)
    {
        float randomScaler = 0.3f;
        Rigidbody coinRigidbody = ObjectPooler.SpawnFromPool("Coin", position, Quaternion.identity).GetComponent<Rigidbody>();

        int currentGain = gain / count;
        coinRigidbody.GetComponent<Coin>().Gain = currentGain;
        Debug.Log(currentGain);
        gain -= currentGain;

        coinRigidbody.AddForce((direction + new Vector3(Random.Range(-randomScaler, randomScaler), 0, Random.Range(-randomScaler, randomScaler))) * forceMultiplier, ForceMode.Impulse);
        coinRigidbody.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), ForceMode.Impulse);
        yield return new WaitForSeconds(delay);

        if (--count > 0) yield return BurstCoin(gain, count, delay, position, direction, forceMultiplier);
    }
}
