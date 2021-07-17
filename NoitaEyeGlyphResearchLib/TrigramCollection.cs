using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoitaEyeGlyphResearchLib {
    public class TrigramCollection : IEnumerable {
        public TrigramCollection(Trigram[] trigrams) {
            Trigrams = trigrams;
        }

        public Trigram[] Trigrams { get; }

        public int Length {
            get { return Trigrams.Length; }
        }

        public Trigram this[int index] {
            get { return Trigrams[index]; }
            set { Trigrams[index] = value; }
        }

        /// <summary>
        /// Returns the index of coincidence of the trigrams.
        /// </summary>
        /// <returns></returns>
        public float GetIc() {
            Dictionary<Trigram, uint> data = GetFrequencyData();
            float sum = data.Aggregate(0.0f, (current, pair) => current + pair.Value * (pair.Value - 1U));
            return sum / (Length * (Length - 1));
        }

        /// <summary>
        /// Returns the frequency data of each trigram within this collection.
        /// </summary>
        /// <returns></returns>
        public Dictionary<Trigram, uint> GetFrequencyData() {
            Dictionary<Trigram, uint> result = new Dictionary<Trigram, uint>();
            foreach (Trigram trigram in Trigrams) {
                if (result.ContainsKey(trigram)) {
                    ++result[trigram];
                } else {
                    result.Add(trigram, 1U);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a new <see cref="TrigramCollection"/> containing only the odd trigrams of this collection.
        /// </summary>
        /// <returns></returns>
        public TrigramCollection GetOdd() {
            return GetHalf(true);
        }

        /// <summary>
        /// Returns a new <see cref="TrigramCollection"/> containing only the even trigrams of this collection.
        /// </summary>
        /// <returns></returns>
        public TrigramCollection GetEven() {
            return GetHalf(false);
        }

        /// <summary>
        /// Returns a new <see cref="TrigramCollection"/> containing only the even or odd trigrams of this collection.
        /// </summary>
        /// <param name="odd"></param>
        /// <returns></returns>
        public TrigramCollection GetHalf(bool odd) {
            List<Trigram> result = new List<Trigram>();
            for (int i = odd ? 0 : 1; i < Length; i += 2) {
                result.Add(Trigrams[i]);
            }
            return new TrigramCollection(result.ToArray());
        }

        /// <summary>
        /// Returns an <see cref="int"/> array containing base10 values of the trigrams.
        /// </summary>
        /// <returns></returns>
        public int[] GetBase10Array() {
            int[] result = new int[Length];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = Trigrams[i].GetBase10();
            }
            return result;
        }

        /// <summary>
        /// Returns an <see cref="int"/> array containing base10 values of the trigrams, but with [0;4] values mapped to the values provided by argument.
        /// </summary>
        /// <param name="mappings">The value mappings. This array should contain exactly 5 bytes.</param>
        /// <returns></returns>
        public int[] GetBase10Array(byte[] mappings) {
            int[] result = new int[Length];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = Trigrams[i].GetBase10(mappings);
            }
            return result;
        }

        /// <summary>
        /// Returns a new <see cref="TrigramCollection"/> with appropriately reordered trigram values.
        /// </summary>
        /// <param name="odd">Reorder param for the odd trigrams.</param>
        /// <param name="even">Reorder param for the even trigrams.</param>
        /// <returns></returns>
        public TrigramCollection Reorder(ReorderParam odd, ReorderParam even) {
            Trigram[] result = new Trigram[Length];
            for (int i = 0; i < Length; ++i) {
                result[i] = i % 2 == 0 ? Trigrams[i].Reorder(odd) : Trigrams[i].Reorder(even);
            }
            return new TrigramCollection(result);
        }

        /// <summary>
        /// Returns the array of index of coincidence values for [1;<see cref="maxKeySize"/>] key length.
        /// Significant peaks at a specific position X and its multiples might suggest a detected key length of X.
        /// </summary>
        /// <param name="maxKeySize"></param>
        /// <returns></returns>
        public float[] GetIcs(uint maxKeySize) {
            float[] result = new float[maxKeySize];
            for (uint i = 1; i <= maxKeySize; ++i) {
                TrigramCollection[] columns = new TrigramCollection[i];
                for (int j = 0; j < columns.Length; ++j) {
                    List<Trigram> col = new List<Trigram>();
                    int ind = j;
                    while (ind < Length) {
                        col.Add(Trigrams[ind]);
                        ind += (int)i;
                    }
                    columns[j] = new TrigramCollection(col.ToArray());
                }

                float ic = columns.Sum(col => col.GetIc());
                result[i - 1] = ic / i;
            }
            return result;
        }

        /// <summary>
        /// Applies Vigenere cypher to the trigrams with given key.
        /// This method supports both encoding and decoding.
        /// </summary>
        /// <param name="key">The key <see cref="TrigramCollection"/>.</param>
        /// <param name="decode">True for decoding, false for encoding.</param>
        /// <returns>Encoded/decoded message.</returns>
        public TrigramCollection Cypher(TrigramCollection key, bool decode) {
            TrigramCollection result = new TrigramCollection(new Trigram[Length]);
            int keyIndex = 0;
            for (int i = 0; i < result.Length; ++i) {
                if (decode) {
                    result[i] = Trigrams[i] - key[keyIndex];
                } else {
                    result[i] = Trigrams[i] + key[keyIndex];
                }

                if (++keyIndex == key.Length) {
                    keyIndex = 0;
                }
            }
            return result;
        }

        /// <summary>
        /// Applies Vigenere cypher to the trigrams with given key.
        /// This method supports both encoding and decoding.
        /// </summary>
        /// <param name="key">The key <see cref="TrigramCollection"/>.</param>
        /// <param name="alphabet">The alphabet, which maps each trigram character to an offset.
        /// It is required that the <see cref="key"/> and this <see cref="TrigramCollection"/> consists only of trigrams that are within the alphabet.</param>
        /// <param name="decode">True for decoding, false for encoding.</param>
        /// <returns>Encoded/decoded message.</returns>
        public TrigramCollection Cypher(TrigramCollection key, Dictionary<Trigram, int> alphabet, bool decode) {
            TrigramCollection result = new TrigramCollection(new Trigram[Length]);
            int keyIndex = 0;
            for (int i = 0; i < result.Length; ++i) {
                if (decode) {
                    result[i] = Trigrams[i] - alphabet[key[keyIndex]];
                } else {
                    result[i] = Trigrams[i] + alphabet[key[keyIndex]];
                }

                if (++keyIndex == key.Length) {
                    keyIndex = 0;
                }
            }
            return result;
        }

        /// <summary>
        /// Converts the trigrams into <see cref="char"/>s by value, including an offset, and returns a <see cref="string"/>.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public string ToString(int offset) {
            return new string(Trigrams.Select(t => t.ToChar(offset)).ToArray());
        }

        public override string ToString() {
            return ToString(0);
        }

        public IEnumerator GetEnumerator() {
            return Trigrams.GetEnumerator();
        }

        /// <summary>
        /// Returns a <see cref="byte"/> of the trigram base10 values, including an offset.
        /// Keep in mind that the maximum value for a byte is 255, so high offsets will result in overflows.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public byte[] ToByteArray(int offset) {
            return Trigrams.Select(t => (byte)((t.GetBase10() + offset) % 256)).ToArray();
        }

        /// <summary>
        /// Returns the sum of all of the values within all of the trigrams.
        /// </summary>
        /// <returns></returns>
        public int GetSum() {
            return Trigrams.Aggregate(0, (sum, t) => sum + t.GetSum());
        }

        /// <summary>
        /// Returns the sum of all of the values within all of the trigrams, but with [0;4] values mapped to the values provided by argument.
        /// </summary>
        /// <param name="mappings">The value mappings. This array should contain exactly 5 bytes.</param>
        /// <returns></returns>
        public int GetSum(byte[] mappings) {
            return Trigrams.Aggregate(0, (sum, t) => sum + t.GetSum(mappings));
        }

        /// <summary>
        /// Applies <see cref="Trigram.ToBinaryString(bool)"/> on each trigram within this collection and concatenates the results.
        /// </summary>
        /// <param name="invert"></param>
        /// <returns></returns>
        public string ToBinaryString(bool invert) {
            StringBuilder builder = new StringBuilder();
            foreach (Trigram t in Trigrams) {
                builder.Append(t.ToBinaryString(invert));
                invert = !invert;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Applies <see cref="Trigram.ToBinaryString(bool, byte[])"/> on each trigram within this collection and concatenates the results.
        /// </summary>
        /// <param name="invert"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        public string ToBinaryString(bool invert, byte[] mappings) {
            StringBuilder builder = new StringBuilder();
            foreach (Trigram t in Trigrams) {
                builder.Append(t.ToBinaryString(invert, mappings));
                invert = !invert;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns an <see cref="int"/> array containing <see cref="Statics.PolybiusCube"/> values of the trigrams.
        /// </summary>
        /// <returns></returns>
        public int[] GetPolybiusValues() {
            return Trigrams.Select(t => t.GetPolybiusValue()).ToArray();
        }

        /// <summary>
        /// Returns a <see cref="byte"/> array containing <see cref="Statics.DiamondMatrix"/> values of the trigrams.
        /// </summary>
        /// <returns></returns>
        public byte[] GetDiamondCypherValues() {
            return Trigrams.Select(t => t.GetDiamondCypherValue(false)).ToArray();
        }

        /// <summary>
        /// Returns a <see cref="byte"/> array containing <see cref="Statics.DiamondMatrix"/> values of the trigrams.
        /// </summary>
        /// <param name="reverseOdd">Specifies whether odd trigrams should have their eye directions reversed.</param>
        /// <param name="reverseEven">Specifies whether even trigrams should have their eye directions reversed.</param>
        /// <returns></returns>
        public byte[] GetDiamondCypherValues(bool reverseOdd, bool reverseEven) {
            byte[] result = new byte[Length];
            for (int i = 0; i < result.Length; ++i) {
                if (i % 2 == 0) {
                    result[i] = Trigrams[i].GetDiamondCypherValue(reverseOdd);
                } else {
                    result[i] = Trigrams[i].GetDiamondCypherValue(reverseEven);
                }
            }
            return result;
        }
    }
}
