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

        public float GetIc() {
            Dictionary<Trigram, uint> data = GetFrequencyData();
            float sum = 0.0f;
            foreach (KeyValuePair<Trigram, uint> pair in data) {
                sum += pair.Value * (pair.Value - 1U);
            }
            return sum / (Length * (Length - 1));
        }

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

        public TrigramCollection GetOdd() {
            return GetHalf(true);
        }

        public TrigramCollection GetEven() {
            return GetHalf(false);
        }

        public TrigramCollection GetHalf(bool odd) {
            List<Trigram> result = new List<Trigram>();
            for (int i = odd ? 0 : 1; i < Length; i += 2) {
                result.Add(Trigrams[i]);
            }
            return new TrigramCollection(result.ToArray());
        }

        public int[] GetBase10Array() {
            int[] result = new int[Length];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = Trigrams[i].GetBase10();
            }
            return result;
        }

        public int[] GetBase10Array(byte[] mappings) {
            int[] result = new int[Length];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = Trigrams[i].GetBase10(mappings);
            }
            return result;
        }

        public TrigramCollection Reorder(ReorderParam odd, ReorderParam even) {
            Trigram[] result = new Trigram[Length];
            for (int i = 0; i < Length; ++i) {
                result[i] = i % 2 == 0 ? Trigrams[i].Reorder(odd) : Trigrams[i].Reorder(even);
            }
            return new TrigramCollection(result);
        }
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

        public TrigramCollection Cypher(TrigramCollection key, bool decode) {
            TrigramCollection result = new TrigramCollection(new Trigram[Length]);
            int keyIndex = 0;
            for (int i = 0; i < result.Length; ++i) {
                if (decode) {
                    result[i] = Trigrams[i] - key[keyIndex];
                } else { // encode
                    result[i] = Trigrams[i] + key[keyIndex];
                }

                if (++keyIndex == key.Length) {
                    keyIndex = 0;
                }
            }
            return result;
        }

        public TrigramCollection Cypher(TrigramCollection key, Dictionary<Trigram, int> alphabet, bool decode) {
            TrigramCollection result = new TrigramCollection(new Trigram[Length]);
            int keyIndex = 0;
            for (int i = 0; i < result.Length; ++i) {
                if (decode) {
                    result[i] = Trigrams[i] - alphabet[key[keyIndex]];
                } else { // encode
                    result[i] = Trigrams[i] + alphabet[key[keyIndex]];
                }

                if (++keyIndex == key.Length) {
                    keyIndex = 0;
                }
            }
            return result;
        }

        public string ToString(int offset) {
            return new string(Trigrams.Select(t => t.ToChar(offset)).ToArray());
        }

        public override string ToString() {
            return ToString(0);
        }

        public IEnumerator GetEnumerator() {
            return Trigrams.GetEnumerator();
        }

        public byte[] ToByteArray(int offset) {
            return Trigrams.Select(t => (byte)((t.GetBase10() + offset) % 256)).ToArray();
        }

        public int GetSum() {
            return Trigrams.Aggregate(0, (sum, t) => sum + t.GetSum());
        }

        public int GetSum(byte[] mappings) {
            return Trigrams.Aggregate(0, (sum, t) => sum + t.GetSum(mappings));
        }

        public string ToBinaryString(bool invert) {
            StringBuilder builder = new StringBuilder();
            foreach (Trigram t in Trigrams) {
                builder.Append(t.ToBinaryString(invert));
                invert = !invert;
            }
            return builder.ToString();
        }

        public string ToBinaryString(bool invert, byte[] mappings) {
            StringBuilder builder = new StringBuilder();
            foreach (Trigram t in Trigrams) {
                builder.Append(t.ToBinaryString(invert, mappings));
                invert = !invert;
            }
            return builder.ToString();
        }

        public int[] GetPolybiusValues() {
            return Trigrams.Select(t => t.GetPolybiusValue()).ToArray();
        }

        public byte[] GetDiamondCypherValues() {
            return Trigrams.Select(t => t.GetDiamondCypherValue(false)).ToArray();
        }

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
