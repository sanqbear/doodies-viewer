import {combineReducers} from 'redux';
import appReducer from '../context/app/app-reducer';

const rootReducer = combineReducers({
  app: appReducer,
});

export type RootState = ReturnType<typeof rootReducer>;

export default rootReducer;
