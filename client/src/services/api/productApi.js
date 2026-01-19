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
 * Get all products (public access)
 * @returns {Promise<Array>} Array of product objects
 */
export const getProducts = async () => {
  return await get('/api/product');
};

/**
 * Get product by ID (public access)
 * @param {number} id - Product ID
 * @returns {Promise<Object>} Product object
 */
export const getProductById = async (id) => {
  return await get(`/api/product/${id}`);
};

/**
 * Create a new product
 * @param {Object} data - Product data { name, description, price, stock, categoryId, sellerId?, sku?, image?: File }
 * @returns {Promise<Object>} Created product object
 */
export const createProduct = async (data) => {
  const formData = new FormData();
  formData.append('name', data.name);
  if (data.description) {
    formData.append('description', data.description);
  }
  formData.append('price', data.price.toString());
  formData.append('stock', data.stock.toString());
  formData.append('categoryId', data.categoryId.toString());
  if (data.sellerId) {
    formData.append('sellerId', data.sellerId.toString());
  }
  if (data.sku) {
    formData.append('sku', data.sku);
  }
  if (data.image) {
    formData.append('image', data.image);
  }
  return await formDataRequest('/api/product', 'POST', formData);
};

/**
 * Update an existing product
 * @param {number} id - Product ID
 * @param {Object} data - Product update data { name, description, price, stock, categoryId, sku?, image?: File, removeImage?: boolean }
 * @returns {Promise<Object>} Updated product object
 */
export const updateProduct = async (id, data) => {
  const formData = new FormData();
  formData.append('name', data.name);
  if (data.description) {
    formData.append('description', data.description);
  }
  formData.append('price', data.price.toString());
  formData.append('stock', data.stock.toString());
  formData.append('categoryId', data.categoryId.toString());
  if (data.sku) {
    formData.append('sku', data.sku);
  }
  if (data.image) {
    formData.append('image', data.image);
  }
  if (data.removeImage === true) {
    formData.append('removeImage', 'true');
  }
  return await formDataRequest(`/api/product/${id}`, 'PUT', formData);
};

/**
 * Delete (soft delete) a product
 * @param {number} id - Product ID
 * @returns {Promise<Object>} Success response
 */
export const deleteProduct = async (id) => {
  return await del(`/api/product/${id}`);
};
