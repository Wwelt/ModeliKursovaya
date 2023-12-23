using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Windows;

public class ImageToSprite : MonoBehaviour
{
    private static ImageToSprite _instance;
 
    public static ImageToSprite instance
    {
        get    
        {
            if(_instance == null)
                _instance = GameObject.FindObjectOfType<ImageToSprite>();
            return _instance;
        }
    }
    
    
    public Sprite LoadNewSprite(byte[] bytes, float PixelsPerUnit = 100.0f) {
        Texture2D SpriteTexture = LoadTexture(bytes);
 
        return Sprite.Create(SpriteTexture, 
            new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),
            new Vector2(0,0), PixelsPerUnit);
    }

    public Texture2D LoadTexture(byte[] byteArray)
    {
        Texture2D Tex2D = new Texture2D(0, 0); 
        if (Tex2D.LoadImage(byteArray))          
            return Tex2D; 
        return null;  
    }
}
