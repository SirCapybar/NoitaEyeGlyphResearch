﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoitaEyeGlyphResearchLib {
    public static class Statics {
        public static int[][] DiamondMatrix { get; } = new int[7][] {
            new[] {  0, 0, 0, 1, 0, 0, 0 },
            new[] {  0, 0, 2, 3, 4, 0, 0 },
            new[] {  0, 5, 6, 7, 8, 9, 0 },
            new[] { 10,11,12,13,14,15,16 },
            new[] {  0,17,18,19,20,21, 0 },
            new[] {  0, 0,22,23,24, 0, 0 },
            new[] {  0, 0, 0,25, 0, 0, 0 }
        };

        public static int[][][] PolybiusCube { get; } = new int[5][][];

        static Statics() {
            // Initializing the polybius cube.
            // I am aware it can be done so much easier, with a single pass.
            // I just didn't feel like... thinking.
            for (int i = 0; i < 5; ++i) {
                PolybiusCube[i] = new int[5][];
                for (int j = 0; j < 5; ++j) {
                    PolybiusCube[i][j] = new int[5];
                }
            }

            for (int i = 0; i < 5; ++i) {
                for (int j = 0; j < 5; ++j) {
                    PolybiusCube[0][i][j] = i * 5 + j;
                }
            }

            for (int i = 1; i < 5; ++i) {
                for (int j = 0; j < 5; ++j) {
                    for (int k = 0; k < 5; ++k) {
                        PolybiusCube[i][j][k] = PolybiusCube[0][(j + i) % 5][k];
                    }
                }
            }
        }

        /// <summary>
        /// Converts this <see cref="int"/> array into ASCII chars with given offset.
        /// Clamped to a minimum value of 32, but there's not upper bound value verification.
        /// </summary>
        /// <param name="base10"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static char[] ToChars(this int[] base10, int offset = 0) {
            char[] result = new char[base10.Length];
            for (int i = 0; i < result.Length; ++i) {
                int n = base10[i] + offset;
                if (n < 32) {
                    n = 32;
                }

                result[i] = (char)n;
            }

            return result;
        }

        /// <summary>
        /// Returns a string that's a nice summary of this trigram frequency data.
        /// Writes out all of the unique values (sorted) and their count, as well as the occurrence count of each trigram.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetFrequencyMessage(this Dictionary<Trigram, uint> data) {
            StringBuilder builder = new StringBuilder($"{data.Keys.Count} unique trigram values:\n");
            Trigram[] keys = data.Keys.ToArray();
            Array.Sort(keys);
            for (int i = 0; i < keys.Length; ++i) {
                builder.Append(keys[i]);
                builder.Append(i + 1 == keys.Length ? '\n' : ',');
            }

            builder.AppendLine();
            IOrderedEnumerable<KeyValuePair<Trigram, uint>> list =
                data.ToList().OrderBy(p => p.Value).ThenBy(p => p.Key);
            foreach (KeyValuePair<Trigram, uint> pair in list) {
                builder.AppendLine($"{pair.Key},{pair.Value}");
            }

            builder.AppendLine();
            return builder.ToString();
        }

        /// <summary>
        /// Returns all possible 5-byte arrays containing distinct values in range [0;4].
        /// Not particularly useful at this point.
        /// </summary>
        /// <returns></returns>
        public static byte[][] GetAllPossibleMappings() {
            byte[] b = { 0, 1, 2, 3, 4 };
            return b.Permute().Select(e => e.ToArray()).ToArray();
        }

        /// <summary>
        /// Converts this <see cref="char"/> into a trigram by value, including an offset.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Trigram ToTrigram(this char c, int offset) {
            return new Trigram(offset + c);
        }

        /// <summary>
        /// Converts this <see cref="string"/> into a <see cref="TrigramCollection"/> by <see cref="char"/> values, including an offset.
        /// This is equivalent to calling <see cref="ToTrigram"/> on each <see cref="char"/> within the <see cref="string"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static TrigramCollection ToTrigrams(this string str, int offset) {
            return new TrigramCollection(str.Select(c => c.ToTrigram(offset)).ToArray());
        }

        /// <summary>
        /// Applies Vigenere cypher to this <see cref="byte"/> array with given key.
        /// This method supports both encoding and decoding.
        /// </summary>
        /// <param name="bytes">The input <see cref="byte"/> array.</param>
        /// <param name="key">The key <see cref="byte"/> array.</param>
        /// <param name="decode">True for decoding, false for encoding.</param>
        /// <returns>Encoded/decoded <see cref="byte"/> array.</returns>
        public static byte[] Cypher(this byte[] bytes, byte[] key, bool decode) {
            byte[] result = new byte[bytes.Length];
            int keyIndex = 0;
            for (int i = 0; i < result.Length; ++i) {
                if (decode) {
                    result[i] = (byte)(bytes[i] - key[keyIndex]);
                } else {
                    result[i] = (byte)(bytes[i] + key[keyIndex]);
                }

                if (++keyIndex == key.Length) {
                    keyIndex = 0;
                }
            }
            return result;
        }

        /// <summary>
        /// Applies Vigenere cypher to this message with given key.
        /// This method supports both encoding and decoding.
        /// </summary>
        /// <param name="msg">The input message.</param>
        /// <param name="alphabet">The alphabet <see cref="string"/>. It is required that both the message and key consist only of the alphabet characters.</param>
        /// <param name="key">The key <see cref="string"/>.</param>
        /// <param name="decode">True for decoding, false for encoding.</param>
        /// <returns>Encoded/decoded message.</returns>
        public static string Cypher(this string msg, string alphabet, string key, bool decode) {
            StringBuilder builder = new StringBuilder();
            int keyIndex = 0;
            foreach (char c in msg) {
                int fromIndex = alphabet.IndexOf(c);
                int offset = alphabet.IndexOf(key[keyIndex]);
                int toIndex = decode ? (fromIndex - offset) : (fromIndex + offset);
                toIndex = (toIndex % alphabet.Length + alphabet.Length) % alphabet.Length;
                builder.Append(alphabet[toIndex]);
                if (++keyIndex == key.Length) {
                    keyIndex = 0;
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns the frequency data of each character within this message.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Dictionary<char, uint> GetFrequencyData(this string msg) {
            Dictionary<char, uint> result = new Dictionary<char, uint>();
            foreach (char c in msg) {
                if (result.ContainsKey(c)) {
                    ++result[c];
                } else {
                    result.Add(c, 1U);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the index of coincidence of this message.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static float GetIc(this string msg) {
            Dictionary<char, uint> data = msg.GetFrequencyData();
            float sum = data.Aggregate(0.0f, (current, pair) => current + pair.Value * (pair.Value - 1U));
            return sum / (msg.Length * (msg.Length - 1));
        }

        /// <summary>
        /// Returns the array of index of coincidence values for [1;<see cref="maxKeySize"/>] key length.
        /// Significant peaks at a specific position X and its multiples might suggest a detected key length of X.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="maxKeySize"></param>
        /// <returns></returns>
        public static float[] GetIcs(this string msg, uint maxKeySize) {
            float[] result = new float[maxKeySize];
            for (uint i = 1; i <= maxKeySize; ++i) {
                string[] columns = new string[i];
                for (int j = 0; j < columns.Length; ++j) {
                    List<char> col = new List<char>();
                    int ind = j;
                    while (ind < msg.Length) {
                        col.Add(msg[ind]);
                        ind += (int)i;
                    }
                    columns[j] = new string(col.ToArray());
                }

                float ic = columns.Sum(col => col.GetIc());
                result[i - 1] = ic / i;
            }
            return result;
        }

        /// <summary>
        /// Converts this <see cref="byte"/> array to a <see cref="TrigramCollection"/> by value, including an offset.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static TrigramCollection ToTrigrams(this byte[] bytes, int offset) {
            return new TrigramCollection(bytes.Select(b => new Trigram(offset + b)).ToArray());
        }

        /// <summary>
        /// Converts a string of '0' and '1' into a <see cref="byte"/>.
        /// This method was only used in a weird test I've done some time ago, but it's still here if anyone wants to mess around.
        /// </summary>
        /// <param name="binString"></param>
        /// <returns></returns>
        public static byte[] BinaryStringToByteArray(string binString) {
            if (binString.Length % 8 != 0) {
                throw new ArgumentException(nameof(binString));
            }
            byte[] result = new byte[binString.Length / 8];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = Convert.ToByte(binString.Substring(8 * i, 8), 2);
            }
            return result;
        }

        /// <summary>
        /// Returns all permutations of this sequence.
        /// Reference: https://stackoverflow.com/questions/15150147/all-permutations-of-a-list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> sequence) {
            if (sequence == null) {
                yield break;
            }

            List<T> list = sequence.ToList();

            if (!list.Any()) {
                yield return Enumerable.Empty<T>();
            } else {
                int startingElementIndex = 0;

                foreach (T startingElement in list) {
                    int index = startingElementIndex;
                    IEnumerable<T> remainingItems = list.Where((e, i) => i != index);

                    foreach (IEnumerable<T> permutationOfRemainder in remainingItems.Permute()) {
                        yield return permutationOfRemainder.Prepend(startingElement);
                    }

                    startingElementIndex++;
                }
            }
        }
    }
}