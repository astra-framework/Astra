using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Astra.Util;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static unsafe class NativeMemory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* Alloc<T>(nuint size) where T : unmanaged => (T*)System.Runtime.InteropServices.NativeMemory.Alloc(size * (nuint)sizeof(T));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Free(void* ptr) => System.Runtime.InteropServices.NativeMemory.Free(ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetByteCountUTF8(string str)
    {
        return Encoding.UTF8.GetByteCount(str);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetByteCountUTF16(string str)
    {
        return Encoding.Unicode.GetByteCount(str);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte* StringToUTF8Ptr(string str)
    {
        int size = GetByteCountUTF8(str);
        byte* ptr = Alloc<byte>((nuint)(size + 1));
        fixed (char* pStr = str)
        {
            Encoding.UTF8.GetBytes(pStr, str.Length, ptr, size);
        }
        ptr[size] = 0;
        return ptr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char* StringToUTF16Ptr(string str)
    {
        int size = GetByteCountUTF16(str);
        byte* ptr = Alloc<byte>((nuint)size);
        fixed (char* pStr = str)
        {
            Encoding.Unicode.GetBytes(pStr, str.Length, ptr, size);
        }
        char* result = (char*)ptr;
        result[str.Length] = '\0';
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Memcpy<T>(void* dest, T[] src, int length) where T : unmanaged
    {
        T* typedDest = (T*)dest;
        for (int i = 0; i < length; i++)
        {
            if (i >= src.Length) break;
            typedDest[i] = src[i];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T PtrToStructure<T>(void* ptr) where T : unmanaged
    {
        return *(T*)ptr;
    }
}