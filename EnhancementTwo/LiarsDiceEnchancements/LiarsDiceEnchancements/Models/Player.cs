using System;
using System.Collections.Generic;
using System.Text;

namespace LiarsDiceEnhancements.Models
{
    class Player
    {
        public string Name { get; set; }
        public int NumberOfDiceGuess { get; set; }
        public int ValueOfDieGuess { get; set; }
        public bool CalledLiar { get; set; }

        private const int MAX_NUMBER_OF_DICE = 5;

        private List<Die> Dice = new List<Die>();
        internal List<Die> DiceList { get => Dice; set => Dice = value; }

        // Default Constructor
        public Player()
        {
            for (int i = 0; i < MAX_NUMBER_OF_DICE; i++)
            {
                DiceList.Add(new Die());
            }
        }
    }
}
