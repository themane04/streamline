using System.Text.Json.Serialization;
using Streamline.Models;
using Streamline.Utilities;

namespace Streamline.Modules.context;

[JsonSerializable(typeof(MovieResponse))]
[JsonSerializable(typeof(List<Movie>))]
public partial class MovieJsonContext : JsonSerializerContext
{
}