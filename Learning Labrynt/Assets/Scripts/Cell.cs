using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    float shade = 0;       

    void Start()
    {
        //Reference to sprite color
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void MarkTrail()
    {        
        spriteRenderer.color = Color.Lerp(new Color(0.5f,1.0f,0.5f), new Color(0.01f, 0.2f, 0.01f), Mathf.Clamp(shade, 0.0f, 1.0f));
        shade += 0.3f;
    }   

    public void ClearTrail()
    {
        spriteRenderer.color = Color.white;
        shade = 0;
    }
}
