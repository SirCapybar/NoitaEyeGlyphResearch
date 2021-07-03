using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoitaEyeGlyphResearchLib {
    public static class Extensions {
        public static TrigramCollection ExtractTrigrams(this string csvLine) {
            /*
             *  1 2 / 6 \ 7 8 . . .
             *   3 / 5 4 \ 9 . . .
             */
            Queue<byte> values = new Queue<byte>(csvLine.Split(',')
                .Skip(2)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(byte.Parse));
            List<Trigram> trigrams = new List<Trigram>();
            while (values.Any()) {
                trigrams.Add(new Trigram(values.Dequeue(), values.Dequeue(), values.Dequeue()));
            }

            return new TrigramCollection(trigrams.ToArray());
        }

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

        public static byte[][] GetAllPossibleMappings() {
            byte[] b = { 0, 1, 2, 3, 4 };
            return b.Permute().Select(e => e.ToArray()).ToArray();
        }

        public static Trigram ToTrigram(this char c, int offset) {
            return new Trigram(offset + c);
        }

        public static TrigramCollection ToTrigrams(this string str, int offset) {
            return new TrigramCollection(str.Select(c => c.ToTrigram(offset)).ToArray());
        }

        public static byte[] Cypher(this byte[] bytes, byte[] key, bool decode) {
            byte[] result = new byte[bytes.Length];
            int keyIndex = 0;
            for (int i = 0; i < result.Length; ++i) {
                if (decode) {
                    result[i] = (byte)(bytes[i] - key[keyIndex]);
                } else { // encode
                    result[i] = (byte)(bytes[i] + key[keyIndex]);
                }

                if (++keyIndex == key.Length) {
                    keyIndex = 0;
                }
            }
            return result;
        }

        public static TrigramCollection ToTrigrams(this byte[] bytes, int offset) {
            return new TrigramCollection(bytes.Select(b => new Trigram(offset + b)).ToArray());
        }

        // reference: https://stackoverflow.com/questions/15150147/all-permutations-of-a-list
        public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> sequence) {
            if (sequence == null) {
                yield break;
            }

            var list = sequence.ToList();

            if (!list.Any()) {
                yield return Enumerable.Empty<T>();
            } else {
                var startingElementIndex = 0;

                foreach (var startingElement in list) {
                    var index = startingElementIndex;
                    var remainingItems = list.Where((e, i) => i != index);

                    foreach (var permutationOfRemainder in remainingItems.Permute()) {
                        yield return permutationOfRemainder.Prepend(startingElement);
                    }

                    startingElementIndex++;
                }
            }
        }
    }
}