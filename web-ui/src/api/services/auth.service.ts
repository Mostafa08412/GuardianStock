/**
 * Authentication Service
 * Handles all auth-related API calls with real backend JWT authentication
 */

import { API_CONFIG } from '../config';
import {
  LoginRequest,
  RegisterCommand,
  ChangePasswordCommand,
  RefreshTokenCommand,
  GoogleLoginCommand,
  UpdateProfileCommand,
  AuthenticationResponseApiResponse,
  IdentityUserDtoApiResponse,
  ApiResponse,
  AuthenticationResponse,
  IdentityUserDto,
  ForgetPasswordCommand,
  VerifyResetPasswordOtpResponseApiResponse,
  ResetPasswordCommand,
  VerifyResetPasswordOtpCommand,
} from '../types/auth.types';

const AUTH_ENDPOINTS = {
  login: '/v1/auth/login',
  register: '/v1/auth/register',
  changePassword: '/v1/auth/change-password',
  logout: '/v1/auth/logout',
  refreshToken: '/v1/auth/refresh-token',
  googleLogin: '/v1/auth/google-login',
  updateProfile: '/v1/auth/update-profile',
  forgetPassword: '/v1/auth/forget-password',
  verifyOtp: '/v1/auth/verify-reset-password-otp',
  resetPassword: '/v1/auth/reset-password',
};

// Token storage keys
const ACCESS_TOKEN_KEY = 'access_token';
const REFRESH_TOKEN_KEY = 'refresh_token';
const USER_KEY = 'auth_user';

class AuthService {
  private baseUrl: string;

  constructor() {
    this.baseUrl = API_CONFIG.baseUrl;
  }

  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;

    const headers: HeadersInit = {
      'Content-Type': 'application/json',
      ...options.headers,
    };

    // Add auth token if available
    const token = this.getAccessToken();
    if (token) {
      (headers as Record<string, string>)['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(url, {
      ...options,
      headers,
    });

    const data = await response.json();
    return data;
  }

  // Token Management
  getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  getStoredUser(): IdentityUserDto | null {
    const userStr = localStorage.getItem(USER_KEY);
    if (userStr) {
      try {
        return JSON.parse(userStr);
      } catch {
        return null;
      }
    }
    return null;
  }

  /**
   * Decode the JWT `exp` claim to get the token expiry time in milliseconds.
   * Returns null if token is missing or cannot be decoded.
   */
  getTokenExpiry(): number | null {
    const token = this.getAccessToken();
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp ? payload.exp * 1000 : null;
    } catch {
      return null;
    }
  }

  /**
   * Check if the current access token is expired.
   */
  isTokenExpired(): boolean {
    const expiry = this.getTokenExpiry();
    if (!expiry) return true;
    return Date.now() >= expiry;
  }

  private saveTokens(response: AuthenticationResponse): void {
    if (response.accessToken?.token) {
      localStorage.setItem(ACCESS_TOKEN_KEY, response.accessToken.token);
    }
    if (response.refreshToken?.token) {
      localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken.token);
    }
    if (response.user) {
      localStorage.setItem(USER_KEY, JSON.stringify(response.user));
    }
  }

  clearTokens(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }

  isAuthenticated(): boolean {
    return !!this.getAccessToken() && !this.isTokenExpired();
  }

  // Auth API Methods
  async login(credentials: LoginRequest): Promise<AuthenticationResponseApiResponse> {
    const response = await this.request<AuthenticationResponseApiResponse>(
      AUTH_ENDPOINTS.login,
      {
        method: 'POST',
        body: JSON.stringify(credentials),
      }
    );

    if (response.isSuccess && response.data) {
      this.saveTokens(response.data);
    }

    return response;
  }

  async register(data: RegisterCommand): Promise<ApiResponse<void>> {
    const response = await this.request<ApiResponse<void>>(
      AUTH_ENDPOINTS.register,
      {
        method: 'POST',
        body: JSON.stringify(data),
      }
    );

    return response;
  }

  async changePassword(data: ChangePasswordCommand): Promise<ApiResponse<void>> {
    const response = await this.request<ApiResponse<void>>(
      AUTH_ENDPOINTS.changePassword,
      {
        method: 'POST',
        body: JSON.stringify(data),
      }
    );

    return response;
  }

  async logout(): Promise<void> {
    try {
      const refreshToken = this.getRefreshToken();
      await this.request<ApiResponse<void>>(AUTH_ENDPOINTS.logout, {
        method: 'POST',
        body: JSON.stringify({ refreshToken }),
      });
    } finally {
      this.clearTokens();
    }
  }

  async refreshToken(): Promise<AuthenticationResponseApiResponse | null> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      return null;
    }

    const data: RefreshTokenCommand = { refreshToken };

    try {
      const response = await this.request<AuthenticationResponseApiResponse>(
        AUTH_ENDPOINTS.refreshToken,
        {
          method: 'POST',
          body: JSON.stringify(data),
        }
      );

      if (response.isSuccess && response.data) {
        this.saveTokens(response.data);
      } else {
        this.clearTokens();
      }

      return response;
    } catch {
      this.clearTokens();
      return null;
    }
  }

  async googleLogin(idToken: string): Promise<AuthenticationResponseApiResponse> {
    const response = await this.request<AuthenticationResponseApiResponse>(
      AUTH_ENDPOINTS.googleLogin,
      {
        method: 'POST',
        body: JSON.stringify({ idToken }),
      }
    );

    if (response.isSuccess && response.data) {
      this.saveTokens(response.data);
    }

    return response;
  }

  async updateProfile(data: UpdateProfileCommand): Promise<IdentityUserDtoApiResponse> {
    const response = await this.request<IdentityUserDtoApiResponse>(
      AUTH_ENDPOINTS.updateProfile,
      {
        method: 'PUT',
        body: JSON.stringify(data),
      }
    );

    if (response.isSuccess && response.data) {
      localStorage.setItem(USER_KEY, JSON.stringify(response.data));
    }

    return response;
  }

  async forgetPassword(data: ForgetPasswordCommand): Promise<ApiResponse<void>> {
    const response = await this.request<ApiResponse<void>>(
      AUTH_ENDPOINTS.forgetPassword,
      {
        method: 'POST',
        body: JSON.stringify(data),
      }
    );

    return response;
  }

  async verifyOtp(data: VerifyResetPasswordOtpCommand): Promise<VerifyResetPasswordOtpResponseApiResponse> {
    const response = await this.request<VerifyResetPasswordOtpResponseApiResponse>(
      AUTH_ENDPOINTS.verifyOtp,
      {
        method: 'POST',
        body: JSON.stringify(data),
      }
    );

    return response;
  }

  async resetPassword(data: ResetPasswordCommand): Promise<ApiResponse<void>> {
    const response = await this.request<ApiResponse<void>>(
      AUTH_ENDPOINTS.resetPassword,
      {
        method: 'POST',
        body: JSON.stringify(data),
      }
    );

    return response;
  }
}

export const authService = new AuthService();

