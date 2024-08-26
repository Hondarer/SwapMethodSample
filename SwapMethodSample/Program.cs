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
            // FIXME: ここで示している引数のソースとターゲットの関係が、プログラム全体のクラス名としてのソース、ターゲットとあべこべなので、
            //        検証コードとはいえ、きちんとリファクタリングする必要がある。解りにくい。

            // Get methods using reflection
            MethodInfo source = sourceType.GetMethod(sourceMethod, BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo target = targetType.GetMethod(targetMethod);

            // Prepare methods to get machine code
            RuntimeHelpers.PrepareMethod(source.MethodHandle);
            RuntimeHelpers.PrepareMethod(target.MethodHandle);

            // 差し替え対象のメソッドのメソッド ディスクリプターのポインターを取得
            IntPtr sourceMethodDescriptorAddress = source.MethodHandle.Value;
            // 差し替えるメソッドのポインターを取得
            IntPtr targetMethodMachineCodeAddress = target.MethodHandle.GetFunctionPointer();

            // 差し替え対象のメソッドのメソッド ディスクリプターのポインターを
            // 差し替えて置き換えるメソッドのポインターに書き換え
            // 以降、差し替え対象のメソッドが呼ばれると、差し替えて置き換えたメソッドが実行される
            // HACK: このような操作は通常行うべきではない。
            //       このコードは、差し替え対象のメソッドが存在するライブラリが変更不可能である等の状況にて
            //       他に回避策がない場合の、メソッドの実装変更のサンプルとしてのコードである。
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
