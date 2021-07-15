using System;
using System.Collections.Generic;
using System.Text;

namespace NoitaEyeGlyphResearchLib {
    public class Trigram : IComparable<Trigram> {
        private byte _A;
        private byte _B;
        private byte _C;

        public Trigram(int base10) {
            base10 = (base10 % 125 + 125) % 125;
            A = (byte)(base10 / 25);
            base10 -= A * 25;
            B = (byte)(base10 / 5);
            base10 -= B * 5;
            C = (byte)base10;
        }

        public Trigram(Trigram trigram) {
            A = trigram.A;
            B = trigram.B;
            C = trigram.C;
        }

        public Trigram(byte a, byte b, byte c) {
            A = a;
            B = b;
            C = c;
        }

        public byte A {
            get { return _A; }
            private set {
                if (value > 4) {
                    throw new ApplicationException("Trigram value overflow");
                }
                _A = value;
            }
        }

        public byte B {
            get { return _B; }
            private set {
                if (value > 4) {
                    throw new ApplicationException("Trigram value overflow");
                }
                _B = value;
            }
        }

        public byte C {
            get { return _C; }
            private set {
                if (value > 4) {
                    throw new ApplicationException("Trigram value overflow");
                }
                _C = value;
            }
        }

        public void SwapAB() {
            byte a = A;
            A = B;
            B = a;
        }

        public void SwapAC() {
            byte a = A;
            A = C;
            C = a;
        }

        public void SwapBC() {
            byte b = B;
            B = C;
            C = b;
        }

        public void RotateCW() {
            byte a = A;
            A = B;
            B = C;
            C = a;
        }

        public void RotateCCW() {
            byte a = A;
            A = C;
            C = B;
            B = a;
        }

        public override bool Equals(object obj) {
            return obj is Trigram other && Equals(other);
        }

        public bool Equals(Trigram other) {
            return A == other.A && B == other.B && C == other.C;
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = A.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                hashCode = (hashCode * 397) ^ C.GetHashCode();
                return hashCode;
            }
        }

        public static Trigram operator +(Trigram trigram, int value) {
            int val = trigram.GetBase10();
            val += value;
            return new Trigram(val);
        }

        public static Trigram operator -(Trigram trigram, int value) {
            return trigram + -value;
        }

        public static Trigram operator +(Trigram left, Trigram right) {
            return left + right.GetBase10();
        }

        public static Trigram operator -(Trigram left, Trigram right) {
            return left - right.GetBase10();
        }

        public static bool operator ==(Trigram left, Trigram right) {
            return (ReferenceEquals(left, null) && ReferenceEquals(right, null)) || !ReferenceEquals(left, null) && !ReferenceEquals(right, null) && left.Equals(right);
        }

        public static bool operator !=(Trigram left, Trigram right) {
            return !(ReferenceEquals(left, null) && ReferenceEquals(right, null)) && (ReferenceEquals(left, null) || !left.Equals(right));
        }

        public static bool operator >(Trigram left, Trigram right) {
            if (left.A > right.A) {
                return true;
            }
            if (left.A < right.A) {
                return false;
            }
            if (left.B > right.B) {
                return true;
            }
            if (left.B < right.B) {
                return false;
            }
            return left.C > right.C;
        }

        public static bool operator <(Trigram left, Trigram right) {
            if (left.A < right.A) {
                return true;
            }
            if (left.A > right.A) {
                return false;
            }
            if (left.B < right.B) {
                return true;
            }
            if (left.B > right.B) {
                return false;
            }
            return left.C < right.C;
        }

        public override string ToString() {
            return $"{A}{B}{C}";
        }

        public char ToChar(int offset) {
            return (char)(GetBase10() + offset);
        }

        public int CompareTo(Trigram other) {
            int aComparison = A.CompareTo(other.A);
            if (aComparison != 0) {
                return aComparison;
            }

            int bComparison = B.CompareTo(other.B);
            return bComparison != 0 ? bComparison : C.CompareTo(other.C);
        }

        public int GetBase10() {
            return C + B * 5 + A * 25;
        }

        public int GetBase10(byte[] mappings) {
            return mappings[C] + mappings[B] * 5 + mappings[A] * 25;
        }

        public Trigram Reorder(ReorderParam param) {
            Trigram result = new Trigram(A, B, C);
            switch (param) {
                case ReorderParam.BAC:
                    result.SwapAB();
                    break;
                case ReorderParam.CBA:
                    result.SwapAC();
                    break;
                case ReorderParam.ACB:
                    result.SwapBC();
                    break;
                case ReorderParam.BCA:
                    result.RotateCW();
                    break;
                case ReorderParam.CAB:
                    result.RotateCCW();
                    break;
            }
            return result;
        }

        public byte GetSum() {
            return (byte)(A + B + C);
        }

        public byte GetSum(byte[] mappings) {
            return (byte)(mappings[A] + mappings[B] + mappings[C]);
        }

        public string ToBinaryString(bool invert) {
            StringBuilder builder = new StringBuilder();
            for (byte i = 0; i < A; ++i) {
                builder.Append(invert ? '1' : '0');
            }
            for (byte i = 0; i < B; ++i) {
                builder.Append(invert ? '0' : '1');
            }
            for (byte i = 0; i < C; ++i) {
                builder.Append(invert ? '1' : '0');
            }
            return builder.ToString();
        }

        public string ToBinaryString(bool invert, byte[] mappings) {
            StringBuilder builder = new StringBuilder();
            for (byte i = 0; i < mappings[A]; ++i) {
                builder.Append(invert ? '1' : '0');
            }
            for (byte i = 0; i < mappings[B]; ++i) {
                builder.Append(invert ? '0' : '1');
            }
            for (byte i = 0; i < mappings[C]; ++i) {
                builder.Append(invert ? '1' : '0');
            }
            return builder.ToString();
        }

        public void InvertA() {
            A = EyeInversions[A];
        }

        public void InvertB() {
            B = EyeInversions[B];
        }

        public void InvertC() {
            C = EyeInversions[C];
        }

        public void Invert() {
            A = EyeInversions[A];
            B = EyeInversions[B];
            C = EyeInversions[C];
        }

        public void InvertAC() {
            A = EyeInversions[A];
            C = EyeInversions[C];
        }

        public int GetPolybiusValue() {
            return Statics.PolybiusCube[A][B][C];
        }

        public byte GetDiamondCypherValue(bool reverse) {
            int x = 3, y = x;
            if (!reverse) {
                Tuple<int, int> off = DiamondCypherOffsets[A];
                x += off.Item1;
                y += off.Item2;
                off = DiamondCypherOffsets[B];
                x += off.Item1;
                y += off.Item2;
                off = DiamondCypherOffsets[C];
                x += off.Item1;
                y += off.Item2;
            } else {
                Tuple<int, int> off = DiamondCypherOffsets[A];
                x -= off.Item1;
                y -= off.Item2;
                off = DiamondCypherOffsets[B];
                x -= off.Item1;
                y -= off.Item2;
                off = DiamondCypherOffsets[C];
                x -= off.Item1;
                y -= off.Item2;
            }

            int value = Statics.DiamondMatrix[x][y];
            if (value == 0) {
                throw new ArgumentOutOfRangeException();
            }
            return (byte)value;
        }

        private Dictionary<byte, Tuple<int, int>> DiamondCypherOffsets { get; } = new Dictionary<byte, Tuple<int, int>> {
            { 0, new Tuple<int, int>(0, 0) },
            { 1, new Tuple<int, int>(0, -1) },
            { 2, new Tuple<int, int>(1, 0) },
            { 3, new Tuple<int, int>(0, 1) },
            { 4, new Tuple<int, int>(-1, 0) }
        };

        private Dictionary<byte, byte> EyeInversions { get; } = new Dictionary<byte, byte> {
            { 0, 0 },
            { 1, 3 },
            { 3, 1 },
            { 2, 4 },
            { 4, 2 }
        };
    }
}