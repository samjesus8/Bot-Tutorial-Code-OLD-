using System;

namespace YouTubeTestBot.External_Classes
{
    internal class CardBuilder
    {
        public int[] cardNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        public string[] cardSuits = { "Clubs", "Spades", "Diamonds", "Hearts" };

        public int SelectedNumber { get; internal set; }
        public string SelectedCard { get; internal set; }

        public CardBuilder() 
        {
            var Random = new Random();
            int indexNumbers = Random.Next(0, this.cardNumbers.Length - 1);
            int indexSuit = Random.Next(0, this.cardSuits.Length - 1);

            this.SelectedNumber = this.cardNumbers[indexNumbers];
            this.SelectedCard = this.cardNumbers[indexNumbers] + " of " + this.cardSuits[indexSuit];
        }
    }
}
