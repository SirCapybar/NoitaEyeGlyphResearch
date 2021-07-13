using System;
using System.Collections.Generic;
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

                const int MIN_ASCII_INCL = 32, MAX_ASCII_EXCL = 127;
                Encoding encoding = Encoding.ASCII;
                const int KEY_COUNT = 35;
                const string alph25 = "abcdefghiklmnopqrstuvwxyz";

                foreach (ReorderParam param in Enum.GetValues(typeof(ReorderParam))) {
                    foreach (TrigramCollection line in tlc.Reorder(param, param).TrigramLines.Take(1)) {
                        for (byte i = 0; i < 3; ++i) {
                            string msg = new string(line.Trigrams.Select(t => t.GetBase10()).Where(n => (n / 25) == i)
                                .Select(n => alph25[n % 25]).ToArray());
                            Console.WriteLine(msg);
                            Console.WriteLine(msg.GetIc(alph25));
                        }
                    }
                }

                //foreach (TrigramCollection line in tlc.TrigramLines) {
                //    string[] l = new string[4];
                //    for (byte i = 0; i < 4; ++i) {
                //        l[i] = new string(line.Trigrams.Where(t => t.A == i).Select(t => alph25[t.B * 5 + t.C]).ToArray());
                //        Console.WriteLine(l[i]);
                //    }
                //}
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
