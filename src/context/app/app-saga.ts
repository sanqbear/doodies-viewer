import {call, put, takeLatest} from 'redux-saga/effects';
import {initFailed, tryInitRequest} from './app-reducer';
import {initSuccess} from './app-reducer';
import findUrl from '@/api/url-finder';
import {PayloadAction} from '@reduxjs/toolkit';

function* initAppSaga(action: PayloadAction<{idx: number}>) {
  try {
    let url = '';
    url = yield call(async () => {
      try {
        return await findUrl(`https://manatoki${action.payload.idx || 0}.net`);
      } catch (error) {
        return null;
      }
    });

    if (url) {
      yield put(initSuccess({url}));
    } else {
      yield put(initFailed());
    }
  } catch (error) {
    yield put(initFailed());
  }
}

export function* watchAppSaga() {
  yield takeLatest(tryInitRequest, initAppSaga);
}
