using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerScore : MonoBehaviour {

    [SerializeField]
    Text scoreText;

    [SerializeField]
    float scorePerSecond;

    int numColliders;
    float score;

    HashSet<Collider> colliders;

    void Start () {
        score = 0;
        colliders = new HashSet<Collider>();
	}
	
	void Update () {
        foreach(Collider c in colliders)
        {
            Vector3 closestPoint = c.ClosestPointOnBounds(transform.position);
            float dist = Vector3.Distance(closestPoint, transform.position);
            score += scorePerSecond + Mathf.Clamp(3 / dist, 0, 100) * numColliders * Time.deltaTime;
        }
        scoreText.text = score.ToString();
	}


    void OnTriggerEnter(Collider c)
    {
        colliders.Add(c);
        numColliders++;
    }

    void OnTriggerExit(Collider c)
    {
        colliders.Remove(c);
        numColliders--;
    }
}
