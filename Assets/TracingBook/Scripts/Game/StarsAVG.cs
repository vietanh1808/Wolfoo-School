using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

public class StarsAVG : MonoBehaviour {

	/// <summary>
	/// The stars reference.
	/// </summary>
	public Image[] stars;

	/// <summary>
	/// The shapes manager reference as name.
	/// </summary>
	public string shapesManagerReference;

	/// <summary>
	/// The star on,off sprites.
	/// </summary>
	public Sprite starOn, starOff;

	// Use this for initialization
	IEnumerator Start () {

		yield return 0;

		//Setting up the stars rating(Average)
		ShapesManager shapesManager = GameObject.Find (shapesManagerReference).GetComponent<ShapesManager> ();
		int collectedStars = DataManager.GetCollectedStars (shapesManager);
		int starsRate = Mathf.FloorToInt(collectedStars /(shapesManager.shapes.Count * 3.0f) * 3.0f);
	
		if (starsRate == 0) {//Zero Stars
			stars [0].sprite = starOff;
			stars [1].sprite = starOff;
			stars [2].sprite = starOff;
		}else if (starsRate == 1) {//One Star
			stars [0].sprite = starOn;
			stars [1].sprite = starOff;
			stars [2].sprite = starOff;
		} else if (starsRate == 2) {//Two Stars
			stars [0].sprite = starOn;
			stars [1].sprite = starOn;
			stars [2].sprite = starOff;
		} else {//Three Stars
			stars [0].sprite = starOn;
			stars [1].sprite = starOn;
			stars [2].sprite = starOn;
		}
	}
}
