using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TargetLibrary;

namespace SwapMethodSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HijackMethod(typeof(TargetClass), "TargetMethod", typeof(Program), nameof(InstanceStringHijacked));

            new TargetClass().CallTargetMethod(1234);

            Console.ReadLine();
        }

        // https://www.infoq.com/jp/articles/overriding-sealed-methods-c-sharp/

        public static void HijackMethod(Type sourceType, string sourceMethod, Type targetType, string targetMethod)
        {
            // Get methods using reflection
            MethodInfo source = sourceType.GetMethod(sourceMethod,System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            MethodInfo target = targetType.GetMethod(targetMethod);

            // Prepare methods to get machine code
            RuntimeHelpers.PrepareMethod(source.MethodHandle);
            RuntimeHelpers.PrepareMethod(target.MethodHandle);

            IntPtr sourceMethodDescriptorAddress = source.MethodHandle.Value;
            IntPtr targetMethodMachineCodeAddress = target.MethodHandle.GetFunctionPointer();

            // Pointer is two pointers from the beginning of the method descriptor
            Marshal.WriteIntPtr(sourceMethodDescriptorAddress, IntPtr.Size * 2, targetMethodMachineCodeAddress);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string InstanceStringHijacked(object target,int param)
        {
            return $"Instance string hijacked, param={param}";
        }
    }
}
