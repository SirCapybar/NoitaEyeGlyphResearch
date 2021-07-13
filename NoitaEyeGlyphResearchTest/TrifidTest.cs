using Microsoft.VisualStudio.TestTools.UnitTesting;

using NoitaEyeGlyphResearchLib;

namespace NoitaEyeGlyphResearchTest {
    [TestClass]
    public class TrifidTest {
        [TestMethod]
        public void Trifid_LayerByLayer_Valid() {
            const string alphabet = "FELIXMARDSTBCGHJKNOPQUVWYZ+";
            Trifid trifid = new Trifid(3, 3, alphabet, false);
            const string message = "AIDETOILECIELTAIDERA";
            string encoded = trifid.Encode(message, 5);
            Assert.AreEqual("FMJFVOISSUFTFPUFEQQC", encoded);
            string decoded = trifid.Decode(encoded, 5);
            Assert.AreEqual(message, decoded);
        }
        [TestMethod]
        public void Trifid_RowByRow_Valid() {
            const string alphabet = "ABCD.EFGHIJKLMNOPQRSTUVWXYZ";
            Trifid trifid = new Trifid(3, 3, alphabet, true);
            const string message = "WIKIPEDIA";
            string encoded = trifid.Encode(message, 9);
            Assert.AreEqual("DSDPLIHKA", encoded);
            string decoded = trifid.Decode(encoded, 9);
            Assert.AreEqual(message, decoded);
        }
    }
}