using LiarsDiceEnhancements.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace LiarsDiceEnhancements
{
    class Program
    {

        private static int numberOfPlayers;
        private const int REQUIRED_PLAYERS = 3;
        private static List<Player> listOfPlayers = new List<Player>();
        private static int diceWithValueOne = 0, diceWithValueTwo = 0, diceWithValueThree = 0,
            diceWithValueFour = 0, diceWithValueFive = 0, diceWithValueSix = 0;
        private const int MIN_DIE_VALUE = 1;
        private const int MAX_DIE_VALUE = 6;
        private const string RULES_FILE = "LiarsDiceRules.txt";
        private static bool continueGameLoop = true;

        static void Main(string[] args)
        {
            // Provide rules before game starts
            ReadRuleFile();

            // Enter and validate number of players
            PlayerCheck();

            // Initialize all player objects
            CreatePlayers();

            // Begin game logic
            while (continueGameLoop)
            {
                StartGame();
            }
        }

        private static void ReadRuleFile()
        {
            try
            {
                if (File.Exists(RULES_FILE))
                {
                    var rulesText = File.ReadAllText(RULES_FILE);
                    Console.WriteLine(rulesText);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading rules file. Program - ReadRuleFile Error: {0}", e);
            }
        }

        /*
		* Method used for verifying there are enough players in to start the game.
		* The user will be continuously prompted for a valid amount of numbers
		* before the game proceeds.
		*/
        private static void PlayerCheck()
        {
            // Loop until valid input is received
            bool waitingForValidInput = true;
            while (waitingForValidInput)
            {
                Console.WriteLine("\n\nHow many players? (must be more than 2): ");
                try
                {
                    numberOfPlayers = int.Parse(Console.ReadLine().ToString());
                    waitingForValidInput = false;
                }
                catch (Exception)
                {
                    Console.WriteLine("Please make sure you enter a valid number");
                }
            }

            while (numberOfPlayers < REQUIRED_PLAYERS)
            {
                Console.WriteLine("Must be {0} or more players to play. Enter a different player amount: ", REQUIRED_PLAYERS);

                try
                {
                    numberOfPlayers = int.Parse(Console.ReadLine().ToString());
                }
                catch (Exception)
                {
                    Console.WriteLine("Please make sure you enter a valid number");
                }
            };
        }

        /*
		* Based on the nubmer of players indicated this method will instantiate a
		* Player object for each player.
		*/
        private static void CreatePlayers()
        {
            for (int i = 1; i <= numberOfPlayers; i++)
            {
                var newPlayerName = "";
                Console.WriteLine("Please enter player name: ");

                try
                {
                    newPlayerName = Console.ReadLine().ToString();
                    Player newPlayer = new Player();
                    newPlayer.Name = newPlayerName;
                    listOfPlayers.Add(newPlayer);
                }
                catch (Exception e)
                {
                    Console.WriteLine("New player not created. Program - CreatePlayers Error: {0}", e);
                }
            }
        }

        /*
		* Game loop starts with this method. All subsequent code holds logic and validation for
		* how the game is played.
		*/
        private static void StartGame()
        {

            // Dice are cast for all players
            RollDice();

            do
            {
                // First player starts with a guess before entering the loop
                TakeGuess();
            } while (!EndGame(CallLiar()));

            // Prompt to play again
            Rematch();
        }

        /*
		* This method mimics the action of rolling all sets of dice for each player, and counts
		* the instances of a certain face value being rolled used for end game logic later.
		*/
        private static void RollDice()
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
                            diceWithValueOne++;
                        }
                        else if (rollValue == 2)
                        {
                            diceWithValueTwo++;
                        }
                        else if (rollValue == 3)
                        {
                            diceWithValueThree++;
                        }
                        else if (rollValue == 4)
                        {
                            diceWithValueFour++;
                        }
                        else if (rollValue == 5)
                        {
                            diceWithValueFive++;
                        }
                        else
                            diceWithValueSix++;
                    }
                }
                Console.WriteLine("Dice have been rolled!");
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
        private static void TakeGuess()
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
            VerifyRules();

            Console.WriteLine("{0} has guess {1} count of die value {2}.", currentPlayer.Name,
                currentPlayer.NumberOfDiceGuess, currentPlayer.ValueOfDieGuess);
        }

        /*
		* This method is used to make sure the current player is guessing larger number of dice than previous player
		* and that the value of the dice they guess is possible.
		*/
        private static void VerifyRules()
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
        private static Player CallLiar()
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
        private static bool EndGame(Player playerWhoCalledLiar)
        {
            var currentPlayer = listOfPlayers[0];

            // Only enter end game loop if player who called liar is not current player
            if (playerWhoCalledLiar.Name != currentPlayer.Name)
            {
                // Only enter loop if current player was lying
                if (CheckLiar())
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
        private static bool CheckLiar()
        {
            var currentPlayer = listOfPlayers[0];

            if (currentPlayer.ValueOfDieGuess == 1)
            {
                Console.WriteLine("Count for dice with value of 1: {0}", diceWithValueOne);
                if (currentPlayer.NumberOfDiceGuess > diceWithValueOne)
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
                Console.WriteLine("Count for dice with value of 2: {0}", diceWithValueTwo);
                if (currentPlayer.NumberOfDiceGuess > diceWithValueTwo)
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
                Console.WriteLine("Count for dice with value of 3: {0}", diceWithValueThree);
                if (currentPlayer.NumberOfDiceGuess > diceWithValueThree)
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
                Console.WriteLine("Count for dice with value of 4: {0}", diceWithValueFour);
                if (currentPlayer.NumberOfDiceGuess > diceWithValueFour)
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
                Console.WriteLine("Count for dice with value of 5: {0}", diceWithValueFive);
                if (currentPlayer.NumberOfDiceGuess > diceWithValueFive)
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
                Console.WriteLine("Count for dice with value of 6: {0}", diceWithValueSix);
                if (currentPlayer.NumberOfDiceGuess > diceWithValueSix)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /*
         *  This method allows the continued play of the liars dice game so that the program
         *  can continue indefinitely as long as players want to continue
         */

        private static void Rematch()
        {
            // Prompt for user input an repeat until valid entry is made
            Console.WriteLine("\n\nWould you like to play again? (y/n): ");
            string playAgain = "";
            bool waitingForValidInput = true;
            while (waitingForValidInput)
            {
                try
                {
                    playAgain = Console.ReadLine().ToString();
                    waitingForValidInput = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an error reading unser input. Program - Rematch Error: {0}", e);
                }
            }

            if (playAgain == "y")
            {
                Reset();
                RemovePlayer();

                // Verify enough players are available after removing players
                if (listOfPlayers.Count < REQUIRED_PLAYERS)
                {
                    Console.WriteLine("You must add at least {0} more player(s) in order to play again",
                        REQUIRED_PLAYERS - listOfPlayers.Count);
                    while (listOfPlayers.Count < REQUIRED_PLAYERS)
                    {
                        AddPlayer();
                    }
                }
                else
                {
                    AddPlayer();
                }
            }
            else
            {
                continueGameLoop = false;
            }
        }

        /*
         * Method resets all dice counts and player dice rolls
         */
        private static void Reset()
        {
            // reset dice coutns
            diceWithValueOne = 0;
            diceWithValueTwo = 0;
            diceWithValueThree = 0;
            diceWithValueFour = 0;
            diceWithValueFive = 0;
            diceWithValueSix = 0;

            // clear dice list for each player
            foreach (var player in listOfPlayers)
            {
                player.DiceList.Clear();
            }
        }

        /*
         * Allows the player base to be modified through removign players for the next round
         */
        private static void RemovePlayer()
        {
            // Prompt user for whether new player will be added
            Console.WriteLine("Would you like to remove a player? (y/n): ");
            bool waitingForValidInput = true;
            string removePlayer = "";
            while (waitingForValidInput)
            {
                try
                {
                    removePlayer = Console.ReadLine().ToString();
                    waitingForValidInput = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an error reading unser input. Program - Rematch Error: {0}", e);
                }
            }

            // If adding new players, prompt for number of new players
            if (removePlayer == "y")
            {
                bool continueRemovingPlayers = true;
                while (continueRemovingPlayers)
                {
                    Console.WriteLine("Enter name of player to be removed: ");
                    waitingForValidInput = true;
                    var playerToRemove = "";
                    while (waitingForValidInput)
                    {
                        try
                        {
                            playerToRemove = Console.ReadLine().ToString();
                            waitingForValidInput = false;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Please make sure you enter a valid number");
                        }
                    }

                    // Find player and remove
                    var removeThisPlayer = listOfPlayers.Find(x => x.Name == playerToRemove);
                    listOfPlayers.Remove(removeThisPlayer);

                    // Prompt to remove any more players
                    Console.WriteLine("Do you want to remove any other players? (y/n)");
                    waitingForValidInput = true;
                    var keepRemovingPlayers = "";
                    while (waitingForValidInput)
                    {
                        try
                        {
                            keepRemovingPlayers = Console.ReadLine().ToString();
                            waitingForValidInput = false;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Please make sure you enter a valid number");
                        }
                    }
                    if (keepRemovingPlayers != "y")
                    {
                        continueRemovingPlayers = false;
                    }
                }
            }
        }

        /*
         * Allows the player base to be modified through adding a new player for the next round
         */
        private static void AddPlayer()
        {
            // Prompt user for whether new player will be added
            Console.WriteLine("Would you like to add a new player? (y/n): ");
            bool waitingForValidInput = true;
            string addPlayer = "";
            while (waitingForValidInput)
            {
                try
                {
                    addPlayer = Console.ReadLine().ToString();
                    waitingForValidInput = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an error reading unser input. Program - Rematch Error: {0}", e);
                }
            }

            // If adding new players, prompt for number of new players
            if (addPlayer == "y")
            {
                Console.WriteLine("How many players do you want to add? ");
                waitingForValidInput = true;
                int numberOfPlayersToAdd = 0;
                var newPlayerName = "";
                while (waitingForValidInput)
                {
                    try
                    {
                        numberOfPlayersToAdd = int.Parse(Console.ReadLine().ToString());
                        waitingForValidInput = false;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Please make sure you enter a valid number");
                    }
                }

                // Add a new player and prompt name for each new player
                for (int i = 1; i <= numberOfPlayersToAdd; i++)
                {
                    Console.WriteLine("Please enter player name: ");

                    try
                    {
                        newPlayerName = Console.ReadLine().ToString();
                        Player newPlayer = new Player();
                        newPlayer.Name = newPlayerName;
                        listOfPlayers.Add(newPlayer);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("New player not created. Program - AddPlayer Error: {0}", e);
                    }
                }
            }
        }

    }
}
