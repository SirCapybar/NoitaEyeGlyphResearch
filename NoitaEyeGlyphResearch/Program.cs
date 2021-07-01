using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using NoitaEyeGlyphResearchLib;

namespace NoitaEyeGlyphResearch {
    class Program {
        private static void Main() {
            try {
                string[] lines = File.ReadAllLines("data.csv");
                TrigramLineCollection tlc = new TrigramLineCollection(new TrigramCollection[lines.Length - 1]);
                for (int i = 1; i < lines.Length; ++i) {
                    tlc.TrigramLines[i - 1] = lines[i].ExtractTrigrams();
                }
                //TrigramSet modified = baseTrigramSet.Reorder(ReorderParam.None, ReorderParam.SwapAC);
                //Console.WriteLine(data.GetFrequencyMessage());
                const int CHAR_BOT_INCL = 32, CHAR_TOP_EXCL = 127, UTF8_TOP_INCL = 255;
                StringBuilder avgIcsBuilder = new StringBuilder(), perMessageIcsBuilder = new StringBuilder();
                foreach (ReorderParam oddParam in new[] {
                    ReorderParam.ABC,
                    ReorderParam.BAC,
                    ReorderParam.CBA,
                    ReorderParam.ACB,
                    ReorderParam.BCA,
                    ReorderParam.CAB
                }) {
                    foreach (ReorderParam evenParam in new[] {
                        ReorderParam.ABC,
                        ReorderParam.BAC,
                        ReorderParam.CBA,
                        ReorderParam.ACB,
                        ReorderParam.BCA,
                        ReorderParam.CAB
                    }) {
                        const int KEY_COUNT = 35;
                        TrigramLineCollection subTlc = tlc.Reorder(oddParam, evenParam);
                        Console.WriteLine($"{oddParam}, {evenParam}");
                        Console.WriteLine(subTlc.GetIc());
                        Console.WriteLine();
                        float[][] keyLengthBasedIcs = new float[subTlc.TrigramLines.Length][];
                        for (int i = 0; i < keyLengthBasedIcs.Length; ++i) {
                            keyLengthBasedIcs[i] = subTlc.TrigramLines[i].GetICs(KEY_COUNT);
                        }
                        float[] avg = new float[KEY_COUNT];
                        for (int i = 0; i < KEY_COUNT; ++i) {
                            float sum = 0.0f;
                            foreach (float[] ics in keyLengthBasedIcs) {
                                sum += ics[i];
                            }
                            avg[i] = sum / keyLengthBasedIcs.Length;
                        }
                        avgIcsBuilder.AppendLine($"{oddParam},{evenParam}," +
                                           $"{string.Join(",", avg.Select(f => f.ToString(CultureInfo.InvariantCulture)))}");
                        for (int i = 0; i < keyLengthBasedIcs.Length; ++i) {
                            perMessageIcsBuilder.AppendLine($"Msg{i + 1},{oddParam},{evenParam},{string.Join(",", keyLengthBasedIcs[i].Select(f => f.ToString(CultureInfo.InvariantCulture)))}");
                        }
                    }
                }

                File.WriteAllText("ics.csv", avgIcsBuilder.ToString());
                File.WriteAllText("msg_ics.csv", perMessageIcsBuilder.ToString());
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
