using UnityEngine;
using System.Collections;

public class CrossHairs : MonoBehaviour {

	bool isDead;
	public LayerMask targetMask;
	public SpriteRenderer dot;
	public Color dotHighlightColor;
	Color originalDotColor;

	// Use this for initialization
	void Start () {
		isDead = false;
		FindObjectOfType<PlayerInput> ().OnDeath += Death;

		originalDotColor = dot.color;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.forward * 40 * Time.deltaTime);
		if (!isDead) {
			Cursor.visible = false;
		} else {
			Cursor.visible = true;
		}

	}

	public void DetectTargets(Ray ray){
		if (Physics.Raycast (ray, 100, targetMask)) {
			dot.color = dotHighlightColor;
		} else {
			dot.color = originalDotColor;
		}
	}

	public void Death(){
		isDead = true;
	}
}
