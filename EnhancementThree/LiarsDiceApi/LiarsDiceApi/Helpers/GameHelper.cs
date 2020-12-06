using LiarsDiceApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiarsDiceApi.Helpers
{
    public class GameHelper
    {
        private static int MIN_DIE_VALUE { get; } = 1;
        private static int MAX_DIE_VALUE { get; } = 6;
        private static int REQUIRED_PLAYERS { get; } = 3;

        /*
        * This method mimics the action of rolling all sets of dice for each player, and counts
        * the instances of a certain face value being rolled used for end game logic later.
        */
        public static void RollDice(Game game, List<Player> listOfPlayers)
        {
            try
            {
                Random randomDiceGenerator = new Random();

                foreach (Player player in listOfPlayers)
                {
                    // Roll every die each player has
                    foreach (Die die in player.DiceList)
                    {

                        // Die value can only be number 1-6
                        int rollValue = randomDiceGenerator.Next(MIN_DIE_VALUE, MAX_DIE_VALUE);
                        die.DieValue = rollValue;

                        // Track amount of each die value rolled for all dice
                        if (rollValue == 1)
                        {
                            game.DiceValueOne++;
                        }
                        else if (rollValue == 2)
                        {
                            game.DiceValueTwo++;
                        }
                        else if (rollValue == 3)
                        {
                            game.DiceValueThree++;
                        }
                        else if (rollValue == 4)
                        {
                            game.DiceValueFour++;
                        }
                        else if (rollValue == 5)
                        {
                            game.DiceValueFive++;
                        }
                        else
                            game.DiceValueSix++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Dice not rolled. Program - RollDice Error: {0}", e);
            }
        }

        /*
        * Current Player uses input in this method to guess the number of dice rolled for a
        * specific die face value. Validation occurs to make sure the game logic is maintained
        * correctly.
        */
        public static void TakeGuess(List<Player> listOfPlayers)
        {
            var currentPlayer = listOfPlayers[0];
            var previousPlayer = listOfPlayers[listOfPlayers.Count - 1];

            // Record current players number of dice guess
            Console.WriteLine("{0} enter your guess for number of dice: ", currentPlayer.Name);
            int numberOfDieGuess;

            bool waitingForValidInput = true;
            while (waitingForValidInput)
            {
                try
                {
                    numberOfDieGuess = int.Parse(Console.ReadLine().ToString());
                    currentPlayer.NumberOfDiceGuess = numberOfDieGuess;
                    waitingForValidInput = false;
                }
                catch (Exception)
                {
                    Console.WriteLine("Please make sure you enter a valid number");
                }
            }

            // Record current players number of dice guess
            Console.WriteLine("{0} enter your die value for your guess: ", currentPlayer.Name);
            int valueOfDieGuess;

            waitingForValidInput = true;
            while (waitingForValidInput)
            {
                try
                {
                    valueOfDieGuess = int.Parse(Console.ReadLine().ToString());
                    currentPlayer.ValueOfDieGuess = valueOfDieGuess;
                    waitingForValidInput = false;
                }
                catch (Exception)
                {
                    Console.WriteLine("Please make sure you enter a valid number");
                }
            }

            // Validate that the rules of guessing have not been broken
            VerifyRules(listOfPlayers);

            Console.WriteLine("{0} has guess {1} count of die value {2}.", currentPlayer.Name,
                currentPlayer.NumberOfDiceGuess, currentPlayer.ValueOfDieGuess);
        }

        /*
        * This method is used to make sure the current player is guessing larger number of dice than previous player
        * and that the value of the dice they guess is possible.
        */
        private static void VerifyRules(List<Player> listOfPlayers)
        {
            var currentPlayer = listOfPlayers[0];
            var previousPlayer = listOfPlayers[listOfPlayers.Count - 1];

            // Loop until player provides a number of dice guess that meets the rule requirement
            while (currentPlayer.NumberOfDiceGuess <= previousPlayer.NumberOfDiceGuess)
            {
                Console.WriteLine("You must enter a value larger than {0}'s guess of {1}: ", previousPlayer.Name,
                    previousPlayer.NumberOfDiceGuess);
                int numberOfDieGuess;
                try
                {
                    numberOfDieGuess = int.Parse(Console.ReadLine().ToString());
                    currentPlayer.NumberOfDiceGuess = numberOfDieGuess;
                }
                catch (Exception)
                {
                    Console.WriteLine("Please make sure you enter a valid number");
                }
            }

            // Loop until player provides a value for die that is valid
            while (currentPlayer.ValueOfDieGuess > MAX_DIE_VALUE || currentPlayer.ValueOfDieGuess < MIN_DIE_VALUE)
            {
                Console.WriteLine("You must enter a valid dice number. {0} is not valid: ", currentPlayer.ValueOfDieGuess);
                int valueOfDieGuess;

                try
                {
                    valueOfDieGuess = int.Parse(Console.ReadLine().ToString());
                    currentPlayer.ValueOfDieGuess = valueOfDieGuess;
                }
                catch (Exception)
                {
                    Console.WriteLine("Please make sure you enter a valid number");
                }
            }
        }


        /*
        * After current Player makes guess, the program cycles through each available
        * Player and asks if they want to call liar based on the user's guess.
        * 
        * Returns a Player object based on who has called lair. If no one call liar current
        * user is returned.
        */
        public static Player CallLiar(List<Player> listOfPlayers)
        {
            var currentPlayer = listOfPlayers[0];
            var nextPlayer = listOfPlayers[1];

            try
            {
                string callLiar;
                Console.WriteLine("Would {0} like to call liar? (y/n)", nextPlayer.Name);

                callLiar = Console.ReadLine().ToString();
                if (callLiar == "y")
                {
                    // Change player's called liar status to true
                    nextPlayer.CalledLiar = true;
                    Console.WriteLine("{0} has called liar!", nextPlayer.Name);
                    return nextPlayer;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Call liar failed. Program - CallLiar Error: {0}", e);
            }

            // Return current player if no one calls liar
            return currentPlayer;
        }

        /*
        * This method holds the logic to end the game loop and also how to order the current, previous, and
        * next player.
        * 
        * Parameter is the returned Player object from the CallLiar() method. The code will only follow
        * the end game logic if the passed Player is not the current player.
        * 
        * Returns a boolean value that signals the game loop to end or continue
        */
        public static bool EndGame(Player playerWhoCalledLiar, Game game, List<Player> listOfPlayers)
        {
            var currentPlayer = listOfPlayers[0];

            // Only enter end game loop if player who called liar is not current player
            if (playerWhoCalledLiar.Name != currentPlayer.Name)
            {
                // Only enter loop if current player was lying
                if (CheckLiar(game, listOfPlayers))
                {
                    Console.WriteLine("{0} was lying. {1} wins!", currentPlayer.Name, playerWhoCalledLiar.Name);
                    return true;
                }
                // Calling player was not correct
                else
                {
                    Console.WriteLine("{0} was not lying. {0} wins!", currentPlayer.Name);
                    return true;
                }
            }

            // If no one called liar then move current player to end of list and next player becomes current player
            else
            {
                listOfPlayers.Add(listOfPlayers[0]);
                listOfPlayers.RemoveAt(0);
                return false;
            }
        }

        /*
        * This method validates whether current player was lying
        * Enter first conditional based on value of dice player called liar on
        * Enter second conditional if current players guess was more than actual die value count
        * 
        * Returns a boolean response
        * If true is returned current player is a liar
        * If false is returned current player was not a liar
        */
        private static bool CheckLiar(Game game, List<Player> listOfPlayers)
        {
            var currentPlayer = listOfPlayers[0];

            if (currentPlayer.ValueOfDieGuess == 1)
            {
                Console.WriteLine("Count for dice with value of 1: {0}", game.DiceValueOne);
                if (currentPlayer.NumberOfDiceGuess > game.DiceValueOne)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (currentPlayer.ValueOfDieGuess == 2)
            {
                Console.WriteLine("Count for dice with value of 2: {0}", game.DiceValueTwo);
                if (currentPlayer.NumberOfDiceGuess > game.DiceValueTwo)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (currentPlayer.ValueOfDieGuess == 3)
            {
                Console.WriteLine("Count for dice with value of 3: {0}", game.DiceValueThree);
                if (currentPlayer.NumberOfDiceGuess > game.DiceValueThree)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (currentPlayer.ValueOfDieGuess == 4)
            {
                Console.WriteLine("Count for dice with value of 4: {0}", game.DiceValueFour);
                if (currentPlayer.NumberOfDiceGuess > game.DiceValueFour)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (currentPlayer.ValueOfDieGuess == 5)
            {
                Console.WriteLine("Count for dice with value of 5: {0}", game.DiceValueFive);
                if (currentPlayer.NumberOfDiceGuess > game.DiceValueFive)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Count for dice with value of 6: {0}", game.DiceValueSix);
                if (currentPlayer.NumberOfDiceGuess > game.DiceValueSix)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
