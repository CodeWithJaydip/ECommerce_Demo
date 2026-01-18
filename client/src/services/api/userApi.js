import { get, put } from './apiClient';

/**
 * Get paginated list of users with filtering and sorting
 * @param {Object} params - Query parameters for pagination, filtering, and sorting
 * @returns {Promise<Object>} PagedResult with users and metadata
 */
export const getUsers = async (params = {}) => {
  const queryParams = new URLSearchParams();
  
  if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber);
  if (params.pageSize) queryParams.append('pageSize', params.pageSize);
  if (params.firstName) queryParams.append('firstName', params.firstName);
  if (params.lastName) queryParams.append('lastName', params.lastName);
  if (params.email) queryParams.append('email', params.email);
  if (params.phoneNumber) queryParams.append('phoneNumber', params.phoneNumber);
  if (params.isActive !== undefined && params.isActive !== null) queryParams.append('isActive', params.isActive);
  if (params.roleName) queryParams.append('roleName', params.roleName);
  if (params.isLocked !== undefined && params.isLocked !== null) queryParams.append('isLocked', params.isLocked);
  if (params.sortBy) queryParams.append('sortBy', params.sortBy);
  if (params.sortDescending !== undefined) queryParams.append('sortDescending', params.sortDescending);

  const queryString = queryParams.toString();
  const endpoint = `/api/user${queryString ? `?${queryString}` : ''}`;
  
  return await get(endpoint);
};

/**
 * Get user by ID
 * @param {number} id - User ID
 * @returns {Promise<Object>} User object
 */
export const getUserById = async (id) => {
  return await get(`/api/user/${id}`);
};

/**
 * Update user information
 * @param {number} id - User ID
 * @param {Object} data - User update data
 * @returns {Promise<Object>} Updated user object
 */
export const updateUser = async (id, data) => {
  return await put(`/api/user/${id}`, data);
};

/**
 * Update user roles
 * @param {number} id - User ID
 * @param {Object} data - { roleIds: number[] }
 * @returns {Promise<Object>} Updated user object
 */
export const updateUserRoles = async (id, data) => {
  return await put(`/api/user/${id}/roles`, data);
};
