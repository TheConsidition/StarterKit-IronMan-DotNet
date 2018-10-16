using System;
using System.Collections.Generic;

namespace Considition.RestApi.Models.Ironman
{
    class Player
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        public string Status { get; set; }
        public int StatusDuration { get; set; }
        public int Stamina { get; set; }
        public List<string> PowerupInventory { get; set; }
        public string Name { get; set; }
        public int PlayedTurns { get; set; }
        public List<ActivePowerup> ActivePowerups { get; set; }
    }
}
