﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Image bar;
    public Sprite whiteline;
    public float whiteDuration=0.2f;
    float changeback;
    int prevHP;
    public Image enbar;
    public Text MoneyCounter;
    
    HealthKeeper target;
    PlayerControl target2;
    float maxWidth;
    float mixelsInPixel;
    Sprite sprite;
    // Use this for initialization
    void Start()
    {
        maxWidth = bar.rectTransform.rect.width;
        mixelsInPixel = maxWidth / 26f;
        sprite = bar.sprite;
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
        if (Time.time >= changeback) {
            bar.sprite = sprite;
        }
        if (target.GetHP() < prevHP) {
            bar.sprite = whiteline;
            changeback = Time.time + whiteDuration;
        }
        float pixels = Mathf.Floor(26f * ((float)target.GetHP() / target.maxHP));
        if (target.GetHP() > 0 && pixels == 0)
            pixels = 1;
        float enpixels = Mathf.Floor(26f * ((float)target2.energy / target2.maxEnergy));
        //Debug.Log(pixels);
        float width = pixels * mixelsInPixel;
        float width2 = enpixels * mixelsInPixel;
        //Debug.Log(width);
        if (target2.CollectedAspirin < 10)
        {
            MoneyCounter.text = ":0" + target2.CollectedAspirin;
        }
        else {
            MoneyCounter.text = ":" + target2.CollectedAspirin;
        }
        bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        enbar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width2);
        prevHP = target.GetHP();
    }
}
