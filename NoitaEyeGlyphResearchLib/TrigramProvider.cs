using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NoitaEyeGlyphResearchLib {
    public class TrigramProvider {
        public const int MESSAGE_COUNT = 9;
        public const int TRIGRAMS_PER_LINE = 26;

        public TrigramProvider() {
            LoadTrigrams();
        }

        private static TrigramCollection ExtractTrigrams(string csvLine) {
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

        private static TrigramLineCollection GetStandardTrigramLineCollection() {
            List<string> lines = new List<string>();
            using (Stream stream = Assembly.GetCallingAssembly()
                .GetManifestResourceStream($"{nameof(NoitaEyeGlyphResearchLib)}.data.csv")) {
                using (StreamReader reader = new StreamReader(stream ?? throw new ApplicationException())) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        lines.Add(line);
                    }
                }
            }
            TrigramLineCollection tlc = new TrigramLineCollection(new TrigramCollection[lines.Count - 1]);
            for (int i = 1; i < lines.Count; ++i) {
                tlc.TrigramLines[i - 1] = ExtractTrigrams(lines[i]);
            }
            return tlc;
        }

        private void LoadTrigrams() {
            TrigramLineCollection tlc = GetStandardTrigramLineCollection();
            if (tlc.TrigramLines.Length != MESSAGE_COUNT) {
                throw new ApplicationException($"Expected {MESSAGE_COUNT} messages, got {tlc.TrigramLines.Length}!");
            }
            for (int i = 0; i < tlc.TrigramLines.Length; ++i) {
                TrigramCollection tc = tlc.TrigramLines[i];
                int lineCount = tc.Trigrams.Length / TRIGRAMS_PER_LINE;
                int remainder = tc.Trigrams.Length % TRIGRAMS_PER_LINE;
                int index = 0;
                if (remainder != 0) {
                    ++lineCount;
                }
                RawTrigramData[i] = new Trigram[lineCount][];
                for (int j = 0; j < lineCount; ++j) {
                    if (j + 1 == lineCount && remainder != 0) {
                        RawTrigramData[i][j] = new Trigram[remainder];
                    } else {
                        RawTrigramData[i][j] = new Trigram[TRIGRAMS_PER_LINE];
                    }
                    for (int k = 0; k < RawTrigramData[i][j].Length; ++k) {
                        RawTrigramData[i][j][k] = tc.Trigrams[index];
                        ++index;
                    }
                }
            }
        }

        public TrigramLineCollection GetStandard() {
            TrigramCollection[] tcs = new TrigramCollection[MESSAGE_COUNT];
            for (int i = 0; i < tcs.Length; ++i) {
                tcs[i] = new TrigramCollection(GetMessageStandard(i).ToArray());
            }
            return new TrigramLineCollection(tcs);
        }

        public TrigramLineCollection GetReversed() {
            TrigramCollection[] tcs = new TrigramCollection[MESSAGE_COUNT];
            for (int i = 0; i < tcs.Length; ++i) {
                List<Trigram> trigrams = GetMessageStandard(i);
                trigrams.Reverse();
                tcs[i] = new TrigramCollection(trigrams.ToArray());
            }
            return new TrigramLineCollection(tcs);
        }

        public TrigramLineCollection GetVerticalLeftToRight(bool readOddDown, bool readEvenDown) {
            TrigramCollection[] tcs = new TrigramCollection[MESSAGE_COUNT];
            for (int i = 0; i < tcs.Length; ++i) {
                List<Trigram> trigrams = new List<Trigram>();
                for (int j = 0; j < TRIGRAMS_PER_LINE; ++j) {
                    if (j % 2 == 0) {
                        trigrams.AddRange(readOddDown ? GetVerticalLineTopToBottom(i, j) : GetVerticalLineBottomToTop(i, j));
                    } else {
                        trigrams.AddRange(readEvenDown ? GetVerticalLineTopToBottom(i, j) : GetVerticalLineBottomToTop(i, j));
                    }
                }
                tcs[i] = new TrigramCollection(trigrams.ToArray());
            }
            return new TrigramLineCollection(tcs);
        }

        public TrigramLineCollection GetVerticalRightToLeft(bool readOddDown, bool readEvenDown) {
            TrigramCollection[] tcs = new TrigramCollection[MESSAGE_COUNT];
            for (int i = 0; i < tcs.Length; ++i) {
                List<Trigram> trigrams = new List<Trigram>();
                for (int j = TRIGRAMS_PER_LINE - 1; j >= 0; --j) {
                    if (j % 2 == 0) {
                        trigrams.AddRange(readOddDown ? GetVerticalLineTopToBottom(i, j) : GetVerticalLineBottomToTop(i, j));
                    } else {
                        trigrams.AddRange(readEvenDown ? GetVerticalLineTopToBottom(i, j) : GetVerticalLineBottomToTop(i, j));
                    }
                }
                tcs[i] = new TrigramCollection(trigrams.ToArray());
            }
            return new TrigramLineCollection(tcs);
        }

        private List<Trigram> GetMessageStandard(int index) {
            List<Trigram> trigrams = new List<Trigram>();
            for (int j = 0; j < RawTrigramData[index].Length; ++j) {
                for (int k = 0; k < RawTrigramData[index][j].Length; ++k) {
                    trigrams.Add(RawTrigramData[index][j][k]);
                }
            }
            return trigrams;
        }

        private List<Trigram> GetVerticalLineTopToBottom(int msgIndex, int lineIndex) {
            if (lineIndex >= TRIGRAMS_PER_LINE) {
                throw new ArgumentException();
            }
            List<Trigram> result = new List<Trigram>();
            for (int i = 0; i < RawTrigramData[msgIndex].Length; ++i) {
                if (i + 1 != RawTrigramData[msgIndex].Length || RawTrigramData[msgIndex][i].Length > lineIndex) {
                    result.Add(RawTrigramData[msgIndex][i][lineIndex]);
                }
            }
            return result;
        }

        private List<Trigram> GetVerticalLineBottomToTop(int msgIndex, int lineIndex) {
            List<Trigram> trigrams = GetVerticalLineTopToBottom(msgIndex, lineIndex);
            trigrams.Reverse();
            return trigrams;
        }

        private Trigram[][][] RawTrigramData { get; } = new Trigram[MESSAGE_COUNT][][];
    }
}
