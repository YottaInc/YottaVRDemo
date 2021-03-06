﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace YottaIO.View {
	public class FbWindow : Window 
	{
		private const string NOT_LIKED_TEXT = "8 others liked this photo";
		private const string LIKED_TXT = "<b>Charlie Miller</b> and 8 others liked this photo";

		public Text otherLikesTxt;

		public GameObject likeAnimImg;
		public GameObject openAnimImg;
		public GameObject angerAnimImg;
		public GameObject laughAnimImg;

		public GameObject likeImg;
		public GameObject openImg;
		public GameObject angerImg;
		public GameObject laughImg;

		private EmotionData.EmotionEnum lastEmotion;
		private const float MAX_CONSECUTIVE_READINGS = 3.0f;
		private const float EMOTION_COMPLETION_TIME = BluetoothListener.DATA_UPDATE_FREQUENCY * MAX_CONSECUTIVE_READINGS;
		private float currEmotionActiveTime = 0.0f;

		private float currPictureShowTime = 0.0f;
		private const float MAX_PICTURE_SHOW_TIME = 4.0f;
		public List<GameObject> imageToShow = new List<GameObject> ();
		public List<EmotionData.EmotionEnum> imageEmotion = new List<EmotionData.EmotionEnum> ();
		public List<bool> isImageLiked = new List<bool> ();
		private int currPictureIndex = 0;

		//-----Debug only, please remove-----
		//private BluetoothListener debugBT;
		//-----------------------------------

		void Start ()
		{
			//debugBT = GameObject.Find ("Bluetooth").GetComponent <BluetoothListener> ();

			for (int i = 0; i <= 5; i++) {
				imageEmotion.Add (EmotionData.EmotionEnum.neutral);	
				isImageLiked.Add (false);
			}

			//--------DEBUG - REMOVE Later-------------
			/*#if UNITY_EDITOR
			Invoke ("debugInvoke", 1.0f);
			#endif*/
			//----------------------------
		}

		void Update () {
			currPictureShowTime += Time.unscaledDeltaTime;

			if (FaceController.CurrentEmotionState == lastEmotion) 
			{
				currEmotionActiveTime += Time.unscaledDeltaTime;

				if (imageEmotion[currPictureIndex] != lastEmotion && currEmotionActiveTime >= EMOTION_COMPLETION_TIME)
					OnEmotionCompleted ();
			} 
			else 
			{
				lastEmotion = FaceController.CurrentEmotionState;
				currEmotionActiveTime = Time.unscaledDeltaTime;
			}

			//Images change logic
			if(Input.GetKeyUp(KeyCode.Mouse0) || currPictureShowTime >= MAX_PICTURE_SHOW_TIME)
			{
				//Insert code here to respond to releasing finger from touchpad, but you won't know how long that touch was, or if they moved around on the touchpad, etc)
				ShowNextPicture ();
			}
		}

		private void OnEmotionCompleted ()
		{
			if (lastEmotion != EmotionData.EmotionEnum.smile 
				&& lastEmotion != EmotionData.EmotionEnum.surprise 
				&& lastEmotion != EmotionData.EmotionEnum.contempt) {
				return;
			}
			
			//currPictureShowTime = MAX_PICTURE_SHOW_TIME / 2.0f;
			otherLikesTxt.text = LIKED_TXT;

			likeAnimImg.SetActive (false);
			openAnimImg.SetActive (false);
			angerAnimImg.SetActive (false);
			laughAnimImg.SetActive (false);

			likeImg.SetActive (false);
			openImg.SetActive (false);
			angerImg.SetActive (false);
			laughImg.SetActive (false);

			imageEmotion [currPictureIndex] = lastEmotion;
			isImageLiked [currPictureIndex] = true;

			if (lastEmotion == EmotionData.EmotionEnum.smile && currPictureIndex != 4) {
				likeImg.GetComponent <FbBtnAnimation> ().disableAnimation = false;
				likeImg.SetActive (true);
				likeAnimImg.SetActive (true);
			} 
			else if (lastEmotion == EmotionData.EmotionEnum.smile && currPictureIndex == 4) {
				laughImg.GetComponent <FbBtnAnimation> ().disableAnimation = false;
				laughImg.SetActive (true);
				laughAnimImg.SetActive (true);
			}
			else if (lastEmotion == EmotionData.EmotionEnum.surprise) {
				openImg.GetComponent <FbBtnAnimation> ().disableAnimation = false;
				openImg.SetActive (true);
				openAnimImg.SetActive (true);
			} 
			else if (lastEmotion == EmotionData.EmotionEnum.contempt) {
				angerImg.GetComponent <FbBtnAnimation> ().disableAnimation = false;
				angerImg.SetActive (true);
				angerAnimImg.SetActive (true);
			}
		}

		private void ShowNextPicture ()
		{
			//--------DEBUG - REMOVE Later-------------
			/*#if UNITY_EDITOR
			debugBT.SetNeutralEmotion ();
			#endif*/
			//----------------------------

			//lastEmotion = EmotionData.EmotionEmum.neutral;
			currEmotionActiveTime = 0;

			currPictureShowTime = 0;

			imageToShow [currPictureIndex].SetActive (false);
			currPictureIndex++;

			if (currPictureIndex >= imageToShow.Count) {
				currPictureIndex = 0;
			}
			imageToShow [currPictureIndex].SetActive (true);

			if (isImageLiked[currPictureIndex])
				otherLikesTxt.text = LIKED_TXT;
			else
				otherLikesTxt.text = NOT_LIKED_TEXT;

			//---
			likeImg.SetActive (false);
			openImg.SetActive (false);
			angerImg.SetActive (false);
			laughImg.SetActive (false);

			if (imageEmotion [currPictureIndex] == EmotionData.EmotionEnum.smile && currPictureIndex != 4) {
				likeImg.GetComponent <FbBtnAnimation> ().disableAnimation = true;
				likeImg.SetActive (true);
			} 
			if (imageEmotion [currPictureIndex] == EmotionData.EmotionEnum.smile && currPictureIndex == 4) {
				laughImg.GetComponent <FbBtnAnimation> ().disableAnimation = true;
				laughImg.SetActive (true);
			}
			else if (imageEmotion [currPictureIndex] == EmotionData.EmotionEnum.surprise) {
				openImg.GetComponent <FbBtnAnimation> ().disableAnimation = true;
				openImg.SetActive (true);
			} 
			else if (imageEmotion [currPictureIndex] == EmotionData.EmotionEnum.contempt) {
				angerImg.GetComponent <FbBtnAnimation> ().disableAnimation = true;
				angerImg.SetActive (true);
			}
			//---

			//--------DEBUG - REMOVE Later-------------
			/*#if UNITY_EDITOR
			Invoke ("debugInvoke", 1.0f);
			#endif*/
			//----------------------------
		}

		/*void debugInvoke ()
		{
			debugBT.ReadRandomEmotion ();
		}*/
	}
}
