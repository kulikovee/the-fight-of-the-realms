﻿using System.Collections;
using UnityEngine;

public class DestroyAfter: MonoBehaviour
{
    public float delay = 1f;

    void Start()
    {
        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
