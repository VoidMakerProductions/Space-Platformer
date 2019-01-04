using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScafanderModelShow : MonoBehaviour
{
    public Sprite[] sprites;
    public Image image;
    int index = 0;
    // Use this for initialization

    private void Start()
    {
        if (PlayerPrefs.HasKey("playerSprite")){
            index = PlayerPrefs.GetInt("playerSprite");
            image.sprite = sprites[index];
        }
    }
    public void Cycle() {
        index++;
        if (index >= sprites.Length) {
            index = 0;
        }
        image.sprite = sprites[index];

        PlayerPrefs.SetInt("playerSprite",index);
    }



}
