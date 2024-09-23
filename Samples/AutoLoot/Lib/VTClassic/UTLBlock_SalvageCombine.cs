///////////////////////////////////////////////////////////////////////////////
//File: UTLBlock_SalvageCombine.cs
//
//Description: A UTL file block for storing salvage combination ranges.
//  This file is shared between the VTClassic Plugin and the VTClassic Editor.
//
//This file is Copyright (c) 2010 VirindiPlugins
//
//The original copy of this code can be obtained from http://www.virindi.net/repos/virindi_public
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////


#if VTC_PLUGIN
#endif

namespace AutoLoot.Lib.VTClassic
{
    internal class UTLBlock_SalvageCombine : IUTLFileBlockHandler
    {
        const int SALVAGEBLOCK_FILE_FORMAT_VERSION = 1;
        static Random rand = new Random();

        public string DefaultCombineString = "";
        public Dictionary<int, string> MaterialCombineStrings = new Dictionary<int, string>();
        public Dictionary<int, int> MaterialValueModeValues = new Dictionary<int, int>();

        public UTLBlock_SalvageCombine()
        {
            //Default rules
            DefaultCombineString = "1-6, 7-8, 9, 10";

            string imbs = "1-10";
            //Magic item tinkering
            MaterialCombineStrings[GameInfo.GetMaterialID("Agate")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Azurite")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Black Opal")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Bloodstone")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Carnelian")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Citrine")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Fire Opal")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Hematite")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Lavender Jade")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Malachite")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Red Jade")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Rose Quartz")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Sunstone")] = imbs;

            //Weapon tinkering
            MaterialCombineStrings[GameInfo.GetMaterialID("White Sapphire")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Red Garnet")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Jet")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Imperial Topaz")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Emerald")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Black Garnet")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Aquamarine")] = imbs;

            //Armor tinkering
            MaterialCombineStrings[GameInfo.GetMaterialID("Zircon")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Yellow Topaz")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Peridot")] = imbs;

            //Other
            MaterialCombineStrings[GameInfo.GetMaterialID("Leather")] = imbs;
            MaterialCombineStrings[GameInfo.GetMaterialID("Ivory")] = imbs;
        }

        #region CombineString Parsing
        struct sDoublePair
        {
            public double a;
            public double b;
        }
        static int GetRangeIndex(List<sDoublePair> Ranges, double val)
        {
            for (int i = 0; i < Ranges.Count; ++i)
            {
                //Gaps in ranges go to the previous range
                if (Ranges[i].a > val)
                    return i - 1;

                //If we fall into this range, choose it
                if (Ranges[i].a <= val && Ranges[i].b >= val)
                    return i;
            }
            return Ranges.Count;
        }

        static List<sDoublePair> ParseCombineSting(string pcombinestring)
        {
            List<sDoublePair> Ranges = new List<sDoublePair>();

            //Look through the string and delete all characters we don't understand
            string combinestring = pcombinestring;
            for (int i = combinestring.Length - 1; i >= 0; --i)
            {
                if (char.IsDigit(combinestring[i])) continue;
                if (combinestring[i] == ',') continue;
                if (combinestring[i] == ';') continue;
                if (combinestring[i] == '-') continue;
                if (combinestring[i] == '.') continue;

                combinestring.Remove(i, 1);
            }

            //Split and parse string into ranges
            string[] toks = combinestring.Split(';', ',');
            foreach (string tok in toks)
            {
                if (tok.Length == 0) continue;
                string[] numbers = tok.Split('-');
                if (numbers.Length == 0) continue;

                sDoublePair addpair = new sDoublePair();
                if (numbers.Length == 1)
                {
                    addpair.a = double.Parse(numbers[0], System.Globalization.CultureInfo.InvariantCulture);
                    addpair.b = addpair.a;
                }
                else
                {
                    addpair.a = double.Parse(numbers[0], System.Globalization.CultureInfo.InvariantCulture);
                    addpair.b = double.Parse(numbers[1], System.Globalization.CultureInfo.InvariantCulture);
                }
                Ranges.Add(addpair);
            }

            return Ranges;
        }

        /*
        static bool TestCombineString(double w1, double w2, string pcombinestring)
        {
			List<sDoublePair> Ranges = ParseCombineSting(pcombinestring);

            //Find out which range we fall into
            return (GetRangeIndex(Ranges, w1) == GetRangeIndex(Ranges, w2));
        }
		 * */
        #endregion CombineString Parsing
        /*
        public bool CanCombineBags(double bag1workmanship, double bag2workmanship, int material)
        {
            if (MaterialCombineStrings.ContainsKey(material))
                return TestCombineString(bag1workmanship, bag2workmanship, MaterialCombineStrings[material]);
            else
                return TestCombineString(bag1workmanship, bag2workmanship, DefaultCombineString);
        }
		*/

        string GetCombineString(int material)
        {
            if (MaterialCombineStrings.ContainsKey(material))
                return MaterialCombineStrings[material];
            else
                return DefaultCombineString;
        }

        public List<int> TryCombineMultiple(List<WorldObject> availablebags)
        {
            if (availablebags.Count == 0) return new List<int>();

            var material = (int)(availablebags[0].MaterialType ?? 0);
            List<sDoublePair> ranges = ParseCombineSting(GetCombineString(material));

            //Bin the available bags by which part of the combine string they fit in.
            Dictionary<int, List<WorldObject>> binnedbags = new Dictionary<int, List<WorldObject>>();
            foreach (WorldObject zz in availablebags)
            {
                int bin = GetRangeIndex(ranges, zz.Workmanship ?? 0);
                if (!binnedbags.ContainsKey(bin)) binnedbags.Add(bin, new List<WorldObject>());
                binnedbags[bin].Add(zz);
            }

            foreach (KeyValuePair<int, List<WorldObject>> kp in binnedbags)
            {
                if (kp.Value.Count < 2) continue;

                //We now have a list of every piece of salvage that can be combined. What should we combine?
                //Finding the best possible combination of bags is equivalent to the knapsack problem, an NP-complete computational problem.
                //That is a bit much for something that is called all the time, so we will just use stupid methods of choosing bags.
                List<int> ret = new List<int>();

                if (MaterialValueModeValues.ContainsKey(material))
                {
                    //Salvage for money mode.

                    //First, we see if we can cap out a bag.
                    int valuemodevalue = MaterialValueModeValues[material];
                    int vsum = 0;
                    int csum = 0;

                    foreach (WorldObject ii in kp.Value)
                    {
                        ret.Add((int)ii.Guid.Full);
                        vsum += ii.Value ?? 0;
                        csum += ii.Structure ?? 0;
                    }

                    //If we are above the value, combine.
                    if (vsum >= valuemodevalue)
                        return ret;

                    //Total bags are below target value. Try combining some without exceeding 100.
                    //In theory this is a hard problem. So to avoid wasting time, we will just randomly try a few possibilities.

                    for (int i = 0; i < 12; ++i)
                    {
                        int ind1 = rand.Next(kp.Value.Count);
                        int ind2 = rand.Next(kp.Value.Count);
                        if (ind1 == ind2) continue;

                        if ((kp.Value[ind1].Structure ?? 0) + (kp.Value[ind2].Structure ?? 0) < 100)
                        {
                            //This pair is good.
                            ret.Clear();
                            ret.Add((int)kp.Value[ind1].Guid.Full);
                            ret.Add((int)kp.Value[ind2].Guid.Full);
                            return ret;
                        }
                    }
                }
                else
                {
                    //Salvage for maximum bags mode.
                    //Just loop and add bags until we are over 100 units.

                    int csum = 0;

                    foreach (WorldObject ii in kp.Value)
                    {
                        ret.Add((int)ii.Guid.Full);
                        csum += ii.Structure ?? 0;
                        if (csum >= 100) break;
                    }

                    return ret;
                }
            }

            return new List<int>();
        }

        public string BlockTypeID
        {
            get { return "SalvageCombine"; }
        }

        public void Read(StreamReader inf, int len)
        {
            MaterialValueModeValues.Clear();

            string formatversion = inf.ReadLine();

            DefaultCombineString = inf.ReadLine();

            MaterialCombineStrings.Clear();
            int nummatstrings = int.Parse(inf.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
            for (int i = 0; i < nummatstrings; ++i)
            {
                int mat = int.Parse(inf.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
                string cmb = inf.ReadLine();
                MaterialCombineStrings[mat] = cmb;
            }

            //This is all there is for older blocks.
            if (inf.EndOfStream) return;

            //MaterialValueModeValues.Clear();
            int nummatvalmodevals = int.Parse(inf.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
            for (int i = 0; i < nummatvalmodevals; ++i)
            {
                int k = int.Parse(inf.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
                int v = int.Parse(inf.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
                MaterialValueModeValues[k] = v;
            }
        }

        public void Write(CountedStreamWriter inf)
        {
            inf.WriteLine(SALVAGEBLOCK_FILE_FORMAT_VERSION);

            inf.WriteLine(DefaultCombineString);

            inf.WriteLine(MaterialCombineStrings.Count);
            foreach (KeyValuePair<int, string> kp in MaterialCombineStrings)
            {
                inf.WriteLine(kp.Key);
                inf.WriteLine(kp.Value);
            }

            inf.WriteLine(MaterialValueModeValues.Count);
            foreach (KeyValuePair<int, int> kp in MaterialValueModeValues)
            {
                inf.WriteLine(kp.Key);
                inf.WriteLine(kp.Value);
            }

        }
    }
}