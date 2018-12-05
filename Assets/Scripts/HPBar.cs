using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Image bar;
    public Image enbar;
    [SerializeField]
    HealthKeeper target;
    PlayerControl target2;
    public float maxWidth;
    public float mixelsInPixel;
    // Use this for initialization
    void Start()
    {
        maxWidth = bar.rectTransform.rect.width;
        mixelsInPixel = maxWidth / 26f;
    }


    public void SetTarget(HealthKeeper target) {
        this.target = target;
    }
    public void SetTarget(PlayerControl target) {
        this.target2 = target;
    }
    // Update is called once per frame
    void Update()
    {
        float pixels = Mathf.Floor(26f * ((float)target.GetHP() / target.maxHP));
        float enpixels = Mathf.Floor(26f * ((float)target2.energy / target2.maxEnergy));
        //Debug.Log(pixels);
        float width = pixels * mixelsInPixel;
        float width2 = enpixels * mixelsInPixel;
        //Debug.Log(width);

        bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        enbar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width2);
    }
}
