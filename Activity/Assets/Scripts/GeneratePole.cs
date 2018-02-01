using UnityEngine;
using System.Collections.Generic;

public class GeneratePole : MonoBehaviour {

    private Texture2D _texture;
    private Vector2 centr;
    private int _tex_side = 512;
    
	void Start () {
        GenerateTexture();
        
        _texture.filterMode = FilterMode.Bilinear;
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(_texture, new Rect(0,0,512,512), new Vector2(0.5f,0.5f) );
        transform.localScale = new Vector3( 2.25f, 2.25f, 1.0f );
    }

    private void GenerateTexture() {
        _texture = new Texture2D(_tex_side, _tex_side, TextureFormat.ARGB32, false);
        centr = new Vector2(_texture.width * 0.5f, _texture.height * 0.5f);

        List<Color32> colors = new List<Color32>();

        for (int x = 0; x < _tex_side; x++)
            for (int y = 0; y < _tex_side; y++) {

                Color color = Color.white;

                if (Vector2.Distance(new Vector2(x, y), centr) > Game.Radius*45)
                {
                    color = new Color32(0,0,0,0);
                }
                else
                {
                    Vector2 pixelDirection = centr - new Vector2(x,y);
                    pixelDirection.Normalize();

                    float angle = Mathf.Atan2(pixelDirection.y, pixelDirection.x) * Mathf.Rad2Deg;

                    if (angle <= 0) angle += 360;

                    if (angle > 355) angle = 360 - angle;

                    color = Game.GetColor( angle );

                }
                colors.Add(color);
            }

        _texture.SetPixels32(colors.ToArray());
        _texture.Apply(false);
	}

}
