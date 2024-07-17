using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeoTimeZone;
using NodaTime;
using TimeZoneConverter;
using TMPro;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public double Lst;
    public LocationManager LocationManager;
    private Instant _j2000 = Instant.FromUtc(2000, 1, 1, 0, 0);
    private Instant _appTimeInstant;
    public ZonedDateTime AppTime;
    public Instant AppTimeInstant
    {
        get => _appTimeInstant;
        set
        {
            _appTimeInstant = value;
            AppTime = new ZonedDateTime(value, DateTimeZone);
            YearUI.text = $"{AppTime.Year}";
            MonthUI.text = $"{AppTime.Month}";
            DayUI.text = $"{AppTime.Day}";
            HourUI.text = $"{AppTime.Hour}";
            MinuteUI.text = $"{AppTime.Minute}";
            SecondUI.text = $"{AppTime.Second}";
        }
    }
    public DateTimeZone DateTimeZone;

    [Header("UI")]
    public TMP_InputField YearUI;
    public TMP_InputField MonthUI;
    public TMP_InputField DayUI;
    public TMP_InputField HourUI;
    public TMP_InputField MinuteUI;
    public TMP_InputField SecondUI;

    // to test only
    public UnityEvent changeTimeEvent;

    public void Awake()
    {
        DateTimeZone = DateTimeZone.Utc;
        ConvertTimeByLocation(-6.914744f, 107.609810f);
        AppTimeInstant = SystemClock.Instance.GetCurrentInstant();
        Lst = SetLst(LocationManager.longitude);
    }

    public void Update()
    {
        bool yearUISelected = YearUI.isFocused;
        bool monthUISelected = MonthUI.isFocused;
        bool dayUISelected = DayUI.isFocused;
        bool hourUISelected = HourUI.isFocused;
        bool minuteUISelected = MinuteUI.isFocused;
        bool secondUISelected = SecondUI.isFocused;
        if (yearUISelected || monthUISelected || dayUISelected || hourUISelected || minuteUISelected || secondUISelected) { }
        else
        {
            AppTimeInstant = AppTimeInstant.Plus(Duration.FromSeconds((double)Time.deltaTime));
        }
        Lst = SetLst(LocationManager.longitude);
    }

    private double SetLst(float lon)
    {
        //DateTimeOffset timeUtc = AppTime.UtcDateTime;
        //double d = (AppTime - _j2000).TotalDays;
        DateTime timeUtc = AppTimeInstant.ToDateTimeUtc();
        double d = (AppTimeInstant - _j2000).TotalDays;
        double lst = 100.46 + 0.985647 * d + lon + 15 * ((double)timeUtc.Hour + (double)timeUtc.Minute / 60 + (double)timeUtc.Second / 3600);
        while (lst < 0) lst += 360;
        lst %= 360;
        return lst;
    }
    public void ConvertTimeByLocation(float lat, float lon)
    {
        string tzIana = TimeZoneLookup.GetTimeZone(lat, lon).Result;
        //TimeZoneInfo tzInfo = TZConvert.GetTimeZoneInfo(tzIana);
        //AppTime = TimeZoneInfo.ConvertTime(AppTime, tzInfo);
        ZonedDateTime zonedDateTime = new ZonedDateTime(AppTimeInstant, DateTimeZone);
        DateTimeZone = DateTimeZoneProviders.Tzdb[tzIana];
        zonedDateTime = zonedDateTime.WithZone(DateTimeZone);
        AppTimeInstant = zonedDateTime.ToInstant();
        
        Lst = SetLst(lon);
        Debug.Log($"tzIana : {tzIana}");
    }

    public void SetYear(string yearString)
    {
        int year = int.Parse(yearString);
        LocalDateTime localDateTime = AppTime.LocalDateTime;
        int diff = year - AppTime.Year;
        localDateTime = localDateTime.PlusYears(diff);

        AppTime = new ZonedDateTime(localDateTime, DateTimeZone, AppTime.Offset);
        AppTimeInstant = AppTime.ToInstant();
    }

    public void SetMonth(string monthString)
    {
        int month = int.Parse(monthString);
        LocalDateTime localDateTime = AppTime.LocalDateTime;
        int diff = month - AppTime.Month;
        localDateTime = localDateTime.PlusMonths(diff);

        AppTime = new ZonedDateTime(localDateTime, DateTimeZone, AppTime.Offset);
        AppTimeInstant = AppTime.ToInstant();
    }

    public void SetDay(string dayString)
    {
        int day = int.Parse(dayString);
        LocalDateTime localDateTime = AppTime.LocalDateTime;
        int diff = day - AppTime.Day;
        localDateTime = localDateTime.PlusDays(diff);
        
        AppTime = new ZonedDateTime(localDateTime, DateTimeZone, AppTime.Offset);
        AppTimeInstant = AppTime.ToInstant();
    }

    public void SetHour(string hourString)
    {
        int hour = int.Parse(hourString);
        int diff = hour - AppTime.Hour;
        AppTime = AppTime.PlusHours(diff);
        AppTimeInstant = AppTime.ToInstant();
        // TO DELETE
        Debug.Log($"Hour is changed to {AppTime.Hour}");
        changeTimeEvent.Invoke();

    }

    public void SetMinute(string minuteString)
    {
        int minute = int.Parse(minuteString);
        int diff = minute - AppTime.Minute;
        AppTime = AppTime.PlusMinutes(diff);
        AppTimeInstant = AppTime.ToInstant();
    }

    public void SetSecond(string secondString)
    {
        int second = int.Parse(secondString);
        int diff = second- AppTime.Second;
        AppTime = AppTime.PlusSeconds(diff);
        AppTimeInstant = AppTime.ToInstant();
    }

    public void ResetTime()
    {
        AppTimeInstant = SystemClock.Instance.GetCurrentInstant();
    }
}
