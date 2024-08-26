using System;
using System.Runtime.CompilerServices;

namespace TargetLibrary
{
    public class TargetClass
    {
        public void CallTargetMethod(int param)
        {
            Console.WriteLine(TargetMethod(param));
        }

        // 実際には、インライン抑止がされているかどうかは確実でないコードのメソッドを
        // 差し替えの対象にすることが多いと思われるが、
        // 特に短いコードはインライン化されることが多いので
        // 検証用コードでは属性を明示的に付与している。
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal string TargetMethod(int param)
        {
            return $"This is TargetMethod, param={param}";
        }
    }
}
