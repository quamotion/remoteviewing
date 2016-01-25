using System.Runtime.InteropServices;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// Provides access to native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Compares two blocks of memory.
        /// </summary>
        /// <param name="b1">
        /// The first block of memory.
        /// </param>
        /// <param name="b2">
        /// The second block of memory.
        /// </param>
        /// <param name="count">
        /// Number of bytes to compare.
        /// </param>
        /// <returns>
        /// Returns an integral value indicating the relationship between the content of the memory blocks:
        /// <list type="ordered">
        ///  <item>
        ///  <c>&lt; 0</c> the first byte that does not match in both memory blocks has a lower value in
        ///  <paramref name="b1"/> than in <paramref name="b2"/> (if evaluated as <c>unsigned char</c> values)
        ///  </item>
        ///  <item>
        ///  <c>0</c> if the contents of both memory blocks are equal
        ///  </item>
        ///  <item>
        ///  <c>&gt; 0</c> the first byte that does not match in both memory blocks has a greater value in
        ///  <paramref name="b1"/> than in <paramref name="b2"/> (if evaluated as <c>unsigned char</c> values)
        ///  </item>
        /// </list>
        /// </returns>
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int memcmp(byte[] b1, byte[] b2, uint count);

        /// <summary>
        /// Compares two blocks of memory.
        /// </summary>
        /// <param name="b1">
        /// The first block of memory.
        /// </param>
        /// <param name="b2">
        /// The second block of memory.
        /// </param>
        /// <param name="count">
        /// Number of bytes to compare.
        /// </param>
        /// <returns>
        /// Returns an integral value indicating the relationship between the content of the memory blocks:
        /// <list type="ordered">
        ///  <item>
        ///  <c>&lt; 0</c> the first byte that does not match in both memory blocks has a lower value in
        ///  <paramref name="b1"/> than in <paramref name="b2"/> (if evaluated as <c>unsigned char</c> values)
        ///  </item>
        ///  <item>
        ///  <c>0</c> if the contents of both memory blocks are equal
        ///  </item>
        ///  <item>
        ///  <c>&gt; 0</c> the first byte that does not match in both memory blocks has a greater value in
        ///  <paramref name="b1"/> than in <paramref name="b2"/> (if evaluated as <c>unsigned char</c> values)
        ///  </item>
        /// </list>
        /// </returns>
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int memcmp(byte* b1, byte* b2, uint count);
    }
}
