using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindEffectToggle : MonoBehaviour {

    [Tooltip("The rewind effect shader")]
    public Shader theRewindEffectShader;

    private Material theRewindEffectMaterial;

    [Tooltip("Debug this effect (apply it at all times)")]
    public bool doDebug = false;


    private void Awake()
    {
        theRewindEffectMaterial = new Material(theRewindEffectShader);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (SceneManager.Instance.IsRewinding || doDebug)
        {
            Graphics.Blit(source, destination, theRewindEffectMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
