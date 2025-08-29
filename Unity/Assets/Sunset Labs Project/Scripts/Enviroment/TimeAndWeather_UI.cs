using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Globalization;

public class TimeAndWeather_UI : MonoBehaviour
{
    private DateTime lastDate;
    private CultureInfo culture;
    private WaitForSeconds waitForSeconds;

    private Coroutine tickCoroutine;
    private const string timeFormat = "HH:mm:ss";
    private const string dateFormat = "dd MMMM yyyy";
   
    [Header("Sprite Parameters")]
    [SerializeField] private Sprite[] weatherIcons;
    [SerializeField] private string cultureName = "en-US"; // Change to desired culture

    [Header("UI Parameters")]
    [SerializeField] private Image weatherIcon;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dateText;

    private void Awake()
    {
        waitForSeconds = new(1f);
        culture = new CultureInfo(cultureName);
    }

    private void OnEnable()
    {
        tickCoroutine = StartCoroutine(SecondTick());
    }

    private void OnDisable()
    {
        if(tickCoroutine != null)
        {
            StopCoroutine(tickCoroutine);
            tickCoroutine = null;
        }
    }

    private IEnumerator SecondTick()
    {
        while(true)
        {
            yield return waitForSeconds;
            timeText.text = DateTime.Now.ToString(timeFormat, culture);
            if (lastDate.Date != DateTime.Now.Date)
            {
                lastDate = DateTime.Now;
                dateText.text = lastDate.ToString(dateFormat, culture);
            }
        }
    }
}
