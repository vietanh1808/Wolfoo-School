using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

public class Progress : MonoBehaviour
{
		/// <summary>
		/// The star off sprite.
		/// </summary>
		public Sprite starOff;
		
		/// <summary>
		/// The star on sprite.
		/// </summary>
		public Sprite starOn;

		/// <summary>
		/// The level stars.
		/// </summary>
		public Image[] levelStars;

		/// <summary>
		/// The game manager reference.
		/// </summary>
		public GameController GameController;

		/// <summary>
		/// The progress image.
		/// </summary>
		public Image progressImage;


		// Use this for initialization
		void Start ()
		{
				if (progressImage == null) {
						progressImage = GetComponent<Image> ();
				}

				if (GameController == null) {
						GameController = GameObject.FindObjectOfType<GameController> ();
				}
		}

		/// <summary>
		/// Set the value of the progress.
		/// </summary>
		/// <param name="currentTime">Current time.</param>
		public void SetProgress (float currentTime)
		{
				if (GameController == null) {
						return;
				}

				if (GameController.shape == null) {
						return;
				}

				if (progressImage != null)
						progressImage.fillAmount = 1 - (currentTime / (GameController.shape.twoStarsTimePeriod * 1.0f + 1));
			
				if (currentTime >= 0 && currentTime <= GameController.shape.threeStarsTimePeriod) {
						if (levelStars [0] != null) {
								levelStars [0].sprite = starOn;
						}
						if (levelStars [1] != null) {
								levelStars [1].sprite = starOn;
						}
						if (levelStars [2] != null) {
								levelStars [2].sprite = starOn;
						}
						if (progressImage != null)
								progressImage.color = Colors.greenColor;

				} else if (currentTime > GameController.shape.threeStarsTimePeriod && currentTime <= GameController.shape.twoStarsTimePeriod) {
						if (levelStars [2] != null) {
								levelStars [2].sprite = starOff;
						}
						if (progressImage != null)
								progressImage.color = Colors.yellowColor;

				} else {
						if (levelStars [1] != null) {
								levelStars [1].sprite = starOff;
						}
						if (levelStars [2] != null) {
								levelStars [2].sprite = starOff;
						}
						if (progressImage != null)
								progressImage.color = Colors.redColor;
				}
		}

}
