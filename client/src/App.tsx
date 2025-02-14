import React, {useEffect} from 'react';
import {createNativeStackNavigator} from '@react-navigation/native-stack';
import HomeScreen from './screen/HomeScreen';
import LookupScreen from './screen/LookupScreen';
import {NavigationContainer} from '@react-navigation/native';
import {useAppDispatch, useAppSelector} from './hook';
import {loadAppState} from './context/app/app-reducer';
import AsyncStorage from '@react-native-async-storage/async-storage';

const App = () => {
  const appSetting = useAppSelector(state => state.app);
  const dispatch = useAppDispatch();

  useEffect(() => {
    if(!appSetting.isGreen) {
      AsyncStorage.getItem('DVIEW:APPSTATE').then(appState => {
        if (appState) {
          dispatch(loadAppState(JSON.parse(appState)));
        }
      });
    }
  }, [dispatch, appSetting.isGreen]);

  const Stack = createNativeStackNavigator();

  return (
    <NavigationContainer>
      <Stack.Navigator screenOptions={{headerShown: false}}>
        {appSetting.isGreen ? (
          <Stack.Screen name="Home" component={HomeScreen} />
        ) : (
          <Stack.Screen name="Lookup" component={LookupScreen} />
        )}
      </Stack.Navigator>
    </NavigationContainer>
  );
};

export default App;
