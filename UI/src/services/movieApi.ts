import { Movie, Genre, MovieDetails, SearchResult, Comment } from "../types";
import { API_CONFIG, createApiUrl } from "../config/api";

async function fetchApi<T>(
  endpoint: string,
  options?: RequestInit
): Promise<T> {
  const user = localStorage.getItem("user");
  const token = user ? JSON.parse(user).token : null;

  const headers = {
    "Content-Type": "application/json",
    ...(token && { Authorization: `Bearer ${token}` }),
    ...(options?.headers || {}),
  };

  const response = await fetch(`${API_CONFIG.BASE_URL}${endpoint}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    throw new Error(`API request failed: ${response.statusText}`);
  }

  return await response.json();
}

export const movieApi = {
  getLatestMovies: (): Promise<Movie[]> => {
    return fetchApi<Movie[]>(API_CONFIG.ENDPOINTS.MOVIES.LATEST);
  },

  getTopRatedMovies: (): Promise<Movie[]> => {
    return fetchApi<Movie[]>(API_CONFIG.ENDPOINTS.MOVIES.TOP_RATED);
  },

  getMovieDetails: (id: number): Promise<MovieDetails> => {
    return fetchApi<MovieDetails>(API_CONFIG.ENDPOINTS.MOVIES.DETAILS(id));
  },

  searchMovies: (
    query: string,
    genreId?: number,
    page: number = 1
  ): Promise<SearchResult> => {
    const params: Record<string, string | number> = { page };

    if (query) {
      params.query = query;
    }

    if (genreId) {
      params.genreId = genreId;
    }

    const url = createApiUrl(API_CONFIG.ENDPOINTS.MOVIES.SEARCH, params);
    return fetchApi<SearchResult>(url.replace(API_CONFIG.BASE_URL, ""));
  },

  getGenres: (): Promise<Genre[]> => {
    return fetchApi<Genre[]>(API_CONFIG.ENDPOINTS.MOVIES.GENRES);
  },
};

export const commentApi = {
  getComments: (movieId: number): Promise<Comment[]> => {
    return fetchApi<Comment[]>(
      API_CONFIG.ENDPOINTS.COMMENTS.GET_BY_MOVIE(movieId)
    );
  },

  addComment: (comment: {
    movieId: number;
    content: string;
  }): Promise<Comment> => {
    return fetchApi<Comment>(API_CONFIG.ENDPOINTS.COMMENTS.ADD, {
      method: "POST",
      body: JSON.stringify(comment),
    });
  },

  deleteComment: (commentId: number): Promise<void> => {
    return fetchApi<void>(API_CONFIG.ENDPOINTS.COMMENTS.DELETE(commentId), {
      method: "DELETE",
    });
  },
};
