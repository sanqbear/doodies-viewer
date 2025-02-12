import {call, put, takeLatest} from 'redux-saga/effects';
import {initFailed, initRequest, updateInitState} from './app-reducer';
import {initSuccess} from './app-reducer';
import findUrl from '@/api/url-finder';

function* initAppSaga() {
  try {
    let url = '';
    let idx = 0;
    do {
      url = yield call(async () => {
        try {
          return await findUrl(`https://manatoki${idx}.net`);
        } catch (error) {
          return null;
        }
      });

      if (url) {
        break;
      } else {
        idx++;
        yield put(updateInitState({lastTriedIndex: idx}));
      }
    } while (!url && idx < 1000);
    if (url) {
      yield put(initSuccess());
    }
  } catch (error) {
    yield put(initFailed());
  }
}

export function* watchAppSaga() {
  yield takeLatest(initRequest, initAppSaga);
}
