using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterAnimation character;
    List<Transform> roadlanes;
    private int curIdxRoadlane;

    private void Start()
    {
        character = GetComponent<CharacterAnimation>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EventManager.OnCollisionObstacle?.Invoke(false);
    }
}
