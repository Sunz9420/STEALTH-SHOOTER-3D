using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class member1 : MonoBehaviour
{
    public float viewRadius = 10f;     // Bán kính tầm nhìn của AI
    [Range(0,360)]
    public float viewAngle = 90f;      // Góc nhìn của AI

    public LayerMask playerMask;       // Layer của Player
    public LayerMask obstacleMask;     // Layer của vật cản

    [HideInInspector]
    public bool canSeePlayer = false;  // Biến kiểm tra AI đã thấy Player chưa

    void Start()
    {
        StartCoroutine(FindPlayersWithDelay(0.2f));
    }

    IEnumerator FindPlayersWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FieldOfViewCheck();
        }
    }

    void FieldOfViewCheck()
    {
        canSeePlayer = false;
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    canSeePlayer = true;
                    Debug.Log("AI đã nhìn thấy Player!");
                }
            }
        }
    }
}