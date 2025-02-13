import {createSlice, PayloadAction} from '@reduxjs/toolkit';

interface AppState {
  isLoading: boolean;
  isGreen: boolean;
  url: string;
  message: string;
  lastTriedIndex: number;
  isCanceled: boolean;
}

const initialState: AppState = {
  isLoading: false,
  isGreen: false,
  url: '',
  message: '',
  lastTriedIndex: 0,
  isCanceled: false,
};

const appSlice = createSlice({
  name: 'app',
  initialState,
  reducers: {
    loadAppState: (state, action: PayloadAction<AppState>) => {
      state.isLoading = action.payload.isLoading;
      state.isGreen = action.payload.isGreen;
      state.url = action.payload.url;
      state.message = '';
      state.lastTriedIndex = action.payload.lastTriedIndex;
      state.isCanceled = false;
    },
    tryInitRequest: (state, action: PayloadAction<{idx: number}>) => {
      state.isLoading = true;
      state.lastTriedIndex = action.payload.idx;
      state.isCanceled = false;
    },
    initSuccess: (state, action: PayloadAction<{url: string}>) => {
      state.isLoading = false;
      state.isCanceled = false;
      state.url = action.payload.url;
    },
    initComplete: state => {
      state.isLoading = false;
      state.isGreen = true;
      state.isCanceled = false;
    },
    initFailed: state => {
      state.isLoading = false;
      state.isGreen = false;
      state.isCanceled = false;
      state.lastTriedIndex = state.lastTriedIndex + 1;
    },
    cancelApp: state => {
      state.isCanceled = true;
    },
    resetApp: state => {
      state.isLoading = false;
      state.isGreen = false;
      state.url = '';
      state.message = '';
      state.lastTriedIndex = 0;
      state.isCanceled = false;
    },
  },
});

export const {
  tryInitRequest,
  initSuccess,
  initComplete,
  initFailed,
  resetApp,
  cancelApp,
  loadAppState,
} = appSlice.actions;
export default appSlice.reducer;
