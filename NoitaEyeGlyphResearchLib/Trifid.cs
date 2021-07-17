using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoitaEyeGlyphResearchLib {
    public class Trifid {
        // So I coded this and then noticed that it's completely irrelevant for the eye puzzle and simply cannot be applied here.
        // It was fun though!
        public Trifid(byte layers, byte gridSize, string alphabet, bool createLayersHorizontally) {
            if (alphabet.Length != gridSize * gridSize * layers) {
                throw new ArgumentException($"Alphabet of size {alphabet.Length} is invalid for grid size of {gridSize}!");
            }

            Layers = layers;
            GridSize = gridSize;
            TrifidCube = new char[Layers][][];

            for (byte i = 0; i < layers; ++i) {
                TrifidCube[i] = new char[gridSize][];
                for (byte j = 0; j < gridSize; ++j) {
                    TrifidCube[i][j] = new char[gridSize];
                }
            }

            Queue<char> chars = new Queue<char>(alphabet);
            if (createLayersHorizontally) {
                for (byte row = 0; row < gridSize; ++row) {
                    for (byte column = 0; column < gridSize * layers; ++column) {
                        byte layerIndex = (byte)(column / gridSize);
                        byte columnIndex = (byte)(column % gridSize);
                        TrifidCube[layerIndex][row][columnIndex] = chars.Dequeue();
                    }
                }
            } else {
                for (byte layer = 0; layer < layers; ++layer) {
                    for (byte row = 0; row < gridSize; ++row) {
                        for (byte column = 0; column < gridSize; ++column) {
                            TrifidCube[layer][row][column] = chars.Dequeue();
                        }
                    }
                }
            }

            if (chars.Count != 0) {
                throw new ApplicationException("Trifid cube creation unsuccessful. This basically means that the code I wrote is messed up.");
            }
        }

        public byte Layers { get; }

        public char[][][] TrifidCube { get; } // layer, row, column

        public byte GridSize { get; }

        public Tuple<byte, byte, byte> Encode(char c) {
            for (byte layer = 0; layer < Layers; ++layer) {
                for (byte row = 0; row < GridSize; ++row) {
                    for (byte column = 0; column < GridSize; ++column) {
                        if (TrifidCube[layer][row][column] == c) {
                            return new Tuple<byte, byte, byte>(layer, row, column);
                        }
                    }
                }
            }
            throw new ArgumentException($"Character '{c}' is not a part of this Trifid's alphabet!");
        }

        private byte[][] GetEncodeByteMatrix(string s) {
            byte[][] matrix = new byte[Layers][];
            for (byte i = 0; i < Layers; ++i) {
                matrix[i] = new byte[s.Length];
            }
            for (int i = 0; i < s.Length; ++i) {
                Tuple<byte, byte, byte> coords = Encode(s[i]);
                matrix[0][i] = coords.Item1;
                matrix[1][i] = coords.Item2;
                matrix[2][i] = coords.Item3;
            }
            return matrix;
        }

        private byte[][] GetEncodeByteMatrix(TrigramCollection trigrams) {
            byte[][] matrix = new byte[Layers][];
            for (byte i = 0; i < Layers; ++i) {
                matrix[i] = new byte[trigrams.Length];
            }
            for (int i = 0; i < trigrams.Length; ++i) {
                matrix[0][i] = trigrams[i].A;
                matrix[1][i] = trigrams[i].B;
                matrix[2][i] = trigrams[i].C;
            }
            return matrix;
        }

        private byte[][] GetDecodeByteMatrix(string s, int groupSize) {
            byte[][] matrix = new byte[Layers][];
            for (byte i = 0; i < Layers; ++i) {
                matrix[i] = new byte[s.Length];
            }
            Queue<byte> bytes = new Queue<byte>();
            foreach (Tuple<byte, byte, byte> coords in s.Select(Encode)) {
                bytes.Enqueue(coords.Item1);
                bytes.Enqueue(coords.Item2);
                bytes.Enqueue(coords.Item3);
            }
            int groups = s.Length / groupSize;
            for (int group = 0; group < groups; ++group) {
                for (int layer = 0; layer < Layers; ++layer) {
                    for (int i = 0; i < groupSize; ++i) {
                        matrix[layer][group * groupSize + i] = bytes.Dequeue();
                    }
                }
            }
            return matrix;
        }

        private byte[][] GetDecodeByteMatrix(TrigramCollection trigrams, int groupSize) {
            byte[][] matrix = new byte[Layers][];
            for (byte i = 0; i < Layers; ++i) {
                matrix[i] = new byte[trigrams.Length];
            }
            Queue<byte> bytes = new Queue<byte>();
            foreach (Trigram trigram in trigrams) {
                bytes.Enqueue(trigram.A);
                bytes.Enqueue(trigram.B);
                bytes.Enqueue(trigram.C);
            }
            int groups = trigrams.Length / groupSize;
            for (int group = 0; group < groups; ++group) {
                for (int layer = 0; layer < Layers; ++layer) {
                    for (int i = 0; i < groupSize; ++i) {
                        matrix[layer][group * groupSize + i] = bytes.Dequeue();
                    }
                }
            }
            return matrix;
        }

        public string Encode(string s, int groupSize) {
            if (s.Length % groupSize != 0 || groupSize > s.Length) {
                throw new ArgumentException($"Message of length {s.Length} is invalid for a group size of {groupSize}!");
            }

            byte[][] matrix = GetEncodeByteMatrix(s);
            int groups = s.Length / groupSize;
            Queue<byte> encodedBytes = new Queue<byte>();
            for (int group = 0; group < groups; ++group) {
                for (int layer = 0; layer < Layers; ++layer) {
                    for (int i = 0; i < groupSize; ++i) {
                        encodedBytes.Enqueue(matrix[layer][group * groupSize + i]);
                    }
                }
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < s.Length; ++i) {
                builder.Append(TrifidCube[encodedBytes.Dequeue()][encodedBytes.Dequeue()][encodedBytes.Dequeue()]);
            }
            return builder.ToString();
        }

        public string Encode(TrigramCollection trigrams, int groupSize) {
            if (trigrams.Length % groupSize != 0 || groupSize > trigrams.Length) {
                throw new ArgumentException($"Message of length {trigrams.Length} is invalid for a group size of {groupSize}!");
            }

            byte[][] matrix = GetEncodeByteMatrix(trigrams);
            int groups = trigrams.Length / groupSize;
            Queue<byte> encodedBytes = new Queue<byte>();
            for (int group = 0; group < groups; ++group) {
                for (int layer = 0; layer < Layers; ++layer) {
                    for (int i = 0; i < groupSize; ++i) {
                        encodedBytes.Enqueue(matrix[layer][group * groupSize + i]);
                    }
                }
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < trigrams.Length; ++i) {
                builder.Append(TrifidCube[encodedBytes.Dequeue()][encodedBytes.Dequeue()][encodedBytes.Dequeue()]);
            }
            return builder.ToString();
        }

        public string Decode(string s, int groupSize) {
            if (s.Length % groupSize != 0 || groupSize > s.Length) {
                throw new ArgumentException($"Message of length {s.Length} is invalid for a group size of {groupSize}!");
            }
            byte[][] matrix = GetDecodeByteMatrix(s, groupSize);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < s.Length; ++i) {
                builder.Append(TrifidCube[matrix[0][i]][matrix[1][i]][matrix[2][i]]);
            }
            return builder.ToString();
        }

        public string Decode(TrigramCollection trigrams, int groupSize) {
            if (trigrams.Length % groupSize != 0 || groupSize > trigrams.Length) {
                throw new ArgumentException($"Message of length {trigrams.Length} is invalid for a group size of {groupSize}!");
            }
            byte[][] matrix = GetDecodeByteMatrix(trigrams, groupSize);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < trigrams.Length; ++i) {
                builder.Append(TrifidCube[matrix[0][i]][matrix[1][i]][matrix[2][i]]);
            }
            return builder.ToString();
        }
    }
}
