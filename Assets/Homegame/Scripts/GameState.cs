using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    public static GameState instance;
    public float fadeToDeathTime = 2.0f;
    public AnimationCurve fadeToDeathColorAnimation;
    public Image screenOverlay;

    private bool inDeathSequence;

    Coroutine currentCoroutine;

    public UnityEvent OnGameStart;
    public UnityEvent OnPlayerDeath;
    public UnityEvent OnGameEnd;

    void Awake()
    {
        instance = instance == null ? this : instance;
        if(instance != this) { Destroy(gameObject); }

        TriggerFadeBirth();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [ContextMenu("Trigger Game Start")]
    public void TriggerGameStart()
    {
        OnGameStart.Invoke();
    }

    [ContextMenu("Trigger Player Death")]
    public void TriggerPlayerDeath()
    {
        TriggerFadeDeath();
        OnPlayerDeath.Invoke();
    }

    [ContextMenu("Trigger Game End")]
    public void TriggerGameEnd()
    {
        OnGameEnd.Invoke();
    }

    private void TriggerFadeBirth()
    {
        currentCoroutine = StartCoroutine(DoFadeBirthRoutine());
    }

    private void TriggerFadeDeath()
    {
        if(!inDeathSequence) { inDeathSequence = true; }
        else                 { return; }

        currentCoroutine = StartCoroutine(DoFadeDeathRoutine());
    }

    IEnumerator DoFadeDeathRoutine()
    {
        float fadeToDeathTime = 2.0f;
        float timer = 0.0f;
        Color initialColor = screenOverlay.color;
        while(true)
        {
            timer += Time.unscaledDeltaTime;
            initialColor.a = fadeToDeathColorAnimation.Evaluate(timer / fadeToDeathTime);
            screenOverlay.color = initialColor;

            if(timer >= fadeToDeathTime) { break; }
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        currentCoroutine = null;
    }

    IEnumerator DoFadeBirthRoutine()
    {
        float fadeToDeathTime = 2.0f;
        float timer = 0.0f;
        Color initialColor = screenOverlay.color;
        while(true)
        {
            timer += Time.unscaledDeltaTime;
            initialColor.a = 1 - (timer / fadeToDeathTime);
            screenOverlay.color = initialColor;

            if(timer >= fadeToDeathTime) { break; }
            yield return null;
        }

        currentCoroutine = null;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Color og = screenOverlay.color;
        og.a = 0.0f;
        screenOverlay.color = og;
    }
}
