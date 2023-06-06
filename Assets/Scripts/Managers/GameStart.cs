using UnityEngine;

public class GameStart : MonoBehaviour
{
  private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, new Vector3(70f, 1f, 100f));
    }
}
