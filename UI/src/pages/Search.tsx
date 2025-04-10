import { useState, useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import { movieApi } from "../services/movieApi";
import { Movie, Genre, SearchResult } from "../types";
import MovieCard from "../components/MovieCard";

const Search = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const initialQuery = searchParams.get("query") || "";
  const initialGenreId = searchParams.get("genreId")
    ? parseInt(searchParams.get("genreId")!)
    : undefined;

  const [query, setQuery] = useState<string>(initialQuery);
  const [selectedGenreId, setSelectedGenreId] = useState<number | undefined>(
    initialGenreId
  );
  const [genres, setGenres] = useState<Genre[]>([]);
  const [searchResults, setSearchResults] = useState<SearchResult | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState<number>(1);

  useEffect(() => {
    // Load genres when component mounts
    const fetchGenres = async () => {
      try {
        const data = await movieApi.getGenres();
        setGenres(data);
      } catch (err) {
        console.error("Failed to fetch genres:", err);
        setError("Failed to load genres");
      }
    };

    fetchGenres();
  }, []);

  useEffect(() => {
    // Perform search when URL parameters change
    const performSearch = async () => {
      if (!initialQuery && !initialGenreId) {
        return; // Don't search if no parameters
      }

      try {
        setLoading(true);
        const results = await movieApi.searchMovies(
          initialQuery,
          initialGenreId
        );
        setSearchResults(results);
        setCurrentPage(results.page);
        setError(null);
      } catch (err) {
        console.error("Search failed:", err);
        setError("Failed to search movies");
      } finally {
        setLoading(false);
      }
    };

    performSearch();
  }, [initialQuery, initialGenreId]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();

    const newParams = new URLSearchParams();
    if (query.trim()) {
      newParams.set("query", query.trim());
    }
    if (selectedGenreId) {
      newParams.set("genreId", selectedGenreId.toString());
    }

    setSearchParams(newParams);
  };

  const handleLoadMore = async () => {
    if (!searchResults || loading) return;

    const nextPage = currentPage + 1;

    if (nextPage > searchResults.totalPages) return;

    try {
      setLoading(true);
      const newResults = await movieApi.searchMovies(
        initialQuery,
        initialGenreId,
        nextPage
      );

      setSearchResults((prevResults) => {
        if (!prevResults) return newResults;

        return {
          ...newResults,
          results: [...prevResults.results, ...newResults.results],
        };
      });

      setCurrentPage(nextPage);
    } catch (err) {
      console.error("Failed to load more results:", err);
      setError("Failed to load more results");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h1 className="text-3xl font-bold mb-6">Search Movies</h1>

      <div className="bg-white p-6 rounded-lg shadow-md mb-8">
        <form onSubmit={handleSearch} className="space-y-4">
          <div>
            <label
              htmlFor="query"
              className="block text-gray-700 font-medium mb-1"
            >
              Movie Title
            </label>
            <input
              type="text"
              id="query"
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-500 bg-white text-gray-900"
              placeholder="Enter movie title..."
              value={query}
              onChange={(e) => setQuery(e.target.value)}
            />
          </div>

          <div>
            <label
              htmlFor="genre"
              className="block text-gray-700 font-medium mb-1"
            >
              Genre
            </label>
            <select
              id="genre"
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-500 bg-white text-gray-900"
              value={selectedGenreId || ""}
              onChange={(e) =>
                setSelectedGenreId(
                  e.target.value ? parseInt(e.target.value) : undefined
                )
              }
            >
              <option value="">All Genres</option>
              {genres.map((genre) => (
                <option key={genre.id} value={genre.id}>
                  {genre.name}
                </option>
              ))}
            </select>
          </div>

          <div>
            <button
              type="submit"
              className="bg-red-600 text-white px-6 py-2 rounded-lg hover:bg-red-700 transition"
            >
              Search
            </button>
          </div>
        </form>
      </div>

      {loading && !searchResults && (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-red-500"></div>
        </div>
      )}

      {error && (
        <div className="bg-red-100 text-red-700 p-4 rounded-lg mb-6">
          {error}
        </div>
      )}

      {searchResults && (
        <div>
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-xl font-semibold">
              Search Results{" "}
              <span className="text-gray-500">
                ({searchResults.totalResults} movies found)
              </span>
            </h2>
          </div>

          {searchResults.results.length > 0 ? (
            <>
              <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6 mb-8">
                {searchResults.results.map((movie) => (
                  <MovieCard key={movie.id} movie={movie} />
                ))}
              </div>

              {currentPage < searchResults.totalPages && (
                <div className="flex justify-center">
                  <button
                    onClick={handleLoadMore}
                    className="bg-gray-200 px-6 py-2 rounded-lg hover:bg-gray-300 transition"
                    disabled={loading}
                  >
                    {loading ? "Loading..." : "Load More"}
                  </button>
                </div>
              )}
            </>
          ) : (
            <div className="text-center py-12 text-gray-500">
              No movies found matching your search criteria.
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default Search;
