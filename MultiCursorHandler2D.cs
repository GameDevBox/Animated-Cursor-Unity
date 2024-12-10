using UnityEngine;

[System.Serializable]
public class CursorAnimation2D
{
    public string name; // Name for clarity (optional)
    public Texture2D[] cursorFrames; // Animation frames for the cursor
    public float frameRate = 0.1f; // Animation speed
    public string[] objectTags; // Specify object tags to customize further (optional)
}

public class MultiCursorHandler2D : MonoBehaviour
{
    public CursorAnimation2D[] animations; // Assign animations for different layers/objects
    public LayerMask interactableLayers; // Specify which layers trigger cursors

    private int currentFrame;
    private float timer;
    private bool isHovering;
    private CursorAnimation2D currentAnimation;

    void Update()
    {
        // Perform a raycast from the mouse position
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, interactableLayers);

        if (hit.collider != null)
        {
            // Check for specific tags
            string objectTag = hit.collider.gameObject.tag;
            currentAnimation = GetAnimationForObject(objectTag);

            if (currentAnimation != null)
            {
                if (!isHovering)
                {
                    isHovering = true; // Start animation
                    currentFrame = 0; // Reset to the first frame
                    Cursor.SetCursor(currentAnimation.cursorFrames[currentFrame], Vector2.zero, CursorMode.Auto);
                }

                AnimateCursor(currentAnimation);
            }
        }
        else
        {
            if (isHovering)
            {
                isHovering = false;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // Reset to default cursor
            }
        }
    }

    void AnimateCursor(CursorAnimation2D animation)
    {
        timer += Time.deltaTime;
        if (timer >= animation.frameRate)
        {
            timer -= animation.frameRate;
            currentFrame = (currentFrame + 1) % animation.cursorFrames.Length;
            Cursor.SetCursor(animation.cursorFrames[currentFrame], Vector2.zero, CursorMode.Auto);
        }
    }

    CursorAnimation2D GetAnimationForObject(string tag)
    {
        // Check if a specific tag matches any cursor animation
        for (int i = 0; i < animations.Length; i++)
        {
            for(int j = 0; j < animations[i].objectTags.Length; j++)
            {
                if (animations[i].objectTags[j] == tag)
                    return animations[i];
            }
        }

        return null; // No animation for the given tag
    }
}
