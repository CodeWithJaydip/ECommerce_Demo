import { post } from './apiClient';

/**
 * Login user
 * @param {Object} credentials - { email, password }
 * @returns {Promise<Object>} AuthResponse with token, refreshToken, expiresAt, and user
 */
export const login = async (credentials) => {
  return await post('/api/auth/login', credentials);
};

/**
 * Register new user
 * @param {Object} userData - { firstName, lastName, email, password, confirmPassword, phoneNumber? }
 * @returns {Promise<Object>} AuthResponse with token, refreshToken, expiresAt, and user
 */
export const register = async (userData) => {
  return await post('/api/auth/register', userData);
};
