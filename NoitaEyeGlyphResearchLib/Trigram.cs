using System;
using System.ComponentModel;

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
    }
}