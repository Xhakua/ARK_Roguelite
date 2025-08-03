using System;
/// <summary>
/// 显示生命值和魔法值的UI接口
/// </summary>
public interface ISetHealthUI
{
    public event EventHandler<OnProgressChangedEventArgs> OnHealthUIChanged;

    public class OnProgressChangedEventArgs : EventArgs
    {
        public int hp;
        public int hpMax;
        public int mp;
        public int mpMax;
    }
}

