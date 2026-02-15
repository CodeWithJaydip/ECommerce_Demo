import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import * as basketApi from '../../services/api/basketApi';

const initialState = {
  itemCount: 0,
  isLoading: false,
};

export const fetchBasketCount = createAsyncThunk(
  'basket/fetchCount',
  async (_, { rejectWithValue }) => {
    try {
      const count = await basketApi.getItemCount();
      return count;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);

const basketSlice = createSlice({
  name: 'basket',
  initialState,
  reducers: {
    setBasketCount: (state, action) => {
      state.itemCount = action.payload;
    },
    resetBasket: (state) => {
      state.itemCount = 0;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchBasketCount.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(fetchBasketCount.fulfilled, (state, action) => {
        state.isLoading = false;
        state.itemCount = action.payload;
      })
      .addCase(fetchBasketCount.rejected, (state) => {
        state.isLoading = false;
      });
  },
});

export const { setBasketCount, resetBasket } = basketSlice.actions;
export default basketSlice.reducer;
