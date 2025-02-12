import React, {useState} from 'react';
import {View, Text, Button} from 'react-native';
import findUrl from './api/url-finder';

const App = () => {
  const [apiUrl, setApiUrl] = useState<string | null>(null);

  const getApiUrl = async () => {
    const result = await findUrl();
    setApiUrl(result);
  };

  if (!apiUrl) {
    return (
      <View>
        <Text>URL not found. Please click the button below to get URL</Text>
        <Button title="Get URL" onPress={getApiUrl} />
      </View>
    );
  }
  return (
    <View>
      <Text>Hello World</Text>
      <Text>{apiUrl}</Text>
      <Button title="Reset" onPress={() => setApiUrl(null)} />
    </View>
  );
};

export default App;
