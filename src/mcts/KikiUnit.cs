using System;
using System.Runtime.CompilerServices;

namespace KikiProject
{
    public class KikiUnit
    {
        public enum Unit
        {
            Null,
            Single,
            Horizontal,
            Vertical
        }
    
        public static String Player = "P";
        public static String Goal = "G";
        public static string Win = "W";

        public Unit Key;

        public KikiUnit(int key)
        {
            this.Key = (Unit) key;
        }

        public KikiUnit(char unit)
        {
            this.Key = (Unit) (unit - '0');
        }
        
        public override string ToString()
        {
            return this.Key.ToString();
        }
    }
}