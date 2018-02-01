using UnityEngine;

public class SpriteAnimator : MonoBehaviour {

    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private float delay;
    [SerializeField]
    private bool play = true;

    private float timer = 0.0f;
    private uint curSpriteIndex = 0;

    private SpriteRenderer sr;

	void Start () {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprites[curSpriteIndex];
	}

	void Update () {
        if (!play) return;

        timer += Time.deltaTime;

        if (timer >= delay)
        {
            curSpriteIndex++;
            if (curSpriteIndex >= sprites.Length) curSpriteIndex = 0;

            sr.sprite = sprites[curSpriteIndex];

            timer = 0.0f;
        }
	}

    public void GoToStart()
    {
        sr.sprite = sprites[0];
    }

    public void Play()
    {
        play = true;
    }

    public void Stop()
    {
        play = false;
    }

    public void FlipX(bool flag)
    {
        sr.flipX = flag;
    }

    public void FlipY(bool flag)
    {
        sr.flipY = flag;
    }

}
