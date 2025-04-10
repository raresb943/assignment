import { useState, useEffect } from "react";
import { Movie } from "../types";
import { movieApi } from "../services/movieApi";
import MovieCard from "../components/MovieCard";

const Home = () => {
  const [latestMovies, setLatestMovies] = useState<Movie[]>([]);
  const [topRatedMovies, setTopRatedMovies] = useState<Movie[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<"latest" | "top">("latest");

  useEffect(() => {
    const fetchMovies = async () => {
      try {
        setLoading(true);
        const [latestData, topRatedData] = await Promise.all([
          movieApi.getLatestMovies(),
          movieApi.getTopRatedMovies(),
        ]);
        setLatestMovies(latestData);
        setTopRatedMovies(topRatedData);
        setError(null);
      } catch (err) {
        console.error("Failed to fetch movies:", err);
        setError("Failed to load movies. Please try again later.");
      } finally {
        setLoading(false);
      }
    };

    fetchMovies();
  }, []);

  return (
    <div>
      <div className="mb-6">
        <h1 className="text-3xl font-bold mb-2">Themoviedb wrapper</h1>
      </div>

      <div className="border-b border-gray-200 mb-6">
        <nav className="flex space-x-8">
          <button
            onClick={() => setActiveTab("latest")}
            className={`py-4 px-1 border-b-2 font-medium text-sm ${
              activeTab === "latest"
                ? "border-red-500 text-red-600"
                : "border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300"
            }`}
          >
            Latest Movies
          </button>
          <button
            onClick={() => setActiveTab("top")}
            className={`py-4 px-1 border-b-2 font-medium text-sm ${
              activeTab === "top"
                ? "border-red-500 text-red-600"
                : "border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300"
            }`}
          >
            Top Rated Movies
          </button>
        </nav>
      </div>

      {loading ? (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-red-500"></div>
        </div>
      ) : error ? (
        <div className="text-center text-red-500 py-8">{error}</div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
          {(activeTab === "latest" ? latestMovies : topRatedMovies).map(
            (movie) => (
              <MovieCard key={movie.id} movie={movie} />
            )
          )}
        </div>
      )}
    </div>
  );
};

export default Home;
