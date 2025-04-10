export interface Movie {
  id: number;
  title: string;
  overview: string;
  posterPath: string;
  backdropPath: string;
  voteAverage: number;
  voteCount: number;
  releaseDate: string;
  genres?: Genre[];
}

export interface Genre {
  id: number;
  name: string;
}

export interface Cast {
  id: number;
  name: string;
  character: string;
  profilePath: string;
}

export interface Image {
  filePath: string;
  width: number;
  height: number;
  type: string;
}

export interface Comment {
  id: number;
  movieId: number;
  userId: string;
  userName: string;
  content: string;
  createdAt: string;
}

export interface MovieDetails {
  movie: Movie;
  cast: Cast[];
  images: Image[];
  comments: Comment[];
}

export interface SearchResult {
  results: Movie[];
  page: number;
  totalPages: number;
  totalResults: number;
}
