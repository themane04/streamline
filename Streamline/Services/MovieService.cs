﻿using System.Text.Json;
using Streamline.Adapters;
using Streamline.Contexts;
using Streamline.Enums;
using Streamline.Models;
using Streamline.Utilities;

namespace Streamline.Services;

public class MovieService
{
    private static readonly string ApiKey = Environments.GetApiKey();
    private static readonly string BaseUrl = Environments.GetDbUrl();
    public static List<Movie> allMovies = new();

    public static async Task<List<Movie>> GetPopularMoviesAsync(int page)
    {
        using HttpClient client = new();
        string url = $"{BaseUrl}movie/popular?api_key={ApiKey}&language=en-US&page={page}";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                LogHelper.Log(LogLevel.Info, "MovieService",
                    $"Fetched {json.Length} bytes of movie data for page {page}.");

                var result = JsonSerializer.Deserialize(json, MovieJsonContext.Default.MovieResponse);

                if (result?.Results != null)
                {
                    LogHelper.Log(LogLevel.Info, "MovieService",
                        $"Deserialized {result.Results.Count} movies on page {page}.");
                    return result.Results;
                }
            }
            else
            {
                LogHelper.Log(LogLevel.Warn, "MovieService", $"API Error on page {page}: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            LogHelper.Log(LogLevel.Error, "MovieService", $"Error fetching movies on page {page}: {ex.Message}");
        }

        return new List<Movie>();
    }

    public static async Task LoadMoviesAsync(Activity activity, int page, MovieAdapter adapter)
    {
        try
        {
            var newMovies = await GetPopularMoviesAsync(page);

            if (newMovies.Count > 0)
            {
                allMovies.AddRange(newMovies);
                LogHelper.Log(LogLevel.Info, "MovieService", $"Total movies in allMovies: {allMovies.Count}");
                activity.RunOnUiThread(() => adapter.AppendMovies(newMovies));
            }
            else
            {
                LogHelper.Log(LogLevel.Warn, "MovieService", "No more movies to load.");
            }
        }
        catch (Exception ex)
        {
            LogHelper.Log(LogLevel.Error, "MovieService", $"Error loading movies for page {page}: {ex.Message}");
        }
    }
}