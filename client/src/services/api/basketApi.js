import { get, post, put, del } from './apiClient';

export const getBasket = async () => {
  return await get('/api/basket');
};

export const getItemCount = async () => {
  return await get('/api/basket/count');
};

export const addItem = async (data) => {
  return await post('/api/basket/items', data);
};

export const updateItem = async (productId, data) => {
  return await put(`/api/basket/items/${productId}`, data);
};

export const removeItem = async (productId) => {
  return await del(`/api/basket/items/${productId}`);
};

export const clearBasket = async () => {
  return await del('/api/basket');
};

export const getCheckoutSummary = async (addressId) => {
  const params = addressId ? `?addressId=${addressId}` : '';
  return await get(`/api/basket/checkout-summary${params}`);
};
