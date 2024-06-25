using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodaTime;

public class AppClock : IClock
{
    public Instant Instant;
    public ZonedDateTime ZonedDateTime;
    public Instant GetCurrentInstant()
    {
        return Instant;
    }
}
