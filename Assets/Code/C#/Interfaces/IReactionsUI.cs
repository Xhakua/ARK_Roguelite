using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 显示反应伤害变化的接口
/// </summary>
public interface IReactionsUI 
{
    public event EventHandler<OnBuffChangedEventArgs> OnBuffChanged;
    public class OnBuffChangedEventArgs : EventArgs
    {
        public ReactionsBuff buff;
    }
}
