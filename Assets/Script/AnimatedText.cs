using System.Collections;
using UnityEngine;
using TMPro;

public class AnimatedText : MonoBehaviour
{
    public float delay = 0.1f; // Delay between each character
    public float fadeTime = 1f; // Time it takes for the text to fade out
    public AudioSource audioSource;

    private TMP_Text textComponent;
    private string[] texts = { "WELCOME TO DRONE LIFE SO LET'S FLY" }; // Array to hold the texts
    private int currentTextIndex = 0; // To keep track of which text to show
    private float timer;

    void Start()
    {
        // Ensure the TMP_Text component is obtained
        textComponent = GetComponent<TMP_Text>();

        // Ensure the text is cleared and animation starts
        textComponent.text = "";
        currentTextIndex = 0;
        StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        while (currentTextIndex < texts.Length)
        {
            string originalText = texts[currentTextIndex];
            textComponent.text = "";
            timer = 0f;

            while (true)
            {
                // Add characters to the text every delay seconds
                timer += Time.deltaTime;
                if (timer >= delay && textComponent.text.Length < originalText.Length)
                {
                    // Add the next character to the text
                    textComponent.text += originalText[textComponent.text.Length];

                    // Reset the timer
                    timer = 0f;
                }

                // Wait until the text is fully displayed
                if (textComponent.text.Length >= originalText.Length)
                {
                    yield return new WaitForSeconds(fadeTime); // Wait for a while before fading out
                    StartCoroutine(FadeOut()); // Start fading out
                    break; // Exit the loop after starting the fade out
                }

                yield return null; // Wait until the next frame
            }

            // Move to the next text
            currentTextIndex++;
        }
    }

    public void SkipButton()
    {
        // Skip to the end of the text immediately
        StopAllCoroutines(); // Stop any running coroutines
        textComponent.text = texts[currentTextIndex - 1]; // Show the last text fully
    }

    private IEnumerator FadeOut()
    {
        // Fade out the text over fadeTime seconds
        float startTime = Time.time;
        while (Time.time < startTime + fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1f - t);
            yield return null;
        }
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1f); // Ensure text is fully transparent
    }
}
