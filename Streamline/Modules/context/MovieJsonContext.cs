using Streamline.Modules.model;
using System.Text.Json.Serialization;
using Streamline.Modules.util;

namespace Streamline.Modules.context;

[JsonSerializable(typeof(MovieResponse))]
[JsonSerializable(typeof(List<Movie>))]
public partial class MovieJsonContext : JsonSerializerContext
{
}