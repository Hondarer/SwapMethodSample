using System;
using System.Runtime.CompilerServices;

namespace SealedLibrary
{
    /// <summary>
    /// 継承による機能拡張が困難なライブラリの実装サンプルを提供します。
    /// </summary>
    public class SealedClass
    {
        /// <summary>
        /// 外部からの処理要求を受け付けます。
        /// </summary>
        /// <param name="param1">パラメーター 1。</param>
        /// <param name="param2">パラメーター 2。</param>
        /// <returns>処理結果。</returns>
        public string ExecuteOperation(int param1, string param2)
        {
            return SealedLogic(param1, param2);
        }

        /// <summary>
        /// 内部の処理を模擬します。
        /// </summary>
        /// <param name="param1">パラメーター 1。</param>
        /// <param name="param2">パラメーター 2。</param>
        /// <returns>処理結果。</returns>
        /// <remarks>
        /// 実際には、インライン抑止がされているかどうかは確実でないコードのメソッドを
        /// 差し替えの対象にすることが多いと思われるが、
        /// 特に短いコードはインライン化されることが多いので
        /// 検証用コードでは属性を明示的に付与している。
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal protected string SealedLogic(int param1, string param2)
        {
            return $"This is SealedLogic ({this.GetHashCode()}), param1={param1}, param2={param2}";
        }
    }
}