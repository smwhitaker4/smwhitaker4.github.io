using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LiarsDiceApi.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        public string GameId { get; set; }
        public int DiceValueOne { get; set; }
        public int DiceValueTwo { get; set; }
        public int DiceValueThree { get; set; }
        public int DiceValueFour { get; set; }
        public int DiceValueFive { get; set; }
        public int DiceValueSix { get; set; }
        public List<Player> Players { get; set; }
        public Player Winner { get; set; }
    }
}
