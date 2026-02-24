import React, { createContext, useContext, useState, useEffect, useRef, ReactNode, useCallback } from 'react';
import { authService } from '@/api/services/auth.service';
import { IdentityUserDto, LoginRequest, RegisterCommand } from '@/api/types/auth.types';

interface AuthContextType {
  user: IdentityUserDto | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (credentials: LoginRequest) => Promise<{ success: boolean; error?: string }>;
  googleLogin: (idToken: string) => Promise<{ success: boolean; error?: string }>;
  register: (data: RegisterCommand) => Promise<{ success: boolean; error?: string }>;
  updateProfile: (data: { firstName: string; lastName: string }) => Promise<{ success: boolean; error?: string }>;
  changePassword: (data: any) => Promise<{ success: boolean; error?: string }>;
  logout: () => Promise<void>;
  getAccessToken: () => string | null;
  forgetPassword: (email: string) => Promise<{ success: boolean; error?: string; errorCode?: string; validationErrors?: Record<string, string> }>;
  verifyOtp: (email: string, otp: string) => Promise<{ success: boolean; token?: string; error?: string; errorCode?: string; validationErrors?: Record<string, string> }>;
  resetPassword: (data: { email: string; token: string; newPassword: string }) => Promise<{ success: boolean; error?: string; errorCode?: string; validationErrors?: Record<string, string> }>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<IdentityUserDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const refreshTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const clearRefreshTimer = useCallback(() => {
    if (refreshTimerRef.current) {
      clearTimeout(refreshTimerRef.current);
      refreshTimerRef.current = null;
    }
  }, []);

  const handleRefresh = useCallback(async () => {
    try {
      const result = await authService.refreshToken();
      if (result?.isSuccess && result.data?.user) {
        setUser(result.data.user);
        // Schedule next refresh based on new token
        scheduleRefresh();
      } else {
        // Refresh failed — force logout
        authService.clearTokens();
        setUser(null);
      }
    } catch {
      authService.clearTokens();
      setUser(null);
    }
  }, []);

  const scheduleRefresh = useCallback(() => {
    clearRefreshTimer();

    const expiry = authService.getTokenExpiry();
    if (!expiry) return;

    // Refresh 60 seconds before the token expires
    const refreshIn = expiry - Date.now() - 60_000;

    if (refreshIn <= 0) {
      // Token already near-expired or expired, refresh immediately
      handleRefresh();
      return;
    }

    refreshTimerRef.current = setTimeout(() => {
      handleRefresh();
    }, refreshIn);
  }, [clearRefreshTimer, handleRefresh]);

  // Initialize auth state from stored tokens
  useEffect(() => {
    const initializeAuth = async () => {
      const storedUser = authService.getStoredUser();
      const hasToken = !!authService.getAccessToken();

      if (hasToken && storedUser) {
        if (authService.isTokenExpired()) {
          // Access token expired — try to refresh
          const result = await authService.refreshToken();
          if (result?.isSuccess && result.data?.user) {
            setUser(result.data.user);
            scheduleRefresh();
          } else {
            authService.clearTokens();
          }
        } else {
          setUser(storedUser);
          scheduleRefresh();
        }
      }

      setIsLoading(false);
    };

    initializeAuth();

    return () => {
      clearRefreshTimer();
    };
  }, []);

  const login = useCallback(async (credentials: LoginRequest): Promise<{ success: boolean; error?: string }> => {
    try {
      const response = await authService.login(credentials);

      if (response.isSuccess && response.data?.user) {
        setUser(response.data.user);
        scheduleRefresh();
        return { success: true };
      }

      // Handle validation errors
      if (response.validationErrors) {
        const errorMessages = Object.values(response.validationErrors).join(', ');
        return { success: false, error: errorMessages };
      }

      return { success: false, error: response.message || 'Login failed' };
    } catch (error) {
      console.error('Login error:', error);
      return { success: false, error: 'An unexpected error occurred' };
    }
  }, [scheduleRefresh]);

  const googleLogin = useCallback(async (idToken: string): Promise<{ success: boolean; error?: string }> => {
    try {
      const response = await authService.googleLogin(idToken);

      if (response.isSuccess && response.data?.user) {
        setUser(response.data.user);
        scheduleRefresh();
        return { success: true };
      }

      if (response.validationErrors) {
        const errorMessages = Object.values(response.validationErrors).join(', ');
        return { success: false, error: errorMessages };
      }

      return { success: false, error: response.message || 'Google login failed' };
    } catch (error) {
      console.error('Google login error:', error);
      return { success: false, error: 'An unexpected error occurred' };
    }
  }, [scheduleRefresh]);

  const register = useCallback(async (data: RegisterCommand): Promise<{ success: boolean; error?: string }> => {
    try {
      const response = await authService.register(data);

      if (response.isSuccess) {
        return { success: true };
      }

      // Handle validation errors
      if (response.validationErrors) {
        const errorMessages = Object.values(response.validationErrors).join(', ');
        return { success: false, error: errorMessages };
      }

      return { success: false, error: response.message || 'Registration failed' };
    } catch (error) {
      console.error('Register error:', error);
      return { success: false, error: 'An unexpected error occurred' };
    }
  }, []);

  const updateProfile = useCallback(async (data: { firstName: string; lastName: string }): Promise<{ success: boolean; error?: string }> => {
    try {
      const response = await authService.updateProfile(data);

      if (response.isSuccess && response.data) {
        setUser(response.data);
        return { success: true };
      }

      if (response.validationErrors) {
        const errorMessages = Object.values(response.validationErrors).join(', ');
        return { success: false, error: errorMessages };
      }

      return { success: false, error: response.message || 'Profile update failed' };
    } catch (error) {
      console.error('Profile update error:', error);
      return { success: false, error: 'An unexpected error occurred' };
    }
  }, []);

  const changePassword = useCallback(async (data: any): Promise<{ success: boolean; error?: string }> => {
    try {
      const response = await authService.changePassword(data);

      if (response.isSuccess) {
        return { success: true };
      }

      if (response.validationErrors) {
        const errorMessages = Object.values(response.validationErrors).join(', ');
        return { success: false, error: errorMessages };
      }

      return { success: false, error: response.message || 'Password change failed' };
    } catch (error) {
      console.error('Password change error:', error);
      return { success: false, error: 'An unexpected error occurred' };
    }
  }, []);

  const logout = useCallback(async () => {
    clearRefreshTimer();
    try {
      await authService.logout();
    } catch (error) {
      console.error('Logout error:', error);
      // Still clear tokens even if API fails
      authService.clearTokens();
    } finally {
      setUser(null);
    }
  }, [clearRefreshTimer]);

  const getAccessToken = useCallback(() => {
    return authService.getAccessToken();
  }, []);

  const forgetPassword = useCallback(async (email: string): Promise<{ success: boolean; error?: string; errorCode?: string; validationErrors?: Record<string, string> }> => {
    try {
      const response = await authService.forgetPassword({ emailAddress: email });
      if (response.isSuccess) {
        return { success: true };
      }
      return {
        success: false,
        error: response.message || 'Failed to send OTP',
        errorCode: response.errorCode || undefined,
        validationErrors: response.validationErrors || undefined
      };
    } catch (error) {
      return { success: false, error: 'An unexpected error occurred' };
    }
  }, []);

  const verifyOtp = useCallback(async (email: string, otp: string): Promise<{ success: boolean; token?: string; error?: string; errorCode?: string; validationErrors?: Record<string, string> }> => {
    try {
      const response = await authService.verifyOtp({ emailAddress: email, otp });
      if (response.isSuccess && response.data) {
        return { success: true, token: response.data.resetPasswordToken };
      }
      return {
        success: false,
        error: response.message || 'OTP verification failed',
        errorCode: response.errorCode || undefined,
        validationErrors: response.validationErrors || undefined
      };
    } catch (error) {
      return { success: false, error: 'An unexpected error occurred' };
    }
  }, []);

  const resetPassword = useCallback(async (data: { email: string; token: string; newPassword: string }): Promise<{ success: boolean; error?: string; errorCode?: string; validationErrors?: Record<string, string> }> => {
    try {
      const response = await authService.resetPassword({
        emailAddress: data.email,
        resetPasswordToken: data.token,
        newPassword: data.newPassword
      });
      if (response.isSuccess) {
        return { success: true };
      }
      return {
        success: false,
        error: response.message || 'Password reset failed',
        errorCode: response.errorCode || undefined,
        validationErrors: response.validationErrors || undefined
      };
    } catch (error) {
      return { success: false, error: 'An unexpected error occurred' };
    }
  }, []);

  const value: AuthContextType = {
    user,
    isAuthenticated: !!user,
    isLoading,
    login,
    googleLogin,
    register,
    updateProfile,
    changePassword,
    logout,
    getAccessToken,
    forgetPassword,
    verifyOtp,
    resetPassword,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

