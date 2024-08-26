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

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal string TargetMethod(int param)
        {
            return $"This is TargetMethod, param={param}";
        }
    }
}
