using System.Collections.Generic;
using System.Linq;

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
        public float[] GetICs(uint maxKeySize) {
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
    }
}
