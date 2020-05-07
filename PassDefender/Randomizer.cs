using System;

namespace PassDefender
{
    class Randomizer
    {
        public string RndString(int countSymbol)
        {
            string rndString = "";
            Random random = new Random();
            int rnd;
            for (int i = 0; i < countSymbol; i++)
            {
                rnd = random.Next(32, 126);
                rndString += (char)rnd;
            }
            return rndString;
        }
    }
}
