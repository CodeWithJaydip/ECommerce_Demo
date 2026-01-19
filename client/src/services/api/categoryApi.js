import { get, del } from './apiClient';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5176';

/**
 * Helper function to make FormData requests
 */
const formDataRequest = async (endpoint, method, formData) => {
  const token = localStorage.getItem('token');
  
  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    method,
    headers: {
      ...(token && { Authorization: `Bearer ${token}` }),
    },
    body: formData,
  });

  const data = await response.json();

  if (!response.ok) {
    const errorMessage = data.message || data.errors || 'An error occurred';
    throw new Error(errorMessage);
  }

  if (data.success !== undefined) {
    if (!data.success) {
      const errorMessage = data.message || data.errors || 'An error occurred';
      throw new Error(errorMessage);
    }
    return data.data;
  }

  return data;
};

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
 * @param {Object} data - Category data { name: string, description?: string, image?: File }
 * @returns {Promise<Object>} Created category object
 */
export const createCategory = async (data) => {
  const formData = new FormData();
  formData.append('name', data.name);
  if (data.description) {
    formData.append('description', data.description);
  }
  if (data.image) {
    formData.append('image', data.image);
  }
  return await formDataRequest('/api/category', 'POST', formData);
};

/**
 * Update an existing category
 * @param {number} id - Category ID
 * @param {Object} data - Category update data { name: string, description?: string, image?: File, removeImage?: boolean }
 * @returns {Promise<Object>} Updated category object
 */
export const updateCategory = async (id, data) => {
  const formData = new FormData();
  formData.append('name', data.name);
  if (data.description) {
    formData.append('description', data.description);
  }
  if (data.image) {
    formData.append('image', data.image);
  }
  if (data.removeImage === true) {
    formData.append('removeImage', 'true');
  }
  return await formDataRequest(`/api/category/${id}`, 'PUT', formData);
};

/**
 * Delete (soft delete) a category
 * @param {number} id - Category ID
 * @returns {Promise<Object>} Success response
 */
export const deleteCategory = async (id) => {
  return await del(`/api/category/${id}`);
};
