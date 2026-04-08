'use client';

import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { User, LoginRequest, RegisterRequest, AuthToken, UserRole } from '@/types';
import { apiClient } from '@/lib/api-client';
import { toast } from 'sonner';

interface AuthContextType {
  user: User | null;
  isLoading: boolean;
  isAuthenticated: boolean;
  login: (credentials: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => void;
  updateUser: (user: User) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Check if user is already logged in on mount
  useEffect(() => {
    const checkAuth = async () => {
      try {
        const token = localStorage.getItem('authToken');
        if (token) {
          // Try to fetch current user
          const response = await apiClient.get<User>('/auth/me');
          if (response.success && response.data) {
            setUser(response.data);
          } else {
            // Token is invalid, clear it
            localStorage.removeItem('authToken');
            localStorage.removeItem('userRole');
          }
        }
      } catch (error) {
        console.error('Auth check failed:', error);
        localStorage.removeItem('authToken');
        localStorage.removeItem('userRole');
      } finally {
        setIsLoading(false);
      }
    };

    checkAuth();
  }, []);

  const login = async (credentials: LoginRequest) => {
    try {
      setIsLoading(true);
      const response = await apiClient.post<AuthToken>('/auth/login', credentials);

      if (!response.success || !response.data) {
        throw new Error(response.error?.message || 'Login failed');
      }

      const token = response.data.accessToken;
      localStorage.setItem('authToken', token);

      // Fetch user details after login
      const userResponse = await apiClient.get<User>('/auth/me');
      if (userResponse.success && userResponse.data) {
        setUser(userResponse.data);
        localStorage.setItem('userRole', userResponse.data.role);
        toast.success('Login successful');
        return;
      }

      throw new Error('Failed to fetch user details');
    } catch (error) {
      localStorage.removeItem('authToken');
      const errorMessage = error instanceof Error ? error.message : 'Login failed';
      toast.error(errorMessage);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const register = async (data: RegisterRequest) => {
    try {
      setIsLoading(true);
      const response = await apiClient.post<AuthToken>('/auth/register', data);

      if (!response.success || !response.data) {
        throw new Error(response.error?.message || 'Registration failed');
      }

      const token = response.data.accessToken;
      localStorage.setItem('authToken', token);

      // Fetch user details after registration
      const userResponse = await apiClient.get<User>('/auth/me');
      if (userResponse.success && userResponse.data) {
        setUser(userResponse.data);
        localStorage.setItem('userRole', userResponse.data.role);
        toast.success('Registration successful');
        return;
      }

      throw new Error('Failed to fetch user details');
    } catch (error) {
      localStorage.removeItem('authToken');
      const errorMessage = error instanceof Error ? error.message : 'Registration failed';
      toast.error(errorMessage);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('userRole');
    setUser(null);
    toast.success('Logged out successfully');
  };

  const updateUser = (updatedUser: User) => {
    setUser(updatedUser);
    localStorage.setItem('userRole', updatedUser.role);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isLoading,
        isAuthenticated: !!user,
        login,
        register,
        logout,
        updateUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}
