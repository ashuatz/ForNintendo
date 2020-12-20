using UnityEngine;
using System.Collections;
using System;
using Util;

public class DataContainer : Singleton<DataContainer>
{
    public NotifierClass<TestPlayer> Player = new NotifierClass<TestPlayer>();

    public NotifierClass<ProgressiveBarUI> ProgressBarUI = new NotifierClass<ProgressiveBarUI>();


}
