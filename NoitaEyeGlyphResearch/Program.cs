using System.Text;

using NoitaEyeGlyphResearchLib;

namespace NoitaEyeGlyphResearch {
    class Program {
        private static void Main() {
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            const int MIN_ASCII_INCL = 32, MAX_ASCII_EXCL = 127;
            Encoding encoding = Encoding.ASCII;
            const int KEY_COUNT = 35;
            const string alph25 = "abcdefghiklmnopqrstuvwxyz";
#pragma warning restore CS0219 // Variable is assigned but its value is never used

            TrigramProvider tp = new TrigramProvider();
            TrigramLineCollection tlc = tp.GetStandard();
        }
    }
}
