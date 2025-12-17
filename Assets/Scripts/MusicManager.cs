using UnityEngine;
using UnityEngine.UI; // <-- You MUST add this line to use UI elements!

public class MusicManager : MonoBehaviour
{
    // Audio
    public AudioSource musicSource;

    // --- New variables for Sprite Swap ---
    [Tooltip("The Image component on your button.")]
    public Image buttonImage;

    [Tooltip("The Sprite to show when music is playing.")]
    public Sprite musicOnSprite;

    [Tooltip("The Sprite to show when music is muted.")]
    public Sprite musicOffSprite;

    // This makes sure the button shows the correct icon when the game loads.
    void Start()
    {
        // Check the initial state of the music
        if (musicSource.mute)
        {
            buttonImage.sprite = musicOffSprite; // Set to OFF sprite
        }
        else
        {
            buttonImage.sprite = musicOnSprite; // Set to ON sprite
        }
    }

    // This is the public function the button calls
    public void ToggleMusic()
    {
        // Toggle the mute state
        musicSource.mute = !musicSource.mute;

        // Now, check the new state and swap the sprite
        if (musicSource.mute)
        {
            // Music is OFF (muted)
            buttonImage.sprite = musicOffSprite;
        }
        else
        {
            // Music is ON (not muted)
            buttonImage.sprite = musicOnSprite;
        }
    }
}