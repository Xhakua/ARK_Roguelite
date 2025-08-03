using System;
/// <summary>
/// ��ʾ����ֵ��ħ��ֵ��UI�ӿ�
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

