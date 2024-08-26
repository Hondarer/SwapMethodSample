using SealedLibrary;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// NOTE: ngen で最適化された後も、このコードは動作するのか?

namespace SwapMethodSample
{
    internal class Program
    {
        /// <summary>
        /// プログラムのエントリー ポイントを定義します。
        /// </summary>
        /// <param name="args">引数。</param>
        static void Main(string[] args)
        {
            SealedClass _sealedInstance = new SealedClass();
            Console.WriteLine($"Hash of sealedInstance is {_sealedInstance.GetHashCode()}");

            // 実際に処理が呼び出される前に、対処を行う必要がある。
            HijackMethod(typeof(SealedClass), "SealedLogic", typeof(Program), nameof(RevisedLogic));

            Console.WriteLine($"{_sealedInstance.ExecuteOperation(1234, "ABCD")}");

            Console.ReadLine();
        }

        /// <summary>
        /// 内部の処理を置き換えます。
        /// </summary>
        /// <param name="target">対象のインスタンス。</param>
        /// <param name="param1">パラメーター 1。</param>
        /// <param name="param2">パラメーター 2。</param>
        /// <returns>処理結果。</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string RevisedLogic(object target, int param1, string param2)
        {
            param1 *= 2;
            return $"This is RevisedLogic ({target.GetHashCode()}), param1={param1}, param2={param2}";
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
    }
}
