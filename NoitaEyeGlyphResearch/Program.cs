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

                const int MIN_ASCII_INCL = 32, MAX_ASCII_EXCL = 127;
                Encoding encoding = Encoding.BigEndianUnicode;

                //StringBuilder keyOutputBuilder = new StringBuilder();
                //string[] keys = { "asabovesobelow", "ASABOVESOBELOW", "AsAboveSoBelow" };
                //Dictionary<Trigram, uint> data = tlc.GetFrequencyData();
                //Dictionary<Trigram, int> alphabet = new Dictionary<Trigram, int>();
                //Trigram[] dataTrigrams = data.Keys.ToArray();
                //for (int i = 0; i < dataTrigrams.Length; ++i) {
                //    alphabet[dataTrigrams[i]] = i;
                //}
                //foreach (ReorderParam oddParam in new[] {
                //    ReorderParam.ABC,
                //    ReorderParam.BAC,
                //    ReorderParam.CBA,
                //    ReorderParam.ACB,
                //    ReorderParam.BCA,
                //    ReorderParam.CAB
                //}) {
                //    foreach (ReorderParam evenParam in new[] {
                //        ReorderParam.ABC,
                //        ReorderParam.BAC,
                //        ReorderParam.CBA,
                //        ReorderParam.ACB,
                //        ReorderParam.BCA,
                //        ReorderParam.CAB
                //    }) {
                //        keyOutputBuilder.AppendLine($">>>>> {oddParam}, {evenParam}");
                //        TrigramLineCollection tlcc = tlc.Reorder(oddParam, evenParam);
                //        foreach (string key in keys) {
                //            for (int offset = 0; offset < 125; ++offset) {
                //                TrigramCollection tck = encoding.GetBytes(key).ToTrigrams(offset).Reorder(oddParam, evenParam);
                //                if (tck.Trigrams.Any(t => !data.ContainsKey(t))) {
                //                    continue;
                //                }

                //                keyOutputBuilder.AppendLine($"FOR KEY {key} AND OFFSET {offset}:");
                //                foreach (TrigramCollection line in tlcc.TrigramLines.Take(1)) {
                //                    TrigramCollection decoded = line.Cypher(tck, alphabet, true);
                //                    string result = encoding.GetString(decoded.ToByteArray(-offset));
                //                    keyOutputBuilder.AppendLine($"{decoded.GetIc()} > {result}");
                //                    //result = encoding.GetString(decoded.ToByteArray(offset));
                //                    //keyOutputBuilder.AppendLine($"{decoded.GetIc()} > {result}");
                //                    //result = encoding.GetString(decoded.ToByteArray(0));
                //                    //keyOutputBuilder.AppendLine($"{decoded.GetIc()} > {result}");
                //                    //result = encoding.GetString(decoded.ToByteArray(32));
                //                    //keyOutputBuilder.AppendLine($"{decoded.GetIc()} > {result}");
                //                }
                //            }

                //            //foreach (TrigramCollection line in tlc.TrigramLines) {
                //            //    byte[] lineBytes = line.ToByteArray();
                //            //    byte[] decoded = lineBytes.Cypher(keyBytes, true);

                //            //}
                //            //int min = key.Min();
                //            //int minOffset = -min, maxOffset = minOffset + 124;
                //            //for (int i = minOffset; i < maxOffset; ++i) {
                //            //    TrigramCollection tck = key.ToTrigrams(i);
                //            //    foreach (TrigramCollection line in tlc.TrigramLines.Take(1)) {
                //            //        string r1 = line.Cypher(tck, true).ToString();
                //            //        string r2 = line.Cypher(tck, false).ToString();
                //            //        keyOutputBuilder.AppendLine(
                //            //            r1.Any(c => c < MIN_ASCII_INCL || c >= MAX_ASCII_EXCL) ? r1 : "Invalid R1");
                //            //        keyOutputBuilder.AppendLine(
                //            //            r2.Any(c => c < MIN_ASCII_INCL || c >= MAX_ASCII_EXCL) ? r2 : "Invalid R2");
                //            //    }
                //            //    keyOutputBuilder.AppendLine("---");
                //            //}
                //            keyOutputBuilder.AppendLine("--------------");
                //        }
                //    }
                //}
                //File.WriteAllText("key_output.txt", keyOutputBuilder.ToString());



                //StringBuilder binaryReworkBuilder = new StringBuilder();
                //foreach (ReorderParam oddParam in new[] {
                //    ReorderParam.ABC,
                //    ReorderParam.BAC,
                //    ReorderParam.CBA,
                //    ReorderParam.ACB,
                //    ReorderParam.BCA,
                //    ReorderParam.CAB
                //}) {
                //    foreach (ReorderParam evenParam in new[] {
                //        ReorderParam.ABC,
                //        ReorderParam.BAC,
                //        ReorderParam.CBA,
                //        ReorderParam.ACB,
                //        ReorderParam.BCA,
                //        ReorderParam.CAB
                //    }) {
                //        Console.WriteLine("--------------------");
                //        foreach (TrigramCollection line in tlc.Reorder(oddParam, evenParam).TrigramLines) {
                //            List<byte> trSum = line.Trigrams.Select(t => t.GetSum()).ToList();
                //            Console.WriteLine(string.Join(',', trSum));
                //            Console.WriteLine(trSum.Max());
                //            Console.WriteLine(line.GetSum());
                //            string bin = line.ToBinaryString(false), binInverse = line.ToBinaryString(true);
                //            int remainder = bin.Length % 8;
                //            if (remainder != 0) {
                //                remainder = 8 - remainder;
                //                bin += new string('0', remainder);
                //                binInverse += new string('1', remainder);
                //            }

                //            char[] c = bin.ToCharArray();
                //            Array.Reverse(c);
                //            bin = new string(c);
                //            c = binInverse.ToCharArray();
                //            Array.Reverse(c);
                //            binInverse = new string(c);

                //            string reworkedBin = encoding.GetString(Extensions.BinaryStringToByteArray(bin));
                //            string reworkedBinInverse = encoding.GetString(Extensions.BinaryStringToByteArray(binInverse));
                //            binaryReworkBuilder.AppendLine(reworkedBin);
                //            binaryReworkBuilder.AppendLine(reworkedBinInverse);
                //            Console.WriteLine("______");
                //        }
                //    }
                //}
                //File.WriteAllText("binary_rework.txt", binaryReworkBuilder.ToString());
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
