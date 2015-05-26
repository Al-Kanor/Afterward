using UnityEngine;
using System.Collections;

public class camtest : MonoBehaviour {
	public Animation anim;
	public float alarm;
	public float slowtime;

	void Update () {
		if (Input.GetMouseButton(0)) { 
			anim = GetComponent<Animation> ();
			anim.Play ();
		}
	}

//	IEnumerator Start() {
//		alarm = slowtime;
//	if (Input.GetMouseButton(0)) { 
//			anim = GetComponent<Animation> ();
//			anim.Play (anim.clip.name);
//			yield return new WaitForSeconds (anim.clip.length);
//		}
//	}
}
