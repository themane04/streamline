﻿using System.Text.Json.Serialization;
using Streamline.Models;
using Streamline.Utilities;

namespace Streamline.Contexts;

[JsonSerializable(typeof(MovieResponse))]
[JsonSerializable(typeof(List<Movie>))]
public partial class MovieJsonContext : JsonSerializerContext
{
}