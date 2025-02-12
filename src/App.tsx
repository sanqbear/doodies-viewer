import React, {useEffect, useState} from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import AppSetting from './types/app-setting';
import {createNativeStackNavigator} from '@react-navigation/native-stack';
import HomeScreen from './screen/HomeScreen';
import LookupScreen from './screen/LookupScreen';
import {
  createStaticNavigation,
  NavigationContainer,
} from '@react-navigation/native';

const App = () => {
  const [appSetting, setAppSetting] = useState<AppSetting>({
    url: '',
    isGreen: false,
    theme: 'light',
  });
  const loadAppSetting = async () => {
    const appSettingItem = await AsyncStorage.getItem('appSetting');
    if (appSettingItem) {
      setAppSetting(JSON.parse(appSettingItem));
    }
  };

  useEffect(() => {
    loadAppSetting();
  }, []);

  const stack = createNativeStackNavigator({
    screens: {
      Home: {
        if: () => appSetting.isGreen,
        screen: HomeScreen,
      },
      Lookup: {
        if: () => !appSetting.isGreen,
        screen: LookupScreen,
      },
    },
  });

  const navigation = createStaticNavigation(stack);

  return <NavigationContainer>{navigation}</NavigationContainer>;
};

export default App;
