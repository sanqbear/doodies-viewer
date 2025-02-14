import {all} from 'redux-saga/effects';
import {watchAppSaga} from '../context/app/app-saga';

export default function* rootSaga() {
  yield all([watchAppSaga()]);
}
