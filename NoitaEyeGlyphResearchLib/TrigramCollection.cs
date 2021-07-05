using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoitaEyeGlyphResearchLib {
    public class TrigramCollection {
        public TrigramCollection(Trigram[] trigrams) {
            Trigrams = trigrams;
        }

        public Trigram[] Trigrams { get; }

        public float GetIc() {
            Dictionary<Trigram, uint> data = GetFrequencyData();
            float sum = 0.0f;
            foreach (KeyValuePair<Trigram, uint> pair in data) {
                sum += pair.Value * (pair.Value - 1U);
            }
            return sum / (Trigrams.Length * (Trigrams.Length - 1));
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
            for (int i = odd ? 0 : 1; i < Trigrams.Length; i += 2) {
                result.Add(Trigrams[i]);
            }
            return new TrigramCollection(result.ToArray());
        }

        public int[] GetBase10Array() {
            int[] result = new int[Trigrams.Length];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = Trigrams[i].GetBase10();
            }
            return result;
        }

        public int[] GetBase10Array(byte[] mappings) {
            int[] result = new int[Trigrams.Length];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = Trigrams[i].GetBase10(mappings);
            }
            return result;
        }

        public TrigramCollection Reorder(ReorderParam odd, ReorderParam even) {
            Trigram[] result = new Trigram[Trigrams.Length];
            for (int i = 0; i < Trigrams.Length; ++i) {
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
                    while (ind < Trigrams.Length) {
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
            TrigramCollection result = new TrigramCollection(new Trigram[Trigrams.Length]);
            int keyIndex = 0;
            for (int i = 0; i < result.Trigrams.Length; ++i) {
                if (decode) {
                    result.Trigrams[i] = Trigrams[i] - key.Trigrams[keyIndex];
                } else { // encode
                    result.Trigrams[i] = Trigrams[i] + key.Trigrams[keyIndex];
                }

                if (++keyIndex == key.Trigrams.Length) {
                    keyIndex = 0;
                }
            }
            return result;
        }

        public TrigramCollection Cypher(TrigramCollection key, Dictionary<Trigram, int> alphabet, bool decode) {
            TrigramCollection result = new TrigramCollection(new Trigram[Trigrams.Length]);
            int keyIndex = 0;
            for (int i = 0; i < result.Trigrams.Length; ++i) {
                if (decode) {
                    result.Trigrams[i] = Trigrams[i] - alphabet[key.Trigrams[keyIndex]];
                } else { // encode
                    result.Trigrams[i] = Trigrams[i] + alphabet[key.Trigrams[keyIndex]];
                }

                if (++keyIndex == key.Trigrams.Length) {
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

        public byte[] GetDiamondCypherValues() {
            return Trigrams.Select(t => t.GetDiamondCypherValue(false)).ToArray();
        }

        public byte[] GetDiamondCypherValues(bool reverseOdd, bool reverseEven) {
            byte[] result = new byte[Trigrams.Length];
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
