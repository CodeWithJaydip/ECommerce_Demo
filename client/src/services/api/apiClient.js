const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5176';

/**
 * Base fetch wrapper with error handling
 */
export const apiClient = async (endpoint, options = {}) => {
  const token = localStorage.getItem('token');
  
  const config = {
    headers: {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` }),
      ...options.headers,
    },
    ...options,
  };

  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, config);
    const data = await response.json();

    // Helper function to safely extract error message
    const getErrorMessage = (responseData) => {
      if (responseData.message) {
        return responseData.message;
      }
      
      // Handle errors array - check if it's an array first
      if (responseData.errors) {
        if (Array.isArray(responseData.errors)) {
          return responseData.errors.length > 0 
            ? responseData.errors.join(', ') 
            : 'An error occurred';
        }
        // If errors is not an array, try to convert it
        if (typeof responseData.errors === 'string') {
          return responseData.errors;
        }
        if (typeof responseData.errors === 'object') {
          // Handle object with error messages
          const errorValues = Object.values(responseData.errors).flat();
          return Array.isArray(errorValues) 
            ? errorValues.join(', ') 
            : 'An error occurred';
        }
      }
      
      return 'An error occurred';
    };

    if (!response.ok) {
      // Handle API error response (400, 401, 500, etc.)
      const errorMessage = getErrorMessage(data);
      throw new Error(errorMessage);
    }

    // Check if the API response has the standard ApiResponse structure
    if (data.success !== undefined) {
      if (!data.success) {
        const errorMessage = getErrorMessage(data);
        throw new Error(errorMessage);
      }
      return data.data;
    }

    return data;
  } catch (error) {
    if (error instanceof Error) {
      throw error;
    }
    throw new Error('Network error. Please try again.');
  }
};

/**
 * GET request
 */
export const get = (endpoint, options = {}) => {
  return apiClient(endpoint, { ...options, method: 'GET' });
};

/**
 * POST request
 */
export const post = (endpoint, data, options = {}) => {
  return apiClient(endpoint, {
    ...options,
    method: 'POST',
    body: JSON.stringify(data),
  });
};

/**
 * PUT request
 */
export const put = (endpoint, data, options = {}) => {
  return apiClient(endpoint, {
    ...options,
    method: 'PUT',
    body: JSON.stringify(data),
  });
};

/**
 * DELETE request
 */
export const del = (endpoint, options = {}) => {
  return apiClient(endpoint, { ...options, method: 'DELETE' });
};
