using System.Text.Json.Serialization;
using Streamline.Models;

namespace Streamline.Contexts;

[JsonSerializable(typeof(MovieResponseTmdb))]
[JsonSerializable(typeof(Movie))]
public partial class MovieJsonContext : JsonSerializerContext
{
}