// API configuration constants
export const API_CONFIG = {
  BASE_URL: "https://localhost:7071/api",
  ENDPOINTS: {
    MOVIES: {
      LATEST: "/movies/latest",
      TOP_RATED: "/movies/top-rated",
      DETAILS: (id: number) => `/movies/${id}`,
      SEARCH: "/movies/search",
      GENRES: "/movies/genres",
    },
    COMMENTS: {
      GET_BY_MOVIE: (id: number) => `/comments/movie/${id}`,
      ADD: "/comments",
      DELETE: (id: number) => `/comments/${id}`,
    },
    AUTH: {
      LOGIN: "/auth/login",
      REGISTER: "/auth/register",
    },
  },
  IMAGES: {
    BASE_URL: "https://image.tmdb.org/t/p/original",
    POSTER_SIZE: {
      SMALL: "w185",
      MEDIUM: "w342",
      LARGE: "w500",
    },
    BACKDROP_SIZE: {
      SMALL: "w300",
      MEDIUM: "w780",
      LARGE: "w1280",
    },
    PROFILE_SIZE: "w185",
  },
};

export const createApiUrl = (
  endpoint: string,
  queryParams?: Record<string, string | number | boolean>
): string => {
  let url = `${API_CONFIG.BASE_URL}${endpoint}`;

  if (queryParams) {
    const queryString = Object.entries(queryParams)
      .filter(
        ([_, value]) => value !== undefined && value !== null && value !== ""
      )
      .map(([key, value]) => `${key}=${encodeURIComponent(String(value))}`)
      .join("&");

    if (queryString) {
      url += `?${queryString}`;
    }
  }

  return url;
};

// Helper function to create image URLs
export const createImageUrl = (
  path: string | null,
  size: string
): string | null => {
  if (!path) return null;

  // Check if the path already includes the base URL
  if (path.startsWith("http")) {
    return path;
  }

  return `${API_CONFIG.IMAGES.BASE_URL}${size}${path}`;
};
