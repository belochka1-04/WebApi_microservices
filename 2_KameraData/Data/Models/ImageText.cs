
namespace KameraData.Data.Models;

public partial class ImageText
{
    public int Id { get; set; }
    public string folder { get; set; } = null!;
    public string file { get; set; } = null!;
    public string text { get; set; } = null!;
}

