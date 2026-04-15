export interface LoginRequest    { email: string; password: string; }
export interface RegisterRequest { email: string; password: string; displayName: string; }
export interface RefreshRequest  { refreshToken: string; }
export interface RevokeRequest   { token: string; }

export interface AuthResponse {
  accessToken:  string;
  refreshToken: string;
  expiresAt:    string;
  user:         CurrentUser;
}

/** Subset of user info kept in-memory after login. No sensitive fields. */
export interface CurrentUser {
  id:          string;   // GUID
  email:       string;
  displayName: string;
}
