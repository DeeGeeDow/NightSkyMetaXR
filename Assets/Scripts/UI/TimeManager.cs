using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeoTimeZone;
using TimeZoneConverter;

public class TimeManager : MonoBehaviour
{
    public double Lst;
    public LocationManager LocationManager;
    private DateTimeOffset _j2000 = new DateTimeOffset(2000, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
    public DateTimeOffset AppTime;

    public void Awake()
    {
        AppTime = DateTimeOffset.UtcNow;
        ConvertTimeByLocation(-6.914744f, 107.609810f);
        Lst = SetLst(LocationManager.longitude);
    }

    public void Update()
    {
        AppTime = AppTime.AddSeconds(Time.deltaTime);
        Lst = SetLst(LocationManager.longitude);
    }

    private double SetLst(float lon)
    {
        DateTimeOffset timeUtc = AppTime.UtcDateTime;
        double d = (timeUtc - _j2000).TotalDays;
        double lst = 100.46 + 0.985647 * d + lon + 15 * ((double)timeUtc.Hour + (double)timeUtc.Minute / 60 + (double)timeUtc.Second / 3600);
        while (lst < 0) lst += 360;
        lst %= 360;
        Debug.Log(AppTime);
        return lst;
    }
    public void ConvertTimeByLocation(float lat, float lon)
    {
        string tzIana = TimeZoneLookup.GetTimeZone(lat, lon).Result;
        TimeZoneInfo tzInfo = TZConvert.GetTimeZoneInfo(tzIana);
        AppTime = TimeZoneInfo.ConvertTime(AppTime, tzInfo);
        Lst = SetLst(lon);
        Debug.Log($"tzIana : {tzIana}");
        Debug.Log($"tzInfo : {tzInfo}");
    }

    public void SetYear(string yearString)
    {
        int year = int.Parse(yearString);
        AppTime = new DateTimeOffset(year, AppTime.Month, AppTime.Day, AppTime.Hour, AppTime.Minute, AppTime.Second, AppTime.Offset);
    }

    public void SetMonth(string monthString)
    {
        int month = int.Parse(monthString);
        AppTime = new DateTimeOffset(AppTime.Year, month, AppTime.Day, AppTime.Hour, AppTime.Minute, AppTime.Second, AppTime.Offset);
    }

    public void SetDay(string dayString)
    {
        int day = int.Parse(dayString);
        AppTime = new DateTimeOffset(AppTime.Year, AppTime.Month, day, AppTime.Hour, AppTime.Minute, AppTime.Second, AppTime.Offset);
    }

    public void SetHour(string hourString)
    {
        int hour = int.Parse(hourString);
        AppTime = new DateTimeOffset(AppTime.Year, AppTime.Month, AppTime.Day, hour, AppTime.Minute, AppTime.Second, AppTime.Offset);
    }

    public void SetMinute(string minuteString)
    {
        int minute = int.Parse(minuteString);
        AppTime = new DateTimeOffset(AppTime.Year, AppTime.Month, AppTime.Day, AppTime.Hour, minute, AppTime.Second, AppTime.Offset);
    }

    public void SetSecond(string secondString)
    {
        int second = int.Parse(secondString);
        AppTime = new DateTimeOffset(AppTime.Year, AppTime.Month, AppTime.Day, AppTime.Hour, AppTime.Minute, second, AppTime.Offset);
    }
}
