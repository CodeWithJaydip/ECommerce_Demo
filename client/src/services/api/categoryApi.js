import { get, post, put, del } from './apiClient';

/**
 * Get all categories
 * @returns {Promise<Array>} Array of category objects
 */
export const getCategories = async () => {
  return await get('/api/category');
};

/**
 * Get category by ID
 * @param {number} id - Category ID
 * @returns {Promise<Object>} Category object
 */
export const getCategoryById = async (id) => {
  return await get(`/api/category/${id}`);
};

/**
 * Create a new category
 * @param {Object} data - Category data { name: string, description?: string }
 * @returns {Promise<Object>} Created category object
 */
export const createCategory = async (data) => {
  return await post('/api/category', data);
};

/**
 * Update an existing category
 * @param {number} id - Category ID
 * @param {Object} data - Category update data { name: string, description?: string }
 * @returns {Promise<Object>} Updated category object
 */
export const updateCategory = async (id, data) => {
  return await put(`/api/category/${id}`, data);
};

/**
 * Delete (soft delete) a category
 * @param {number} id - Category ID
 * @returns {Promise<Object>} Success response
 */
export const deleteCategory = async (id) => {
  return await del(`/api/category/${id}`);
};
