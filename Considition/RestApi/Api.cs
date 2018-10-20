using Considition.RestApi.Models.Ironman;
using Considition.RestApi.Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Diagnostics;
using System.Threading;

namespace Considition.RestApi
{
    static class Api
    {
        private const string BasePath = "http://theconsidition.se/considition/ironman";
        private static readonly RestClient Client = new RestClient(BasePath);
        private static string _apiKey;

        public static bool Silent { get; set; }

        private static void LogError(string message)
        {
	        Console.WriteLine($"API: {message}");
	        Debug.WriteLine($"API: {message}");
        }

        private static void Log(string message)
        {
            if (!Silent)
            {
                Console.WriteLine($"API: {message}");
				Debug.WriteLine($"API: {message}");
            }
        }

        private static bool HasErrors(IRestResponse response)
        {
            if (response.ErrorException != null)
            {
                var message = $"Error: {response.ErrorMessage}";
                LogError(message);
	            return true;
            }

            dynamic result = JsonConvert.DeserializeObject(response.Content);
            if (!(bool)((JValue)result.success).Value)
            {
                string message = result.message.ToString();
                LogError($"Game error: {message}");
	            return true;
            }

	        return false;
        }

        private static GameState GetGameState(string jsonResponse)
        {
            var response = JsonConvert.DeserializeObject<GameStateApiResponse>(jsonResponse);
			Log($"Starting turn {response.GameState.Turn}");
            return response.GameState;
        }

        public static void SetHeaders(string apiKey)
        {
            _apiKey = apiKey;
            Client.AddDefaultHeader("x-api-key", apiKey);
			Client.AddDefaultHeader("content-type", "application/json; charset=utf-8");
        }

        public static string InitGame(int maxPlayers = 1, string map = "standardmap", int numberOfStreams = 10, int numberOfElevations = 10, int numberOfPowerups = 10)
        {
            var request = new RestRequest("games", Method.POST);
            request.AddJsonBody(new { maxPlayers, map, numberOfStreams, numberOfElevations, numberOfPowerups });
            var response = Client.Execute(request);
            HasErrors(response);

            dynamic result = JsonConvert.DeserializeObject(response.Content);
            Log($"Created new game: {result.gameId}");
            return result.gameId;
        }

        public static GameState GetGame(string gameId)
        {
	        var request = new RestRequest($"games/{gameId}/{_apiKey}", Method.GET);
            IRestResponse response;
	        do
	        {
		        Log($"Getting game: {gameId}");
		        response = Client.Execute(request);
	        } while (HasErrors(response));

            return GetGameState(response.Content);
        }

        public static GameState JoinGame(string gameId)
        {
            var request = new RestRequest($"games/{gameId}/join", Method.POST);
            var response = Client.Execute(request);
            HasErrors(response);
            Log($"Joined game: {gameId}");
            return GetGameState(response.Content);
        }

        public static GameState ReadyUp(string gameId)
        {
            Log("Readying up!");
            var request = new RestRequest($"games/{gameId}/ready", Method.POST);
            var response = Client.Execute(request);
            HasErrors(response);
            return GetGameState(response.Content);
        }

        public static GameState TryReadyUp(string gameId)
        {
            Log("Readying up!");
            while (true)
            {
                try
                {
	                Log("Trying to ready up!");
                    var request = new RestRequest($"games/{gameId}/ready", Method.POST);
                    var response = Client.Execute(request);
                    HasErrors(response);
                    return GetGameState(response.Content);
                }
                catch (Exception)
                {
                    Thread.Sleep(3000);
                }
            }
        }

        public static GameState MakeMove(string gameId, string direction, string speed)
        {
            var request = new RestRequest($"games/{gameId}/action/move", Method.POST);
            request.AddJsonBody(new { speed, direction });
	        Log($"Attempting to makeMove with speed: {speed} and direction: {direction}");
	        var response = Client.Execute(request);
	        return HasErrors(response) ? GetGame(gameId) : GetGameState(response.Content);
        }

        public static GameState Step(string gameId, string direction)
        {
            var request = new RestRequest($"games/{gameId}/action/step", Method.POST);
            request.AddJsonBody(new { direction });
	        Log($"Attempting to step in direction: {direction}");
            var response = Client.Execute(request);
	        return HasErrors(response) ? GetGame(gameId) : GetGameState(response.Content);
        }

        public static GameState Rest(string gameId)
        {
            var request = new RestRequest($"games/{gameId}/action/rest", Method.POST);
	        Log("Attempting to rest!");
            var response = Client.Execute(request);
	        return HasErrors(response) ? GetGame(gameId) : GetGameState(response.Content);
        }

        public static GameState UsePowerup(string gameId, string powerupName)
        {
            var request = new RestRequest($"games/{gameId}/action/usepowerup", Method.POST);
            request.AddJsonBody(new { name = powerupName });
	        Log($"Attempting to use powerup: {powerupName}");
            var response = Client.Execute(request);
	        return HasErrors(response) ? GetGame(gameId) : GetGameState(response.Content);
        }

	    public static GameState DropPowerup(string gameId, string powerupName)
	    {
		    var request = new RestRequest($"games/{gameId}/action/droppowerup", Method.POST);
		    request.AddJsonBody(new { name = powerupName });
		    Log($"Attempting to drop powerup: {powerupName}");
		    var response = Client.Execute(request);
		    return HasErrors(response) ? GetGame(gameId) : GetGameState(response.Content);
	    }

	    public static void EndPreviousGamesIfAny()
	    {
		    var request = new RestRequest("games", Method.DELETE);
		    Log("Attempting to remove previous games if any.");
		    var response = Client.Execute(request);
	    }
    }
}
