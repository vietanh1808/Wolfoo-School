using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using SCN;
public class Pencil : MonoBehaviour,IPointerClickHandler
{
	/// <summary>
	/// The color of the pencil.
	/// </summary>
	public Color value;


    private void Awake()
    {
		GetComponent<Animator>().enabled = false;
	}
    /// <summary>
    /// Enable pencil selection.
    /// </summary>
    public void EnableSelection(){
		GetComponent<Animator>().enabled = true;
		GetComponent<Animator>().SetBool("RunScale",true);
	}

	/// <summary>
	/// Disable pencil selection.
	/// </summary>
	public void DisableSelection(){
		GetComponent<Animator>().SetBool("RunScale",false);
		GetComponent<Animator>().enabled = false;
	}

    public void OnPointerClick(PointerEventData eventData)
    {
		// EventDispatcher.Instance.Dispatch(new EventKey.ChangePencil { pencilClick = this });
    }
}
