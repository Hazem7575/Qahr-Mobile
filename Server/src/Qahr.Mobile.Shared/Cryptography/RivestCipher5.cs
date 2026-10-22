// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.Shared.Cryptography
{
    public unsafe static class NativeFunctionCalls
    {
        public const string MSVCRT = "msvcrt.dll";
        public const string KERNEL32 = "kernel32.dll";
        // Only memcpy(void*, byte[], len) is used (GenerateKeys below). The other msvcrt imports that were
        // here (malloc/calloc/realloc/memset/memcmp, the other memcpy overloads, srand/rand) had no caller
        // and took 'int' where C takes size_t (calloc even missed an argument), so they were removed
        // (free is unused too but its signature is right on x86 and x64, so it stays).
        // size_t = UIntPtr: 32-bit on x86 (identical to the old int), 64-bit on x64.
        [DllImport(MSVCRT, EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern void* memcpy_native(void* dst, byte[] src, UIntPtr length);

        public static void* memcpy(void* dst, byte[] src, int length)
        {
            return memcpy_native(dst, src, (UIntPtr)unchecked((uint)length));
        }
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern unsafe void free(void* memblock);
    }
    public unsafe static class SeedGenerator
    {
        public static byte[] Generate(int seed)
        {
            byte[] initializationVector = new byte[0x10];
            for (int index = 0; index < 0x10; index++)
            {
                seed *= 0x343fd;
                seed += 0x269ec3;
                initializationVector[index] = (byte)((seed >> 0x10) & 0x7fff);
            }
            return initializationVector;
        }
    }
    public unsafe sealed class RivestCipher5
    {
        private const int KEY_SIZE = 4;
        private const int SEED_SIZE = 16;
        private const int SUBSTITUTION_SIZE = 26;
        private const int BITS_SHIFTED = 32;
        private static byte[] _initializationVector = { 0x3C, 0xDC, 0xFE, 0xE8, 0xC4, 0x54, 0xD6, 0x7E, 0x16, 0xA6, 0xF8, 0x1A, 0xE8, 0xD0, 0x38, 0xBE };
        private uint[] _keyBuffer;
        private uint[] _substitutionBuffer;
        public RivestCipher5()
        {
            GenerateKeys(_initializationVector);
        }
        public RivestCipher5(int seed)
        {
            GenerateKeys(SeedGenerator.Generate(seed));
        }
        public void GenerateKeys(byte[] initializationVector)
        {
            _keyBuffer = new uint[KEY_SIZE];
            _substitutionBuffer = new uint[SUBSTITUTION_SIZE];
            fixed (uint* keyBufferPtr = _keyBuffer)
                NativeFunctionCalls.memcpy((byte*)keyBufferPtr, initializationVector, KEY_SIZE * sizeof(uint));

            _substitutionBuffer[0] = 0xB7E15163;
            for (int index = 1; index < SUBSTITUTION_SIZE; index++)
                _substitutionBuffer[index] = _substitutionBuffer[index - 1] + 0x9E3779B9;

            uint substitutionIndex = 0, keyIndex = 0, x = 0, y = 0;
            for (int loopControlIndex = 0; loopControlIndex < 3 * SUBSTITUTION_SIZE; loopControlIndex++)
            {
                _substitutionBuffer[substitutionIndex] = RotateLeft(_substitutionBuffer[substitutionIndex] + x + y, 3);
                x = _substitutionBuffer[substitutionIndex];
                substitutionIndex = (substitutionIndex + 1) % SUBSTITUTION_SIZE;
                _keyBuffer[keyIndex] = RotateLeft(_keyBuffer[keyIndex] + x + y, (int)(x + y));
                y = _keyBuffer[keyIndex];
                keyIndex = (keyIndex + 1) % KEY_SIZE;
            }
        }
        public bool Decrypt(byte[] input)
        {
            if (input.Length % 8 != 0) return false;
            // [fix] الـ pointer كان بيتستخدم بعد ما بلوك fixed يخلص (لو الـ GC حرّك البافر = كتابة في مكان غلط).
            fixed (byte* ptr = input)
            {
            uint* buffer = (uint*)ptr;
            int rounds = input.Length / 8;
            for (int index = 0; index < rounds; index++)
            {
                uint left = buffer[2 * index];
                uint right = buffer[(2 * index) + 1];
                for (int subIndex = 12; subIndex > 0; subIndex--)
                {
                    right = RotateRight(right - _substitutionBuffer[(2 * subIndex) + 1], (int)left) ^ left;
                    left = RotateRight(left - _substitutionBuffer[2 * subIndex], (int)right) ^ right;
                }
                uint resultLeft = left - _substitutionBuffer[0];
                uint resultRight = right - _substitutionBuffer[1];
                buffer[2 * index] = resultLeft;
                buffer[(2 * index) + 1] = resultRight;
            }
            }
            return true;
        }
        public bool Encrypt(byte[] input)
        {
            if (input.Length % 8 != 0) return false;
            // [fix] الـ pointer كان بيتستخدم بعد ما بلوك fixed يخلص (لو الـ GC حرّك البافر = كتابة في مكان غلط).
            fixed (byte* ptr = input)
            {
            uint* buffer = (uint*)ptr;
            int rounds = input.Length / 8;
            for (int index = 0; index < rounds; index++)
            {
                uint left = buffer[2 * index] + _substitutionBuffer[0];
                uint right = buffer[(2 * index) + 1] + _substitutionBuffer[1];
                for (int subIndex = 1; subIndex <= 12; subIndex++)
                {
                    left = RotateLeft(left ^ right, (int)right) + _substitutionBuffer[2 * subIndex];
                    right = RotateLeft(right ^ left, (int)left) + _substitutionBuffer[(2 * subIndex) + 1];
                }
                buffer[2 * index] = left;
                buffer[(2 * index) + 1] = right;
            }
            }
            return true;
        }
        public uint RotateLeft(uint value, int count)
        {
            count %= BITS_SHIFTED;
            uint high = value >> (BITS_SHIFTED - count);
            return (value << count) | high;
        }
        public uint RotateRight(uint value, int count)
        {
            count %= BITS_SHIFTED;
            uint low = value << (BITS_SHIFTED - count);
            return (value >> count) | low;
        }
    }
}