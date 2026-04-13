using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameIntro : MonoBehaviour
{
    public Image targetImage;
    public Sprite[] images;
    public float fadeDuration = 0.3f;

    private int index = 0;
    private bool isTransitioning = false;

    public void ImageChange()
    {
        if (isTransitioning) return;
        if (images.Length == 0) return;

        index++;

       
        if (index >= images.Length)
        {
            SceneManager.LoadScene(2);
            return;
        }

        StartCoroutine(FadeToImage(images[index]));
    }

    IEnumerator FadeToImage(Sprite nextSprite)
    {
        isTransitioning = true;

        
        yield return StartCoroutine(Fade(1f, 0f));

  
        targetImage.sprite = nextSprite;


        yield return StartCoroutine(Fade(0f, 1f));

        isTransitioning = false;
    }

    IEnumerator Fade(float from, float to)
    {
        float time = 0f;
        Color color = targetImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, time / fadeDuration);
            targetImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        targetImage.color = new Color(color.r, color.g, color.b, to);
    }
}