using UnityEngine;
using System.Collections;

public class camerashake : MonoBehaviour {

		// Transform of the camera to shake. Grabs the gameObject's transform
		// if null.
		Transform camTransform;
		
		// How long the object should shake for.
		public float shakeDuration = 0.4f;
		
		// Amplitude of the shake. A larger value shakes the camera harder.
		public float shakeAmount = 0.1f;
		
		//Vector3 originalPos;
		
		void Awake()
		{
			if (camTransform == null)
			{
				camTransform = GetComponent(typeof(Transform)) as Transform;
			}
		}
		
		void OnEnable()
		{
			//originalPos = camTransform.localPosition;
		}

        void FixedUpdate () {
            if (isShaked) {
                Shake ();
            }
        }

        void Shake () {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
        }

		IEnumerator StartShake ()
		{
            isShaked = true;
            originalPos = camTransform.localPosition;
            yield return new WaitForSeconds (shakeDuration);
            isShaked = false;
            camTransform.localPosition = originalPos;
		}

        bool isShaked = false;
        Vector3 originalPos;
	}
