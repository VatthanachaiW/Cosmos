using System;
using IL2CPU.API.Attribs;

namespace Cosmos.Core_Plugs.System
{
    [Plug(Target = typeof(Buffer))]
    public class BufferImpl
    {
        /// <summary>
        /// The memmove() function copies n bytes from memory area src to memory area dest.
        /// The memory areas may overlap: copying takes place as though the bytes in src
        /// are first copied into a temporary array that does not overlap src or dest,
        /// and the bytes are then copied from the temporary array to dest.
        /// </summary>
        /// <param name="dest">Destination address to copy data into.</param>
        /// <param name="src">Source address from where copy data.</param>
        /// <param name="count">Count of bytes to copy.</param>
        [PlugMethod(IsOptional = true, Signature = "System_Void__System_Buffer___Memmove_System_Byte___System_Byte___System_UIntPtr_")]
        public static unsafe void __Memmove(byte* dest, byte* src, uint count)
        {
            uint t;
            const int wmask = 0xFFFF;
            const int wsize = 2;

            /* nothing to do */
            if (count == 0 || dest == src)
            {
                return;
            }

            if ((uint)dest < (uint)src)
            {
                /* Copy forward. */
                t = (uint)src;

                /* only need low bits */
                if (((t | (uint)dest) & wmask) != 0)
                {
                    /*
                    * Try to align operands.  This cannot be done
                    * unless the low bits match.
                    */
                    if ((((t ^ (int)dest) & wmask) != 0) || (count < wsize))
                        t = count;
                    else
                        t = wsize - (t & wmask);
                    count -= t;
                    if (t != 0)
                    {
                        do { *dest++ = *src++; }
                        while (--t != 0);
                    }
                }

                /*
                * Copy whole words, then mop up any trailing bytes.
                */
                t = count / wsize;
                if (t != 0)
                {
                    do
                    {
                        *(short*)dest = *(short*)src;
                        src += wsize;
                        dest += wsize;
                    }
                    while (--t != 0);
                }

                t = count & wmask;
                if (t != 0)
                {
                    do
                    {
                        dest++;
                        src++;
                        *dest = *src;
                    }
                    while (--t != 0);
                }
            }
            else
            {
                /*
                * Copy backwards.  Otherwise essentially the same.
                * Alignment works as before, except that it takes
                * (t&wmask) bytes to align, not wsize-(t&wmask).
                */
                src += count;
                dest += count;
                t = (uint)src;
                if (((t | (uint)dest) & wmask) != 0)
                {
                    if (((t ^ (uint)dest) & wmask) != 0 || count <= wsize)
                        t = count;
                    else
                        t &= wmask;
                    count -= t;
                    if (t != 0)
                    {
                        do
                        {
                            --dest;
                            --src;
                            *dest = *src;
                        }
                        while (--t != 0);
                    }
                }
                t = count / wsize;
                if (t != 0)
                {
                    do
                    {
                        src -= wsize;
                        dest -= wsize;
                        *(ushort*)dest = *(ushort*)src;
                    }
                    while (--t != 0);
                }

                t = count & wmask;
                if (t != 0)
                {
                    do
                    {
                        --dest;
                        --src;
                        *dest = *src;
                    }
                    while (--t != 0);
                }
            }
        }

        /// <summary>
        /// The memmove() function copies n bytes from memory area src to memory area dest.
        /// The memory areas may overlap: copying takes place as though the bytes in src
        /// are first copied into a temporary array that does not overlap src or dest,
        /// and the bytes are then copied from the temporary array to dest.
        /// </summary>
        /// <param name="dest">Destination address to copy data into.</param>
        /// <param name="src">Source address from where copy data.</param>
        /// <param name="count">Count of bytes to copy.</param>
        [PlugMethod(IsOptional = true)]
        public static unsafe void __Memmove(byte* dest, byte* src, ulong count)
        {
            // TODO: Cast could cause a loss of data.
            __Memmove(dest, src, (uint)count);
        }

        [PlugMethod(Signature = "System_Void__System_Buffer___BulkMoveWithWriteBarrier__System_Byte___System_Byte__System_UIntPtr_")]
        public static unsafe void __BulkMoveWithWriteBarrier(ref byte destination, ref byte source, uint byteCount)
        {
            fixed (byte* srcPtr = &source)
            fixed (byte* dstPtr = &destination)
            {
                for (int i = 0; i < byteCount; i++)
                {
                    dstPtr[i] = srcPtr[i];
                }
            }

            // Unsafe.CopyBlock(ref destination, ref source, byteCount);
        }

        public static unsafe void __ZeroMemory(void* aVoidPtr, UIntPtr aUIntPtr)
        {
            throw new NotImplementedException();
        }
    }
}
