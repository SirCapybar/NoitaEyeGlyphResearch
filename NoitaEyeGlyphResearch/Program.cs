using System;
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
                string[] msgs = {
                    "SWBQWFÖNIVOWWWMG!ED?CNFRB.?MX!FHAMTVKGFWPQQOHQFPHRYLBIDPXXNRNUFKKBJEXDÄUEÄHXETNP.YWX“!DEQLKENVMFYPQ",
                    ".WBQWFÖNIVOWWWMG!ED?CNFRBNDKU!FHAJKWEGFWPQQOHQFPHRQKX!YP“MYYBKZHXCNJY!YOFMFFFYFFOXVOW?DCZWCDVYNQCPYWFOG",
                    "MWBQWFÖNIVOWWWMG!ED?CNFRBZ.UFQW.GYVMR?DEUHPM.CFYNRYXCRLFECNIK!CU!VOWWFDEW!ONKGADNYVEHVVNHGYOBDK?EBWN!JYVEQWTEXJDKNVUOO",
                    ".WBRÖWZOMBOUKQS.MITVPEFERWOJMDUTEFEWF?FQNUV?O.BPXD.!P?OJWGCRWYU!DWWFPFUWKHMLYHLMSCBJENPPK.VY.FDNXGVXUE",
                    "XWBRÖWCUNM?PUÖXXUCVDPGNÄXN.FUF!GLSMPCXQL.C.ÖBUXDFEMVNÖTPUOFDGVCCWFWBWUXYJ“DEHYCWZKXEEKVNEFGXGOYXN!?G?MPFECFBM“XBYZDPEWULGYWGQ?ZBAOEXZJLEI",
                    "OWBRÖWHÄDFMKHQXVNUOMÄCFMDGHPH!OEWFORMT.WEPPFX“FMQEKNKMNYVOTCZWECOPGPPPDFNKLWNANBOTNFMJFFDYB“OYDXDQDXVMGGLSVPDCWRTVFZMÖÄWIKWW",
                    "LWBRÖWCUNMCTFDXGYDMBP!QOBGM.EOUXW.FW“ÖQWTHWYGPTWÖBDPNWYZ“JONZPYSON?K“VEUGUGÖPUXXXORNMV“!DDKWL?KVZMEHKFXOOYDPVUJC.WY!X?H",
                    "!WBRÖWCUNMCTFDXGYDMBPUF.BYTE?LVHBWGLPFÄ!PG?AJVPWYO!HISVKXÖKYWVUSXZGFEMKXBNHPWPVOWHZLWO!XDX“CGFTZÄFBFXNNFUG?QKEEOLBLWOEMK",
                    "NWBRÖWCUNMCTFDXGYDMBPNFXQGM.XOUX“LEWBRQWTUWYZCVGWQXVDOMKLHKEOODKXTOÖWFCUBHJNO.JEG.ETWFMFNCQZENWZCLFYBMVTOWYÄVJXOIN"

                };

                foreach (string msg in msgs) {
                    string alphabet = new string(msg.Distinct().OrderBy(c => c).ToArray());
                    Console.WriteLine(msg.GetIc(alphabet));
                }

                foreach (ReorderParam param in Enum.GetValues(typeof(ReorderParam))) {
                    foreach (TrigramCollection line in tlc.Reorder(param, param).TrigramLines.Take(1)) {
                        //for (byte i = 0; i < 3; ++i) {
                        //    string msg = new string(line.Trigrams.Select(t => t.GetBase10()).Where(n => (n / 25) == i)
                        //        .Select(n => alph25[n % 25]).ToArray());
                        //    Console.WriteLine(msg);
                        //    Console.WriteLine(msg.GetIc(alph25));
                        //}
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
