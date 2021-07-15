using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NoitaEyeGlyphResearchLib {
    public class TrigramLineCollection : IEnumerable {
        public TrigramLineCollection(TrigramCollection[] trigramLines) {
            TrigramLines = trigramLines;
        }

        public TrigramCollection[] TrigramLines { get; }

        public int Length {
            get { return TrigramLines.Length; }
        }

        public TrigramCollection this[int index] {
            get { return TrigramLines[index]; }
            set { TrigramLines[index] = value; }
        }

        public TrigramLineCollection Reorder(ReorderParam odd, ReorderParam even) {
            TrigramCollection[] result = new TrigramCollection[Length];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = TrigramLines[i].Reorder(odd, even);
            }
            return new TrigramLineCollection(result);
        }

        public TrigramLineCollection GetOdd() {
            return GetHalf(true);
        }

        public TrigramLineCollection GetEven() {
            return GetHalf(false);
        }

        public TrigramLineCollection GetHalf(bool odd) {
            TrigramCollection[] result = new TrigramCollection[Length];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = TrigramLines[i].GetHalf(odd);
            }
            return new TrigramLineCollection(result);
        }

        public Dictionary<Trigram, uint> GetFrequencyData() {
            Dictionary<Trigram, uint> result = new Dictionary<Trigram, uint>();
            foreach (TrigramCollection trigrams in TrigramLines) {
                Dictionary<Trigram, uint> data = trigrams.GetFrequencyData();
                foreach (Trigram key in data.Keys) {
                    if (result.ContainsKey(key)) {
                        result[key] += data[key];
                    } else {
                        result.Add(key, data[key]);
                    }
                }
            }
            return result;
        }


        public float GetIc() {
            Dictionary<Trigram, uint>[] data = TrigramLines.Select(t => t.GetFrequencyData()).ToArray();
            float sum = 0.0f;
            float denom = TrigramLines[0].Length;
            for (int i = 1; i < Length; ++i) {
                denom *= TrigramLines[i].Length;
            }
            IEnumerable<Trigram> alphabet =
                data.Select(p => p.Keys.ToList())
                    .Aggregate(new List<Trigram>(), (x, y) => x.Concat(y).ToList()).Distinct();
            foreach (Trigram trigram in alphabet) {
                float partialSum = 0.0f;
                if (data[0].ContainsKey(trigram)) {
                    partialSum = data[0][trigram];
                }
                for (int i = 1; i < data.Length; ++i) {
                    partialSum *= data[i].ContainsKey(trigram) ? data[i][trigram] : 0.0f;
                }
                sum += partialSum;
            }
            return sum / denom;
        }

        public IEnumerator GetEnumerator() {
            return TrigramLines.GetEnumerator();
        }
    }
}