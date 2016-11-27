using UnityEngine;
using System.Collections;

public class GoPointer : MonoBehaviour {
    private LineRenderer _lineRenderer;

    // Use this for initialization
    void Start () {
        _lineRenderer = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position + transform.forward * 3);
    }
}
