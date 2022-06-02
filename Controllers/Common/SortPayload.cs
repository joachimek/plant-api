namespace plant_api.Controllers.Common
{
    public class SortPayload
    {
        public string Field { get; set; } = "Id";
        public SortOrder Order { get; set; } = SortOrder.DESC;
    }
}
