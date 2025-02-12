import App from '@/App';
import React from 'react';
import {Provider} from 'react-redux';
import store from '@/store/store';

const AppContainer = () => {
  return (
    <Provider store={store}>
      <App />
    </Provider>
  );
};

export default AppContainer;
