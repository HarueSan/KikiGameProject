using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace KikiProject

{
    public class KikiUnitList : List<KikiUnit>
    {
        public KikiUnitList(string unitListString)
        {
            foreach (char c in unitListString)
            {
                KikiUnit unit = new KikiUnit(c);
                this.Add(unit);
            }
            // sort string for preventing duplicate unit list.
            Sort((a,b) => a.Key - b.Key);
        }

        public string Encode()
        {
            string output = "";
            foreach (KikiUnit kikiUnits in this)
            {
                output = output + ((int) kikiUnits.Key).ToString();
            }

            return output;
        }

        public override string ToString()
        {
            return Encode();
        }
    }
}