using System;

namespace Meyer
{
    internal class Program
    {
        static int turn = 1;

        static void Main(string[] args)
        {
            // Get player move for first turn
            PlayerMove? move = NextTurn(null); // null = "empty", meaning there is no previous player move

            while (move != null)
            {
                // Give (previous players') move as input, and receive current players move as output. 
                // Keep playing until player makes no move (NextTurn returns null meaning game is over)
                Console.WriteLine();
                move = NextTurn(move); // In: previous, out: current
            }
        }

        public static PlayerMove? NextTurn(PlayerMove? previousPlayerMove)
        {
            Console.WriteLine("--- Tur " + (turn++) + " ---");

            // If this is not the first turn (no previous player move) check if we call the bluff
            if (previousPlayerMove != null) // Only null on first turn 
            {
                if (!AskIfPlayerTrustsPrevious(previousPlayerMove.DiceSet))
                {
                    // We don't trust, and call the previous players bluff. This ends the game with either a win or loss for current player.
                    Console.WriteLine("Du tror ikke på forrige spillers slag");
                    if (previousPlayerMove.Truth)
                    {
                        Console.WriteLine("... men det var rigtigt. Du taber :-(");
                        return null; // No move, game ends
                    }
                    else
                    {
                        Console.WriteLine("... og det var bluff. Du har vundet :-)");
                        return null; // No move, game ends
                    }
                }
            }

            // Roll our dice
            DiceSet rolledDice = new DiceSet();
            Console.WriteLine("Du slog " + rolledDice);

            // Are you forced to bluff?
            if (previousPlayerMove != null // There is a previous move
                && previousPlayerMove.DiceSet.IsBetterThan(rolledDice)) // And it's better than ours
            {
                // Our roll was worse than what we have to beat so we have to bluff
                Console.WriteLine("Dit slag (" + rolledDice + ") er værre end forrige spillers slag (" + previousPlayerMove.DiceSet + "). Du bliver nødt til at bluffe.");
                DiceSet bluffedDice = AskForBluff(previousPlayerMove.DiceSet);
                Console.WriteLine("Du bluffer og siger " + bluffedDice);

                // Player makes a forced bluff
                return new PlayerMove(bluffedDice, false);
            }

            // Either keep the current dice, or bluff. Either way, return our move.
            return TruthOrBluff(rolledDice);
        }



        static PlayerMove TruthOrBluff(DiceSet rolledDice)
        {
            if (rolledDice.IsMeyer)
            {
                // No option to bluff if you rolled a true Meyer
                Console.WriteLine("Du kan ikke bluffe når du slår Meyer, så du fortæller sandheden og siger " + rolledDice);
                return new PlayerMove(rolledDice, true);
            }

            // If you actually rolled Meyer, don't ask if you want to bluff, just return truth
            if (AskIfPlayerBluffs())
            {
                // Player wants to bluff. Ask for a bluff that is an improvement.
                DiceSet bluffedDice = AskForBluff(rolledDice);

                if (bluffedDice.Value == rolledDice.Value)
                {
                    // Player tells the truth anyway...?
                    Console.WriteLine("Du fortæller sandheden alligevel og siger " + rolledDice);
                    return new PlayerMove(rolledDice, true);
                }
                else
                {
                    Console.WriteLine("Du bluffer og siger " + bluffedDice);
                    return new PlayerMove(bluffedDice, false);
                }
            }
            else
            {
                // Player decides to tell the truth
                Console.WriteLine("Du fortæller sandheden og siger " + rolledDice);
                return new PlayerMove(rolledDice, true);

            }
        }


        static bool AskIfPlayerBluffs()
        {
            while (true)
            {
                Console.Write("Vil du bluffe og sige du slog noget bedre? [j/n]: ");
                char result = Console.ReadKey().KeyChar;

                if (result == 'j' || result == 'n')
                {
                    Console.WriteLine();
                    return result == 'j'; // Exit with true/false response
                }
                else
                {
                    // Keep trying until you enter j or n
                    Console.WriteLine();
                    Console.WriteLine("Ugyldigt input, skriv enten 'j' eller 'n'");
                }
            }
        }


        static bool AskIfPlayerTrustsPrevious(DiceSet previousDiceSet)
        {
            while (true)
            {
                Console.Write("Tror du på forrige spillers slag (" + previousDiceSet + ")? [j/n]: ");
                char result = Console.ReadKey().KeyChar;

                if (result == 'j' || result == 'n')
                {
                    Console.WriteLine();
                    return result == 'j'; // Exit with true/false response
                }
                else
                {
                    // Keep trying until you enter j or n
                    Console.WriteLine();
                    Console.WriteLine("Ugyldigt input, skriv enten 'j' eller 'n'");
                }
            }
        }


        static DiceSet AskForBluff(DiceSet hasToBeatDice)
        {
            while (true)
            {
                Console.Write("Skriv dit bluffede terningeslag som ét tal: ");
                string? inputString = Console.ReadLine();

                try
                {
                    // Try to convert what player wrote to a DiceSet. Two conversions happening here:
                    int inputNumber = Int32.Parse(inputString);
                    DiceSet bluffedDice = new DiceSet(inputNumber);

                    // Bluffed dice has to match or beat the input, or we ask again
                    if (hasToBeatDice.IsBetterThan(bluffedDice))
                    {
                        Console.WriteLine("Dit bluff skal være mindst lige så godt som " + hasToBeatDice); 
                    }
                    else
                    {
                        return bluffedDice;
                    }
                }
                catch (Exception)
                {
                    // Whatever the player wrote was not something we cound convert to a DiceSet. Just write error message, and try again.
                    Console.WriteLine("Ugyldigt input, skriv et tal som f.eks. '12' for Meyer eller '55' for Par 5");
                }
            }
        }
    }


    public class Dice
    {
        private static readonly Random rnd = new();
        private int _value;

        public int Value
        {
            get => _value;
            private set
            {
                if (value < 1 || value > 6)
                    throw new Exception("Terning skal være 1-6");

                this._value = value;
            }
        }

        public Dice()
        {
            Value = rnd.Next(1, 7);
        }

        public Dice(int value)
        {
            Value = value;
        }
    }

    public class DiceSet
    {
        Dice Dice1 { get; }
        Dice Dice2 { get; }

        public Dice BigDice => Dice1.Value >= Dice2.Value ? Dice1 : Dice2;

        public Dice SmallDice => Dice1.Value <= Dice2.Value ? Dice1 : Dice2;

        public int Value => BigDice.Value * 10 + SmallDice.Value;

        public bool IsMeyer => SmallDice.Value == 1 && BigDice.Value == 2;

        public bool IsLittleMeyer => SmallDice.Value == 1 && BigDice.Value == 3;

        public bool IsPair => Dice1.Value == Dice2.Value;

        public DiceSet()
        {
            // Construct set with random dice rolls
            Dice1 = new Dice();
            Dice2 = new Dice();
        }

        public DiceSet(int dice1, int dice2)
        {
            // Construct set with specific dice values
            Dice1 = new Dice(dice1);
            Dice2 = new Dice(dice2);
        }

        public DiceSet(int value)
        {
            // Construct from input dice value
            Dice1 = new Dice(value / 10); // "10s" dice
            Dice2 = new Dice(value % 10); // "1s" dice"
        }

        public bool IsBetterThan(DiceSet other)
        {
            // Nothing beats Meyer
            if (other.IsMeyer)
            {
                return false;
            }

            // Other LittleMeyer is only beaten by Meyer
            if (other.IsLittleMeyer)
            {
                return this.IsMeyer;
            }

            // Other pair is beaten by Meyer, Little Meyer and a better pair
            if (other.IsPair)
            {
                return this.IsMeyer || this.IsLittleMeyer || (this.IsPair && this.Value > other.Value);
            }

            // Other is just a number. If we have a good roll, we beat it
            if (IsMeyer || IsLittleMeyer || IsPair)
            {
                return true;
            }

            // Our hand and other hand is just a number, check if theirs is better
            return this.Value > other.Value;
        }

        public override String ToString()
        {
            if (IsMeyer)
            {
                return "Meyer";
            }

            if (IsLittleMeyer)
            {
                return "Lille Meyer";
            }

            if (IsPair)
            {
                return "Par " + Dice1.Value;
            }

            return "" + Value;
        }
    }

    public class PlayerMove
    {
        public DiceSet DiceSet { get; set; }
        public bool Truth { get; set; }

        public PlayerMove(DiceSet diceSet, bool truth)
        {
            DiceSet = diceSet;
            Truth = truth;
        }
    }
}