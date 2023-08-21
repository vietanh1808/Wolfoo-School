using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    CharacterAnimation character;
    List<Transform> roadlanes;
    private int curIdxRoadlane = 0;

    public void AssignLane(List<Transform> _roadlanes)
    {
        roadlanes = _roadlanes;
        character = GetComponent<CharacterAnimation>();
        character.transform.position = roadlanes[curIdxRoadlane].position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        character.transform.position = roadlanes[curIdxRoadlane == 0 ? 1 : 0].position;
    }
}
