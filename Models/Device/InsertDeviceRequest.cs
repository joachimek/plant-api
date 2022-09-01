namespace plant_api.Models.Device
{
    public class InsertDeviceRequest
    {
        public long UserID { get; set; } = -1;
        public long PlantID { get; set; } = -1;
        public string Name { get; set; } = "Unnamed device";
    }
}
