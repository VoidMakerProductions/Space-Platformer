using UnityEngine;
using System.Collections;

public class GunRot : MonoBehaviour
{
    public Transform rotSource;
    
    public SpriteRenderer playerSprRen;
    SpriteRenderer Myrenderer;
    public Sprite up;
    public Sprite up_up_forward;
    public Sprite up_forward;
    public Sprite forward;
    public Sprite down_forward;
    public Sprite down_down_forward;
    public Sprite down;
    // Use this for initialization
    void Start()
    {
        Myrenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        float z = rotSource.rotation.normalized.eulerAngles.z;
        bool flipX = playerSprRen.flipX;
        if (z >= 345.0 || z <= 15.0)
        {
            Myrenderer.sprite = forward;
            Myrenderer.flipX = false;
        }
        else if (z >= 315.0 && z < 345.0) {
            Myrenderer.sprite = down_forward;
            Myrenderer.flipX = false;
        }
        else if (z >= 285.0 && z < 315.0)
        {
            Myrenderer.sprite = down_down_forward;
            Myrenderer.flipX = false;
        }
        else if (z > 255.0 && z < 285.0)
        {
            Myrenderer.sprite = down;
            Myrenderer.flipX = flipX;
        }
        else if (z <= 255.0 && z > 225.0)
        {
            Myrenderer.sprite = down_down_forward;
            Myrenderer.flipX = true;
        }
        else if (z <= 225.0 && z > 195.0)
        {
            Myrenderer.sprite = down_forward;
            Myrenderer.flipX = true;
        }
        else if (z <= 195.0 && z > 165.0)
        {
            Myrenderer.sprite = forward;
            Myrenderer.flipX = true;
        }
        else if (z <= 165.0 && z >135.0)
        {
            Myrenderer.sprite = up_forward;
            Myrenderer.flipX = true;
        }
        else if (z <= 135.0 && z > 105.0)
        {
            Myrenderer.sprite = up_up_forward;
            Myrenderer.flipX = true;
        }
        else if (z <= 105.0 && z > 75.0)
        {
            Myrenderer.sprite = up;
            Myrenderer.flipX = flipX;
        }
        else if (z <= 75.0 && z > 45.0)
        {
            Myrenderer.sprite = up_up_forward;
            Myrenderer.flipX = false;
        }
        else if (z <= 45.0 && z > 15.0)
        {
            Myrenderer.sprite = up_forward;
            Myrenderer.flipX = false;
        }


    }
}
