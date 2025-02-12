import { createSlice, PayloadAction } from '@reduxjs/toolkit';


interface AppState {
  isLoading: boolean;
  isGreen: boolean;
  url: string;
  message: string;
  lastTriedIndex: number;
}

const initialState: AppState = {
  isLoading: false,
  isGreen: false,
  url: '',
  message: '',
  lastTriedIndex: 0,
};

const appSlice = createSlice({
  name: 'app',
  initialState,
  reducers: {
    initRequest: (state) => {
      state.isLoading = true;
    },
    updateInitState: (state, action: PayloadAction<{lastTriedIndex: number}>) => {
      state.isLoading = true;
      state.lastTriedIndex = action.payload.lastTriedIndex;
    },
    initSuccess: (state) => {
      state.isLoading = false;
      state.isGreen = true;
    },
    initFailed: (state) => {
      state.isLoading = false;
      state.message = 'Failed to initialize';
      state.isGreen = false;
      state.lastTriedIndex = 0;
    },
  },
});

export const { initRequest, updateInitState, initSuccess, initFailed } = appSlice.actions;
export default appSlice.reducer;
