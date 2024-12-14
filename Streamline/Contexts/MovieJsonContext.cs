using System.Text.Json.Serialization;
using Streamline.Models;

namespace Streamline.Contexts;

[JsonSerializable(typeof(MovieResponse))]
[JsonSerializable(typeof(Movie))]
public partial class MovieJsonContext : JsonSerializerContext
{
}