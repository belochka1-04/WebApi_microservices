
namespace KameraData.Data.Models
{
    public class ModelNumberRequest
    {
        public int JobId { get; set; }
        public Model Model { get; set; }
        public string SearchText { get; set; }
        public string? ModelNumber { get; set; }
        public string? Brand { get; set; }
        public int Status { get; set; }
        public int Count { get; set; }
        public string? CleanedModel { get; set; }

    }

    public class NModelNumberRequest
    {
        public int JobId { get; set; }
        public NModel Model { get; set; }
        public string SearchText { get; set; }
        public string? ModelNumber { get; set; }
        public string? Brand { get; set; }
        public int Status { get; set; }
        public int Count { get; set; }


    }
}
