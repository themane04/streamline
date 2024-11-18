using Streamline.Resources.model;
using Streamline.Resources.util;

namespace Streamline.Resources.context;
using System.Collections.Generic;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(MovieResponse))]
[JsonSerializable(typeof(List<Movie>))]
public partial class MovieJsonContext : JsonSerializerContext
{
}
