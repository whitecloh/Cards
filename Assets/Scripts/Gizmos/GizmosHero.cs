using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosHero : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, new Vector3(200f, 1f, 250f));
    }
}
