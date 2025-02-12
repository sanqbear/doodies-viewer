interface AppState {
  theme: 'light' | 'dark';
  isUrlGreen: boolean;
  isLoading: boolean;
  apiUrl: string;
  language: string;
}

interface SetThemeAction {
  type: 'SET_THEME';
  payload: 'light' | 'dark';
}

interface SetIsUrlGreenAction {
  type: 'SET_IS_URL_GREEN';
  payload: boolean;
}

interface TryChangeApiUrlAction {
  type: 'TRY_CHANGE_API_URL';
}

interface SetApiUrlAction {
  type: 'SET_API_URL';
  payload: string;
}

interface SetLanguageAction {
  type: 'SET_LANGUAGE';
  payload: string;
}

type AppAction =
  | SetThemeAction
  | SetIsUrlGreenAction
  | TryChangeApiUrlAction
  | SetApiUrlAction
  | SetLanguageAction;

const initialState: AppState = {
  theme: 'light',
  isUrlGreen: false,
  isLoading: false,
  apiUrl: '',
  language: 'en',
};

const appReducer = (state = initialState, action: AppAction): AppState => {
  switch (action.type) {
    case 'SET_THEME':
      return {...state, theme: action.payload};
    case 'SET_IS_URL_GREEN':
      return {...state, isUrlGreen: action.payload};
    case 'SET_API_URL':
      return {...state, apiUrl: action.payload, isLoading: false};
    case 'SET_LANGUAGE':
      return {...state, language: action.payload};
    case 'TRY_CHANGE_API_URL':
      return {...state, isLoading: true};
    default:
      return state;
  }
};

export const setTheme = (theme: 'light' | 'dark') => ({
  type: 'SET_THEME',
  payload: theme,
});

export const setIsUrlGreen = (isUrlGreen: boolean) => ({
  type: 'SET_IS_URL_GREEN',
  payload: isUrlGreen,
});

export const tryChangeApiUrl = () => ({
  type: 'TRY_CHANGE_API_URL',
});

export const setApiUrl = (apiUrl: string) => ({
  type: 'SET_API_URL',
  payload: apiUrl,
});

export const setLanguage = (language: string) => ({
  type: 'SET_LANGUAGE',
  payload: language,
});

export default appReducer;
