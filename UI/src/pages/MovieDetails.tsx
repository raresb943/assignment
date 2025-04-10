import { useState, useEffect, useContext } from "react";
import { useParams } from "react-router-dom";
import { movieApi, commentApi } from "../services/movieApi";
import { MovieDetails as MovieDetailsType, Comment } from "../types";
import { AuthContext } from "../context/AuthContext";
import { API_CONFIG, createImageUrl } from "../config/api";

const MovieDetails = () => {
  const { id } = useParams<{ id: string }>();
  const [movieDetails, setMovieDetails] = useState<MovieDetailsType | null>(
    null
  );
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [activeImageIndex, setActiveImageIndex] = useState<number>(0);
  const [commentText, setCommentText] = useState<string>("");
  const { isAuthenticated, user } = useContext(AuthContext);

  useEffect(() => {
    const fetchMovieDetails = async () => {
      if (!id) return;

      try {
        setLoading(true);
        const data = await movieApi.getMovieDetails(parseInt(id));
        setMovieDetails(data);
        setError(null);
      } catch (err) {
        console.error("Failed to fetch movie details:", err);
        setError("Failed to load movie details. Please try again later.");
      } finally {
        setLoading(false);
      }
    };

    fetchMovieDetails();
  }, [id]);

  const handleAddComment = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!isAuthenticated || !movieDetails || !commentText.trim()) {
      return;
    }

    if (!user || !user.id) {
      alert("You must be logged in to add comments");
      return;
    }

    try {
      const newComment: Omit<
        Comment,
        "id" | "createdAt" | "userId" | "userName"
      > = {
        movieId: movieDetails.movie.id,
        content: commentText.trim(),
      };

      const addedComment = await commentApi.addComment(newComment as any);

      setMovieDetails((prevDetails) => {
        if (!prevDetails) return null;

        return {
          ...prevDetails,
          comments: [addedComment, ...prevDetails.comments],
        };
      });

      setCommentText("");
    } catch (err: any) {
      console.error("Failed to add comment:", err);

      if (err.message && err.message.includes("401")) {
        alert("Your session has expired. Please login again.");
      } else {
        alert("Failed to add comment. Please try again.");
      }
    }
  };

  const handleDeleteComment = async (commentId: number) => {
    if (!movieDetails) return;

    try {
      await commentApi.deleteComment(commentId);

      setMovieDetails((prevDetails) => {
        if (!prevDetails) return null;

        return {
          ...prevDetails,
          comments: prevDetails.comments.filter((c) => c.id !== commentId),
        };
      });
    } catch (err) {
      console.error("Failed to delete comment:", err);
      alert("Failed to delete comment. Please try again.");
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-red-500"></div>
      </div>
    );
  }

  if (error || !movieDetails) {
    return (
      <div className="text-center text-red-500 py-8">
        {error || "Movie not found"}
      </div>
    );
  }

  const { movie, cast, images, comments } = movieDetails;

  const backdropUrl = createImageUrl(
    movie.backdropPath,
    API_CONFIG.IMAGES.BACKDROP_SIZE.LARGE
  );
  const posterUrl = createImageUrl(
    movie.posterPath,
    API_CONFIG.IMAGES.POSTER_SIZE.MEDIUM
  );

  const allImages = [
    backdropUrl,
    posterUrl,
    ...images.map((img) =>
      createImageUrl(
        img.filePath,
        img.type === "backdrop"
          ? API_CONFIG.IMAGES.BACKDROP_SIZE.MEDIUM
          : API_CONFIG.IMAGES.POSTER_SIZE.MEDIUM
      )
    ),
  ].filter(Boolean) as string[];

  return (
    <div className="pb-12">
      <div
        className="relative h-96 bg-cover bg-center mb-8"
        style={{
          backgroundImage: `linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.6)), url(${backdropUrl})`,
        }}
      >
        <div className="absolute inset-0 flex items-center">
          <div className="container mx-auto px-4 text-white">
            <h1 className="text-4xl font-bold mb-2">{movie.title}</h1>
            <div className="flex items-center mb-4">
              <span className="flex items-center mr-4">
                <svg
                  className="w-5 h-5 text-yellow-400"
                  fill="currentColor"
                  viewBox="0 0 20 20"
                >
                  <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path>
                </svg>
                <span className="ml-1">
                  {movie.voteAverage.toFixed(1)} ({movie.voteCount} votes)
                </span>
              </span>
              <span className="mr-4">
                {new Date(movie.releaseDate).getFullYear()}
              </span>
              {movie.genres && (
                <span>{movie.genres.map((g) => g.name).join(", ")}</span>
              )}
            </div>
            <p className="max-w-2xl">{movie.overview}</p>
          </div>
        </div>
      </div>

      <div className="container mx-auto px-4">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2">
            {allImages.length > 0 && (
              <div className="mb-12">
                <h2 className="text-2xl font-bold mb-4">Gallery</h2>
                <div className="bg-white rounded-lg shadow-md p-4">
                  <div className="mb-4 h-96 overflow-hidden rounded-lg">
                    <img
                      src={allImages[activeImageIndex]}
                      alt={`${movie.title} - Image ${activeImageIndex + 1}`}
                      className="w-full h-full object-contain"
                    />
                  </div>

                  <div className="grid grid-cols-4 sm:grid-cols-5 gap-2">
                    {allImages.map((image, index) => (
                      <button
                        key={index}
                        className={`rounded overflow-hidden h-20 border-2 ${
                          index === activeImageIndex
                            ? "border-red-500"
                            : "border-transparent"
                        }`}
                        onClick={() => setActiveImageIndex(index)}
                      >
                        <img
                          src={image}
                          alt={`${movie.title} - Thumbnail ${index + 1}`}
                          className="w-full h-full object-cover"
                        />
                      </button>
                    ))}
                  </div>
                </div>
              </div>
            )}

            {cast.length > 0 && (
              <div className="mb-12">
                <h2 className="text-2xl font-bold mb-4">Cast</h2>
                <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
                  {cast.map((person) => {
                    const profileUrl = createImageUrl(
                      person.profilePath,
                      API_CONFIG.IMAGES.PROFILE_SIZE
                    );

                    return (
                      <div
                        key={person.id}
                        className="bg-white rounded-lg shadow overflow-hidden"
                      >
                        {profileUrl ? (
                          <img
                            src={profileUrl}
                            alt={person.name}
                            className="w-full h-40 object-cover"
                          />
                        ) : (
                          <div className="w-full h-40 bg-gray-200 flex items-center justify-center">
                            <span className="text-gray-500 text-sm">
                              No Image
                            </span>
                          </div>
                        )}
                        <div className="p-3">
                          <h3 className="font-semibold text-sm">
                            {person.name}
                          </h3>
                          <p className="text-gray-600 text-xs">
                            {person.character}
                          </p>
                        </div>
                      </div>
                    );
                  })}
                </div>
              </div>
            )}

            <div>
              <h2 className="text-2xl font-bold mb-4">Comments</h2>

              {isAuthenticated ? (
                <form onSubmit={handleAddComment} className="mb-8">
                  <div className="mb-4">
                    <textarea
                      value={commentText}
                      onChange={(e) => setCommentText(e.target.value)}
                      className="w-full p-3 border rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-red-500"
                      rows={4}
                      placeholder="Share your thoughts about this movie..."
                      required
                    ></textarea>
                  </div>
                  <button
                    type="submit"
                    className="bg-red-600 text-white px-6 py-2 rounded-lg hover:bg-red-700 transition"
                    disabled={!commentText.trim()}
                  >
                    Post Comment
                  </button>
                </form>
              ) : (
                <div className="bg-gray-100 p-4 rounded-lg mb-8 text-center">
                  <p>Please login to leave a comment</p>
                </div>
              )}

              {comments.length > 0 ? (
                <div className="space-y-4">
                  {comments.map((comment) => (
                    <div
                      key={comment.id}
                      className="bg-white p-4 rounded-lg shadow"
                    >
                      <div className="flex justify-between items-start mb-2">
                        <div>
                          <h4 className="font-semibold">{comment.userName}</h4>
                          <p className="text-gray-500 text-xs">
                            {new Date(comment.createdAt).toLocaleDateString()}{" "}
                            at{" "}
                            {new Date(comment.createdAt).toLocaleTimeString()}
                          </p>
                        </div>
                        {isAuthenticated && user?.id === comment.userId && (
                          <button
                            onClick={() => handleDeleteComment(comment.id)}
                            className="text-red-500 hover:text-red-700"
                          >
                            Delete
                          </button>
                        )}
                      </div>
                      <p className="text-gray-700">{comment.content}</p>
                    </div>
                  ))}
                </div>
              ) : (
                <p className="text-gray-500 italic">No comments yet</p>
              )}
            </div>
          </div>

          <div className="lg:col-span-1">
            <div className="sticky top-4">
              <div className="bg-white rounded-lg shadow-md overflow-hidden mb-6">
                <img
                  src={posterUrl || ""}
                  alt={movie.title}
                  className="w-full h-auto"
                />
              </div>
              <div className="bg-white rounded-lg shadow-md p-4">
                <h3 className="font-bold text-lg mb-2">Movie Information</h3>
                <div className="space-y-2">
                  <div>
                    <span className="font-semibold text-gray-700">
                      Release Date:
                    </span>{" "}
                    <span>
                      {new Date(movie.releaseDate).toLocaleDateString()}
                    </span>
                  </div>
                  <div>
                    <span className="font-semibold text-gray-700">Rating:</span>{" "}
                    <span>
                      {movie.voteAverage.toFixed(1)}/10 ({movie.voteCount}{" "}
                      votes)
                    </span>
                  </div>
                  {movie.genres && movie.genres.length > 0 && (
                    <div>
                      <span className="font-semibold text-gray-700">
                        Genres:
                      </span>{" "}
                      <span>{movie.genres.map((g) => g.name).join(", ")}</span>
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default MovieDetails;
