/**
 * Authentication API Types
 * Generated from OpenAPI spec
 */

// Request Types
import { UserRole } from '@/types/inventory';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterCommand {
  email: string;
  firstName: string;
  lastName: string;
  password: string;
}

export interface ChangePasswordCommand {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

export interface LogoutCommand { }

export interface RefreshTokenCommand {
  refreshToken: string;
}

export interface GoogleLoginCommand {
  idToken: string;
}

export interface UpdateProfileCommand {
  firstName: string;
  lastName: string;
}

export interface ForgetPasswordCommand {
  emailAddress: string;
}

export interface VerifyResetPasswordOtpCommand {
  emailAddress: string;
  otp: string;
}

export interface VerifyResetPasswordOtpResponse {
  resetPasswordToken: string;
}

export interface ResetPasswordCommand {
  emailAddress: string;
  resetPasswordToken: string;
  newPassword: string;
}

// Response Types
export interface AccessTokenDto {
  token: string | null;
}

export interface RefreshTokenDto {
  token: string | null;
}

export interface IdentityUserDto {
  id: string | null;
  firstName: string | null;
  lastName: string | null;
  fullName: string | null;
  email: string | null;
  userName: string | null;
  roles: string[] | null;
}

export interface AuthenticationResponse {
  user: IdentityUserDto | null;
  accessToken: AccessTokenDto | null;
  refreshToken: RefreshTokenDto | null;
}

export interface ApiResponse<T> {
  isSuccess: boolean;
  message: string | null;
  errorCode: string | null;
  validationErrors: Record<string, string> | null;
  meta: Record<string, string> | null;
  instance: string | null;
  traceId: string | null;
  data: T | null;
}

export interface NGApiResponse {
  isSuccess: boolean;
  message: string | null;
  errorCode: string | null;
  validationErrors: Record<string, string> | null;
  meta: Record<string, string> | null;
  instance: string | null;
  traceId: string | null;
}

export interface IdentityUserDtoApiResponse extends ApiResponse<IdentityUserDto> { }

export type AuthenticationResponseApiResponse = ApiResponse<AuthenticationResponse>;

export type VerifyResetPasswordOtpResponseApiResponse = ApiResponse<VerifyResetPasswordOtpResponse>;
