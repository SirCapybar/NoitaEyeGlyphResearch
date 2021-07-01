using Microsoft.VisualStudio.TestTools.UnitTesting;

using NoitaEyeGlyphResearchLib;

namespace NoitaEyeGlyphResearchTest {
    [TestClass]
    public class TrigramTest {
        [TestMethod]
        public void Base10Construction_Valid() {
            Trigram t = new Trigram(0);
            Assert.AreEqual(0, t.A);
            Assert.AreEqual(0, t.B);
            Assert.AreEqual(0, t.C);
            t = new Trigram(10);
            Assert.AreEqual(0, t.A);
            Assert.AreEqual(2, t.B);
            Assert.AreEqual(0, t.C);
            t = new Trigram(125);
            Assert.AreEqual(0, t.A);
            Assert.AreEqual(0, t.B);
            Assert.AreEqual(0, t.C);
            t = new Trigram(-1);
            Assert.AreEqual(4, t.A);
            Assert.AreEqual(4, t.B);
            Assert.AreEqual(4, t.C);
            t = new Trigram(86);
            Assert.AreEqual(3, t.A);
            Assert.AreEqual(2, t.B);
            Assert.AreEqual(1, t.C);
        }

        [TestMethod]
        public void Base10Conversion_Valid() {
            for (int i = -200; i < 200; ++i) {
                Trigram t = new Trigram(i);
                Assert.AreEqual((i % 125 + 125) % 125, t.GetBase10());
            }
        }

        [TestMethod]
        public void ComparisonOperators_Valid() {
            Trigram
                a = new Trigram(0, 0, 0),
                b = new Trigram(4, 4, 4),
                c = new Trigram(2, 4, 4),
                d = new Trigram(3, 0, 0),
                e = new Trigram(2, 4, 4);
            Assert.IsTrue(a != b);
            Assert.IsTrue(b != c);
            Assert.IsTrue(c != d);
            Assert.IsTrue(d != e);
            Assert.IsFalse(c != e);
            Assert.IsTrue(c == e);

            Assert.IsFalse(a > b);
            Assert.IsTrue(b > a);
            Assert.IsFalse(c > e);
            Assert.IsTrue(b > c);
            Assert.IsTrue(c < d);
        }
    }
}
