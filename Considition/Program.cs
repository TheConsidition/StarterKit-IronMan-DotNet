using Considition.RestApi;
using Considition.RestApi.Models.Ironman;
using System;

namespace Considition
{
    public class Program
    {
        // TODO: Enter your API key
        private const string ApiKey = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";

        // Game options
        private const int MaxPlayers = 1;
        private const string Map = "standardmap";
        private const int NumberOfStreams = 10;
        private const int NumberOfElevations = 10;
        private const int NumberOfPowerups = 10;

        static void Play(GameState gameState)
        {
            // TODO: Implement your ironman

            // Example
            var random = new Random();
            var directions = new[] { "e", "w", "n", "s" };
            for (var i = 0; i < 50; i++)
            {
                var direction = directions[random.Next(directions.Length)];
                gameState = Api.MakeMove(gameState.GameId, direction, "slow");
            }
        }

        public static void Main(string[] args)
        {
            Api.SetHeaders(ApiKey);
			Api.EndPreviousGamesIfAny(); //Can only have 2 active games at once. This will end any previous ones.
            var gameId = Api.InitGame(MaxPlayers, Map, NumberOfStreams, NumberOfElevations, NumberOfPowerups);
	        var gameState = Api.JoinGame(gameId);
            gameState = Api.TryReadyUp(gameId);
            Play(gameState);
        }
    }
}
