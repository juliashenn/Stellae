using System.Collections;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    private Coroutine setMatColor;
    public Material material;
    public float transitionTime = 1f;
    public Color newColor;
    public Color startColor;
    private bool colorChanged = false;

    private void Awake()
    {
        material.color = startColor;
        //StartCoroutine(wait(3.0f));
        //changeColor();
    }

    private IEnumerator wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
    public void changeColor()
    {
        Debug.Log("color changed");
        if (colorChanged) return;
        setMatColor = StartCoroutine(SetMatColor());
        colorChanged = true;
    }
    private IEnumerator SetMatColor()
    {
        float timer = 0f;
        while ( timer < transitionTime )
        {
            timer += Time.deltaTime;
            float t = timer / transitionTime;

            material.color = Color.Lerp(startColor, newColor, t);

            yield return null;
        }
        material.color = newColor;
    }
    
}
