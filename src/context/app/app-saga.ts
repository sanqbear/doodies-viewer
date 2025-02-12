import {call, put, takeLatest} from 'redux-saga/effects';
import {
  setApiUrl,
  setIsUrlGreen,
  setLanguage,
  setTheme,
  tryChangeApiUrl,
} from './app-reducer';
import findUrl from '../../api/url-finder';

async function* tryChangeApiUrlSaga(_: ReturnType<typeof tryChangeApiUrl>) {
  try {
    const result: string | null = await findUrl();
    if (result) {
      yield call(setApiUrl, result);
    }
  } catch (error) {
    console.error(error);
  }
}

export function* watchAppSaga() {
  yield takeLatest(tryChangeApiUrl, tryChangeApiUrlSaga);
}
