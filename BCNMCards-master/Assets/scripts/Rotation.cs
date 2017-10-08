using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// http://stackoverflow.com/questions/7120845/equivalent-of-tuple-net-4-for-net-framework-3-5
public class Tuple<T1, T2>
{
	public T1 First { get; private set; }
	public T2 Second { get; private set; }
	internal Tuple(T1 first, T2 second)
	{
		First = first;
		Second = second;
	}
}

public static class Tuple
{
	public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
	{
		var tuple = new Tuple<T1, T2>(first, second);
		return tuple;
	}
}
	
public class Rotation : MonoBehaviour {

	public List<GameObject> Cards;
	private string suit;
	private int value;

	private GameObject card;
	private Vector3 vector;
	private List<Tuple<Vector3, float>> SuitOrderArray;
	private List<Tuple<Vector3, float>> RandomOrderArray;
	private Vector3[] SequentialOrderArray;

//	private bool randomlySorted;
	private List<Tuple<Vector3, float>> shuffledCardsWithRemoval;

	void Start () {


		// S INPUT: Order cards by suits
		SuitOrderArray = new List<Tuple<Vector3, float>> ();
		float theta;
		float radius = 3f;
		float xPos;
		float height;
		float zPos;
		Vector3 xzPos;
		// Initialize variable used to calculate xz position
		float thetaStart = 0;

		for (int i = 0; i < 4; i++) {

			if (i == 0) {
				thetaStart = 12;	
			} else if (i == 1) {
				thetaStart = 25;
			} else if (i == 2) {
				thetaStart = 38;
			} else if (i == 3) {
				thetaStart = 51;
			}
				
			for (int j = 0; j < 13; j++) {

				// height
				if (j == 0) {
					height = -2.5f;
				} else if (j < 4) {
					height = -1.25f;
				} else if (j < 9) {
					height = 0f;
				} else if (j < 12) {
					height = 1.25f;
				} else {
					height = 2.5f;
				}

				// Calculate angle around sphere
				if (j == 1 | j == 5 | j == 9) {
					theta = (2 * Mathf.PI / 52) * (thetaStart-1f);
				} else if (j == 4) {
					theta = (2 * Mathf.PI / 52) * (thetaStart-2f);
				} else if (j == 3 | j == 7 | j == 11) {
					theta = (2 * Mathf.PI / 52) * (thetaStart+1f);
				} else if (j == 8) {
					theta = (2 * Mathf.PI / 52) * (thetaStart+2f);
				} else {
					theta = (2 * Mathf.PI / 52) * thetaStart;
				}

				// Calculate x position
				xPos = Mathf.Cos (theta) * radius;
				// Calculate z position
				zPos = Mathf.Sin (theta) * radius;
				// Add position and height to list
				xzPos = new Vector3 (xPos, 0, zPos);
				// S INPUT : includes variable height
				var suitPosAndHeight = Tuple.New(xzPos, height);
				SuitOrderArray.Add (suitPosAndHeight);

			}

		}

		// R INPUT: Initialize list to store positions and heights
		RandomOrderArray = new List<Tuple<Vector3, float>> ();
		// Initialize variables used to calculate position.
		theta = 0;
		radius = 3f;
		xPos = 3;
		height = 0;
		zPos = 0;
		xzPos = new Vector3 (xPos, height, zPos);
		var posAndHeight = Tuple.New (xzPos, height);
		// Add first card's position and height to list
		RandomOrderArray.Add (posAndHeight);

		// N INPUT : Initialize list to store positions
		SequentialOrderArray = new Vector3[52];
		SequentialOrderArray[0] = xzPos;

		// CREATE ARRAYS FOR N AND R INPUTS, INITIALIZE CARDS WITH RANDOM ORDER AND HEIGHT
		for (int i = 0; i < 52; i++) {

			// Calculate angle around sphere
			theta = (2 * Mathf.PI / 52) * i;
			// Calculate x position
			xPos = Mathf.Cos (theta) * radius;
			// Calculate y position
			height = Random.Range(-2.5f, 2.5f);
			// Calculate z position
			zPos = Mathf.Sin (theta) * radius;
			// Add position and height to list
			xzPos = new Vector3 (xPos, 0, zPos);
			// S INPUT : includes variable height
			posAndHeight = Tuple.New(xzPos, height);
			RandomOrderArray.Add (posAndHeight);

			// N INPUT : without height
			SequentialOrderArray[i] = xzPos;

			// scale
			Cards[i].transform.localScale = new Vector3 (10, 10, 10);
			// face camera
			Cards[i].transform.LookAt (Vector3.zero);
			// turn 180 so front of card face camera
			Cards[i].transform.RotateAround (Vector3.zero, Vector3.up, 180);
			// apply height
			Cards[i].transform.position = new Vector3(xPos, height, zPos);
		}

//		randomlySorted = false;

	}
		
	// Update is called once per frame
	void Update () {

		float x = Input.GetAxis ("Mouse X");
		float y = Input.GetAxis ("Mouse Y");

		// swipe down -- random order
		if ((y < 0 && x > -0.5f && x < 0.5f) || Input.GetKeyUp (KeyCode.R)) {

			RandomOrder ();

		}
		//swipe left -- suit order
		if ((x < 0 && y > -0.5f && y < 0.5f) || Input.GetKeyUp (KeyCode.S)) {

			SuitOrder ();

		}
		//swipe right -- sequential order
		if ((x > 0 && y > -0.5f && y < 0.5f) || Input.GetKeyUp (KeyCode.N)) {

			SequentialOrder ();

		}
		// rotate cards around sphere in editor
		if (Input.GetKey (KeyCode.Space)) {
			RotateAllCardsAroundSphere ();
		}
	}

	// Suit Order, at every 90 degrees
	void SuitOrder() {
		
		// find first card
		// get angle and height of first card
		Vector3 xzPos = SuitOrderArray [0].First;
		float height = SuitOrderArray [0].Second;
		// Move position
		Cards[0].transform.position = xzPos;
		float xPos = Cards[0].transform.position.x;
		float zPos = Cards[0].transform.position.z;
		iTween.MoveTo (Cards[0], new Vector3 (xPos, height, zPos), 5);
		// Face camera
		Cards[0].transform.LookAt (Vector3.zero);
		// Turn 180 so front of card is facing camera
		Cards[0].transform.RotateAround (Vector3.zero, Vector3.up, 180);


		for (int i = 1; i < 52; i++) {

			xzPos = SuitOrderArray [i].First;
			height = SuitOrderArray [i].Second;

			// Move x and z positions
			Cards[i].transform.position = xzPos;
			xPos = Cards[i].transform.position.x;
			zPos = Cards[i].transform.position.z;
			iTween.MoveTo (Cards[i], new Vector3 (xPos, height, zPos), 5);
			// Face camera
			Cards[i].transform.LookAt (Vector3.zero);
			// Turn 180 so front of card is facing camera
			Cards[i].transform.RotateAround (Vector3.zero, Vector3.up, 180);

		}
	}

	// Sequential Order, Same Height
	void SequentialOrder() {

		// find first card
		// get angle and height of first card
		Vector3 xzPos = SequentialOrderArray [0];

		// Move position
		Cards[0].transform.position = xzPos;
		float xPos = Cards[0].transform.position.x;
		float zPos = Cards[0].transform.position.z;
		iTween.MoveTo (Cards[0], new Vector3 (xPos, 0, zPos), 5);

		// Face camera
		Cards[0].transform.LookAt (Vector3.zero);
		// Turn 180 so front of card is facing camera
		Cards[0].transform.RotateAround (Vector3.zero, Vector3.up, 180);

		for (int i = 1; i < 52; i++) {

			xzPos = SequentialOrderArray [i];

			// Move x and z positions
			Cards[i].transform.position = xzPos;
			xPos = Cards[i].transform.position.x;
			zPos = Cards[i].transform.position.z;
			iTween.MoveTo (Cards[i], new Vector3 (xPos, 0, zPos), 5);
			// Face camera
			Cards[i].transform.LookAt (Vector3.zero);
			// Turn 180 so front of card is facing camera
			Cards[i].transform.RotateAround (Vector3.zero, Vector3.up, 180);

		}
	}

	// Random Order, Random Height
	void RandomOrder() {

		// shuffle cards into random order -- working
		shuffledCardsWithRemoval = RandomOrderArray.OrderBy (a => System.Guid.NewGuid ()).ToList ();;

		// find first card
		// get angle and height of first card
		Vector3 xzPos = shuffledCardsWithRemoval [0].First;
		float height = Random.Range (-2.5f, 2.5f);
		shuffledCardsWithRemoval.RemoveAt (0);

		// Move position
		Cards[0].transform.position = xzPos;
		float xPos = Cards[0].transform.position.x;
		float zPos = Cards[0].transform.position.z;
		iTween.MoveTo (Cards[0], new Vector3 (xPos, height, zPos), 5);

		// Face camera
		Cards[0].transform.LookAt (Vector3.zero);
		// Turn 180 so front of card is facing camera
		Cards[0].transform.RotateAround (Vector3.zero, Vector3.up, 180);

		for (int i = 1; i < 52; i++) {

			xzPos = shuffledCardsWithRemoval [0].First;
			height = Random.Range (-2.5f, 2.5f);
			shuffledCardsWithRemoval.RemoveAt (0);

			// Move x and z positions
			Cards[i].transform.position = xzPos;
			xPos = Cards[i].transform.position.x;
			zPos = Cards[i].transform.position.z;
			iTween.MoveTo (Cards[i], new Vector3 (xPos, height, zPos), 5);
			// Face camera
			Cards[i].transform.LookAt (Vector3.zero);
			// Turn 180 so front of card is facing camera
			Cards[i].transform.RotateAround (Vector3.zero, Vector3.up, 180);

		}
	}

	// Rotate all cards around sphere
	void RotateAllCardsAroundSphere() {

		Cards[0].transform.RotateAround (Vector3.zero, Vector3.up, (float)1);
		for (int i = 1; i < 52; i++) {

			Cards[i].transform.RotateAround (Vector3.zero, Vector3.up, (float)1);

		}	
	}
	
}
