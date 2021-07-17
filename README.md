# Noita Eye Glyph Research

The purpose of this solution is processing of the eye message data from a yet unsolved secret from Noita game. If you don't know what this puzzle is about, make sure to check out the following docs:

- [Noita Eye Glyph Messages](https://docs.google.com/document/d/1s6gxrc1iLJ78iFfqC2d4qpB9_r_c5U5KwoHVYFFrjy0/edit?usp=sharing): puzzle introduction and basic info
- [Noita Eye Data](https://docs.google.com/spreadsheets/d/195Rtc8kj4b74LtIyakqGP-iHhm36vyT5i8w7H5JjOV8/edit?usp=sharing): useful data sets, data from which are utilised within this project
- [Capybar#6875: Noita eye room research](https://docs.google.com/document/d/1CT4VW_A20peJBt49F93sQEbnrYogcnO_igvjAtzYpyo/edit?usp=sharing): my documentation of thoughts, trials and errors

## Content

This project operates on *trigrams*. If you're not into the trigram theory, you *may* or *may not* find it useful.
All of the functionality is documented within the code, but as a summary (which might be incomplete/outdated):
- various trigram representations, summaries and data analysis
- easy processing of individual trigrams, separate eye messages or all of the messages combined
- index of coincidence, frequency analysis, data conversions
- Vigenere/caesar cyphers for both text and trigrams
- diamond cypher
- trifid cypher (which is useless here, btw)
- polybius cube
- and probably more cryptanalysis will come soon!

## Usage

You can find most of the functionality within either the trigram collection classes (**TrigramCollection** or **TrigramLineCollection**) or the **Statics**.
- **TrigramCollection** represents a single trigram text, like a single eye message
- **TrigramLineCollection** is basically a collection of TrigramCollections (usually just contains all of the 9 messages)
- **Statics** contain a lot of extension methods and a bunch of other things

You can get all the eye messages easily with **TrigramProvider**.

Here come some examples of basic data analysis:
- Calculating index of coincidence of the first eye message:
``` C#
TrigramProvider tp = new TrigramProvider();
TrigramLineCollection tlc = tp.GetStandard();
Console.WriteLine(tlc[0].GetIc());
```
- Gathering combined trigram frequency data of all eye messages and printing out unique trigram count, all of the unique trigram values and occurrency count of each value:
``` C#
TrigramProvider tp = new TrigramProvider();
TrigramLineCollection tlc = tp.GetStandard();
Console.WriteLine(tlc.GetFrequencyData().GetFrequencyMessage());
```

Although I added documentation wherever appropriate, I am aware that the code might not be easily usable for 'outsiders' as I did not plan to make it public. Hence I am open for suggestions as well as ready to help anyone who struggles with something within my project.

You can always contact me at Noita's discord in the eye room (silma-houne) or directly: **Capybar#6875**.