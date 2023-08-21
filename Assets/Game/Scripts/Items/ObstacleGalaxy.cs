using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ObstacleGalaxy : MonoBehaviour
{
    [SerializeField] ParticleSystem smokeFx;
    [SerializeField] ParticleSystem starFx;
    
    int id;
    Image image;

    public void AssignItem(int _id, Sprite sprite)
    {
        id = _id;
        image = GetComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();
        image.DOFade(1, 0);
    }

    public void OnSucess()
    {
       // starFx.Play();
    }
    public void OnFail()
    {
        image.DOFade(0, 0);
      //  smokeFx.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        EventManager.OnCollisionObstacleGalaxy?.Invoke(id, this);
    }
}
