using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;

    public Text scoreUI;
    Spawner spawner;

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    private void OnNewWave(int waveNumver)
    {
        string[] strings = { "One", "Two", "Three", "Four", "Five" };
        newWaveTitle.text = "- Wave " + strings[waveNumver-1] + " -";
        string enemyCountString = ((spawner.waves[waveNumver - 1].infinite) ? "Infinite" : spawner.waves[waveNumver - 1].enemyCount.ToString());
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine(AnimateNewWaveBanner());
        StartCoroutine(AnimateNewWaveBanner());
    }

    private IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 1.5f;
        float speed = 3f;
        float animatePercent = 0;
        int dir = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * dir;

            if (animatePercent >= 1)
            {
                animatePercent = 1;
                if (Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }
            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-300, -100, animatePercent);
            yield return null;
        }
        
    }

    void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
    }

    private void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 3f));
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }
}
