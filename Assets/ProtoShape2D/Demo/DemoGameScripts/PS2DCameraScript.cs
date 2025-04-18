﻿using UnityEngine;

namespace ProtoShape2D
{
	public class PS2DCameraScript : MonoBehaviour
	{
		public GameObject follow;
		public GameObject[] pxObjects;
		private Vector3[] pxPositions;
		public Vector2 startPos;
		private Vector2 diffPos;
		private bool isFullScreen = false;

		private void Start()
		{
			pxPositions = new Vector3[pxObjects.Length];
			for (int i = 0; i < pxObjects.Length; i++)
			{
				pxPositions[i] = pxObjects[i].transform.position;
			}
		}

		private void Update()
		{
			if (!Screen.fullScreen && isFullScreen)
			{
				isFullScreen = false;
				Screen.SetResolution(1280, 720, false);
			}

			if (Screen.fullScreen && !isFullScreen)
			{
				isFullScreen = true;
				Resolution biggestResolution = new Resolution();
				for (int i = 0; i < Screen.resolutions.Length; i++)
				{
					if (biggestResolution.width * biggestResolution.height <
						Screen.resolutions[i].width * Screen.resolutions[i].height)
					{
						biggestResolution = Screen.resolutions[i];
					}
				}

				Screen.SetResolution(biggestResolution.width, biggestResolution.height, true);
			}

			diffPos = startPos - (Vector2)transform.position;
			for (int i = 0; i < pxObjects.Length; i++)
			{
				pxObjects[i].transform.position = pxPositions[i] - ((Vector3)diffPos * pxPositions[i].z * 0.1f);
			}
		}

		private void FixedUpdate()
		{
			var dest = follow.transform.position;
			var dist = Vector2.Distance(transform.position, follow.transform.position);
			if (dist > 0.01f)
			{
				Vector3 target = Vector2.MoveTowards(transform.position, follow.transform.position, dist * 0.09f);
				target.z = transform.position.z;
				transform.position = target;
			}
		}
	}
}