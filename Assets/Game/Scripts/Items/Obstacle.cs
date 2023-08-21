using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Obstacle : MonoBehaviour
{
    Image image;
    new BoxCollider2D collider2D;

    private void Start()
    {
        image = GetComponent<Image>();
        collider2D = GetComponent<BoxCollider2D>();
    }

    public void AssignItem(Sprite sprite, Vector3 _position)
    {
        image = GetComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();
        GameManager.instance.ScaleImage(image, 800, 600);

        transform.position = _position + Vector3.up;

        collider2D = GetComponent<BoxCollider2D>();
        collider2D.size = new Vector2(transform.GetComponent<RectTransform>().rect.size.x, 200);
    }

}
