import { ReactNode, useContext, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";

interface LayoutProps {
  children: ReactNode;
}

const Layout = ({ children }: LayoutProps) => {
  const { isAuthenticated, logout, user } = useContext(AuthContext);
  const navigate = useNavigate();
  const [searchQuery, setSearchQuery] = useState("");

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (searchQuery.trim()) {
      navigate(`/search?query=${encodeURIComponent(searchQuery.trim())}`);
    }
  };

  return (
    <div className="flex flex-col min-h-screen bg-gray-100">
      <header className="bg-gray-900 text-white shadow-md">
        <div className="container mx-auto px-4 py-3 flex flex-col sm:flex-row items-center justify-between">
          <div className="flex items-center mb-2 sm:mb-0">
            <Link to="/" className="text-xl font-bold">
              themoviedb wrapper
            </Link>
          </div>

          <div className="w-full sm:w-1/3 mb-2 sm:mb-0">
            <form onSubmit={handleSearch} className="flex">
              <input
                type="text"
                placeholder="Search movies..."
                className="w-full px-3 py-1 text-white rounded-l focus:outline-none"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />
              <button
                type="submit"
                className="bg-red-600 px-4 py-1 rounded-r hover:bg-red-700 transition"
              >
                Search
              </button>
            </form>
          </div>

          <nav className="flex items-center space-x-4">
            <Link to="/" className="hover:text-red-400 transition">
              Home
            </Link>
            {isAuthenticated ? (
              <>
                <span className="text-gray-400">Hi, {user?.username}</span>
                <button
                  onClick={() => logout()}
                  className="hover:text-red-400 transition"
                >
                  Logout
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="hover:text-red-400 transition">
                  Login
                </Link>
                <Link to="/register" className="hover:text-red-400 transition">
                  Register
                </Link>
              </>
            )}
          </nav>
        </div>
      </header>

      <main className="flex-grow container mx-auto px-4 py-6">{children}</main>

      <footer className="bg-gray-900 text-white py-4">
        <div className="container mx-auto px-4 text-center">
          <p>Themoviedb wrapper - Rares Butean assignment</p>
        </div>
      </footer>
    </div>
  );
};

export default Layout;
