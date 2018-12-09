using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessFX : MonoBehaviour {

    public RenderTexture bufferTexture;
    public Camera mainCamera;
    public Material postProcessEffect;
    public LayerMask targetLayer;

	// Use this for initialization
	void Start () {
        bufferTexture = new RenderTexture(new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGB32, 32));
        bufferTexture.Create();

        Camera fxCamera = GameObject.Instantiate(mainCamera);
        Destroy(fxCamera.GetComponent<PostProcessFX>());
        fxCamera.targetTexture = bufferTexture;
        fxCamera.clearFlags = CameraClearFlags.Color;
        fxCamera.backgroundColor = Color.black;
        fxCamera.cullingMask = targetLayer;
        fxCamera.transform.SetParent(mainCamera.transform);
        fxCamera.transform.localPosition = Vector3.zero;

        postProcessEffect.SetTexture("_DoodleBuffer", bufferTexture);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, postProcessEffect, 0);
    }
}
