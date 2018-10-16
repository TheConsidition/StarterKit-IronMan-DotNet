using System.Collections.Generic;

namespace Considition.RestApi.Models.Ironman
{
    class GameState
    {
        public string GameId { get; set; }
        public string GameStatus { get; set; }
        public int Turn { get; set; }
        public Player YourPlayer { get; set; }
        public List<Player> OtherPlayers { get; set; }
        public TileInfo[][] TileInfo { get; set; }
    }
}
