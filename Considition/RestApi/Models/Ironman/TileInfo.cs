namespace Considition.RestApi.Models.Ironman
{
    class TileInfo
    {
        public string Type { get; set; }
        public string Weather { get; set; }
        public Powerup Powerup { get; set; }
        public WaterStream WaterStream { get; set; }
        public Elevation Elevation { get; set; }
    }
}
