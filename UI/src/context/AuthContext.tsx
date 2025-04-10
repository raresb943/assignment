import { createContext, useState, useEffect, ReactNode } from "react";
import { API_CONFIG } from "../config/api";

interface User {
  id: string;
  username: string;
  token: string;
}

interface LoginFormData {
  username: string;
  password: string;
}

interface RegisterFormData {
  username: string;
  email: string;
  password: string;
}

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  login: (data: LoginFormData) => Promise<boolean>;
  register: (
    data: RegisterFormData
  ) => Promise<{ success: boolean; message?: string }>;
  logout: () => void;
  loading: boolean;
  error: string | null;
}

export const AuthContext = createContext<AuthContextType>({
  user: null,
  isAuthenticated: false,
  login: async () => false,
  register: async () => ({ success: false }),
  logout: () => {},
  loading: false,
  error: null,
});

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider = ({ children }: AuthProviderProps) => {
  const [user, setUser] = useState<User | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    // Check for saved user in localStorage on component mount
    const savedUser = localStorage.getItem("user");
    if (savedUser) {
      try {
        const parsedUser = JSON.parse(savedUser);
        setUser(parsedUser);
        setIsAuthenticated(true);
      } catch (err) {
        // If there's an error parsing the stored user, clear it
        localStorage.removeItem("user");
      }
    }
  }, []);

  const login = async (data: LoginFormData): Promise<boolean> => {
    try {
      setLoading(true);
      setError(null);

      const response = await fetch(
        `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.AUTH.LOGIN}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(data),
        }
      );

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || "Login failed");
      }

      const userData = await response.json();

      // Store user data with token
      setUser(userData);
      setIsAuthenticated(true);
      localStorage.setItem("user", JSON.stringify(userData));

      setLoading(false);
      return true;
    } catch (err) {
      setLoading(false);
      setError(err instanceof Error ? err.message : "Login failed");
      return false;
    }
  };

  const register = async (
    data: RegisterFormData
  ): Promise<{ success: boolean; message?: string }> => {
    try {
      setLoading(true);
      setError(null);

      const response = await fetch(
        `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.AUTH.REGISTER}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(data),
        }
      );

      const responseData = await response.json();

      if (!response.ok) {
        throw new Error(responseData.message || "Registration failed");
      }

      setLoading(false);
      return {
        success: true,
        message: responseData.message,
      };
    } catch (err) {
      setLoading(false);
      setError(err instanceof Error ? err.message : "Registration failed");
      return {
        success: false,
        message: err instanceof Error ? err.message : "Registration failed",
      };
    }
  };

  const logout = () => {
    setUser(null);
    setIsAuthenticated(false);
    localStorage.removeItem("user");
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated,
        login,
        register,
        logout,
        loading,
        error,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
