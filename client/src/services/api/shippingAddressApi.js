import { get, post, put, del } from './apiClient';

export const getAddresses = async () => {
  return await get('/api/shippingaddress');
};

export const getAddress = async (id) => {
  return await get(`/api/shippingaddress/${id}`);
};

export const createAddress = async (data) => {
  return await post('/api/shippingaddress', data);
};

export const updateAddress = async (id, data) => {
  return await put(`/api/shippingaddress/${id}`, data);
};

export const deleteAddress = async (id) => {
  return await del(`/api/shippingaddress/${id}`);
};
