using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public decimal Price { get; set; }
    public string Type { get; set; }
    public bool Available { get; set; }
    public string AddedTime { get; set; }
    public int SellerId { get; set; }
}
