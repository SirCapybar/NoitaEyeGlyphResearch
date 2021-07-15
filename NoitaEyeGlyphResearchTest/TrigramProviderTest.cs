using Microsoft.VisualStudio.TestTools.UnitTesting;

using NoitaEyeGlyphResearchLib;

namespace NoitaEyeGlyphResearchTest {
    [TestClass]
    public class TrigramProviderTest {
        [TestMethod]
        public void GetStandard_Valid() {
            TrigramLineCollection tlc = new TrigramProvider().GetStandard();
            Assert.AreEqual(new Trigram(2, 0, 0), tlc[0][0]);
            Assert.AreEqual(new Trigram(2, 2, 2), tlc[0][4]);
            Assert.AreEqual(new Trigram(1, 1, 4), tlc[5][0]);
            Assert.AreEqual(new Trigram(0, 2, 1), tlc[5][8]);
        }

        [TestMethod]
        public void GetReversed_Valid() {
            TrigramLineCollection tlc = new TrigramProvider().GetReversed();
            Assert.AreEqual(new Trigram(0, 4, 4), tlc[4][0]);
            Assert.AreEqual(new Trigram(0, 1, 3), tlc[4][1]);
            Assert.AreEqual(new Trigram(1, 4, 3), tlc[0][0]);
            Assert.AreEqual(new Trigram(2, 1, 2), tlc[0][5]);
        }
    }
}