using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��ʾ��Ӧ�˺��仯�Ľӿ�
/// </summary>
public interface IReactionsUI 
{
    public event EventHandler<OnBuffChangedEventArgs> OnBuffChanged;
    public class OnBuffChangedEventArgs : EventArgs
    {
        public ReactionsBuff buff;
    }
}
