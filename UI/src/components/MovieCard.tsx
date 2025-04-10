import { Link } from "react-router-dom";
import { Movie } from "../types";
import { API_CONFIG, createImageUrl } from "../config/api";

interface MovieCardProps {
  movie: Movie;
}

const MovieCard = ({ movie }: MovieCardProps) => {
  // Use the createImageUrl utility to handle both direct URLs (from backend) and partial paths
  const posterUrl = createImageUrl(
    movie.posterPath,
    API_CONFIG.IMAGES.POSTER_SIZE.MEDIUM
  );

  return (
    <Link to={`/movie/${movie.id}`} className="block">
      <div className="bg-white rounded-lg shadow-md overflow-hidden h-full hover:shadow-xl transition-shadow duration-300">
        {posterUrl ? (
          <img
            src={posterUrl}
            alt={movie.title}
            className="w-full h-80 object-cover"
            loading="lazy"
          />
        ) : (
          <div className="w-full h-80 bg-gray-300 flex items-center justify-center">
            <span className="text-gray-500">No Image Available</span>
          </div>
        )}
        <div className="p-4">
          <div className="flex justify-between items-center mb-2">
            <h3 className="font-bold text-lg line-clamp-1">{movie.title}</h3>
            <div className="flex items-center">
              <svg
                className="w-5 h-5 text-yellow-400"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path>
              </svg>
              <span className="ml-1 font-semibold">
                {movie.voteAverage.toFixed(1)}
              </span>
            </div>
          </div>
          <p className="text-gray-600 text-sm line-clamp-3">{movie.overview}</p>
          <div className="mt-3 text-xs text-gray-500">
            {new Date(movie.releaseDate).getFullYear()}
            {movie.genres && movie.genres.length > 0 && (
              <span className="ml-2">
                • {movie.genres.map((genre) => genre.name).join(", ")}
              </span>
            )}
          </div>
        </div>
      </div>
    </Link>
  );
};

export default MovieCard;
