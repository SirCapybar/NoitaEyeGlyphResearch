using Microsoft.VisualStudio.TestTools.UnitTesting;

using NoitaEyeGlyphResearchLib;

namespace NoitaEyeGlyphResearchTest {
    [TestClass]
    public class PolybiusCubeTest {
        [TestMethod]
        public void PolybiusCube_Values_Valid() {
            Assert.AreEqual(0, Statics.PolybiusCube[0][0][0]);
            Assert.AreEqual(5, Statics.PolybiusCube[0][1][0]);
            Assert.AreEqual(5, Statics.PolybiusCube[1][0][0]);
            Assert.AreEqual(11, Statics.PolybiusCube[1][1][1]);
            Assert.AreEqual(17, Statics.PolybiusCube[1][2][2]);
            Assert.AreEqual(22, Statics.PolybiusCube[2][2][2]);
            Assert.AreEqual(14, Statics.PolybiusCube[2][0][4]);
            Assert.AreEqual(22, Statics.PolybiusCube[2][2][2]);
            Assert.AreEqual(19, Statics.PolybiusCube[4][4][4]);
            //Assert.AreEqual(32, Statics.PolybiusCube[2][4][2]);
            //Assert.AreEqual(124, Statics.PolybiusCube[4][4][4]);
        }
    }
}