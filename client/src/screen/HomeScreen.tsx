import {getHomePage} from '@/api/home-api';
import {useAppSelector} from '@/hook';
import React, {useEffect} from 'react';
import {Text, View} from 'react-native';

const HomeScreen = () => {
  const url = useAppSelector(state => state.app.url);

  useEffect(() => {
    getHomePage(url).then(data => {
      console.log(data);
    });
  }, [url]);

  return (
    <View>
      <Text>Home</Text>
    </View>
  );
};

export default HomeScreen;
