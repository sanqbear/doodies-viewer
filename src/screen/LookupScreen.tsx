import {
  cancelApp,
  initComplete,
  resetApp,
  tryInitRequest,
} from '@/context/app/app-reducer';
import {useAppDispatch, useAppSelector} from '@/hook';
import AsyncStorage from '@react-native-async-storage/async-storage';
import React, {useEffect, useState} from 'react';
import {Modal, Pressable, StyleSheet, Text, View} from 'react-native';
import {SafeAreaProvider, SafeAreaView} from 'react-native-safe-area-context';

const LookupScreen = () => {
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [isInitStarted, setIsInitStarted] = useState(false);

  const dispatch = useAppDispatch();
  const appState = useAppSelector(state => state.app);

  const saveToStorage = () => {
    AsyncStorage.setItem('DVIEW:APPSTATE', JSON.stringify(appState));
  };

  const handleTryBindUrl = () => {
    setIsModalVisible(true);
    setIsInitStarted(true);
  };

  const handleCancel = () => {
    dispatch(cancelApp());
    setIsModalVisible(false);
    setIsInitStarted(false);
  };

  const handleOk = () => {
    setIsModalVisible(false);
    saveToStorage();
    dispatch(initComplete());
  };

  const handleReset = () => {
    dispatch(resetApp());
    setIsModalVisible(false);
    setIsInitStarted(false);
  };

  useEffect(() => {
    if (isInitStarted && !appState.isCanceled) {
      dispatch(tryInitRequest({idx: appState.lastTriedIndex}));
    }
  }, [isInitStarted, appState.isCanceled, appState.lastTriedIndex, dispatch]);

  useEffect(() => {
    if (appState.url) {
      setIsInitStarted(false);
    }
  }, [appState.url]);

  return (
    <SafeAreaProvider>
      <SafeAreaView style={styles.screen}>
        <Modal animationType="fade" visible={isModalVisible} transparent={true}>
          <View style={styles.screen}>
            <View style={styles.modalView}>
              {appState.url ? (
                <>
                  <View>
                    <Text style={styles.instruction}>
                      We found {appState.url}!
                    </Text>
                  </View>
                  <View style={styles.buttonContainer}>
                    <Pressable style={styles.button} onPress={() => handleOk()}>
                      <Text style={styles.buttonText}>Okay</Text>
                    </Pressable>
                    <Pressable
                      style={styles.buttonAlt}
                      onPress={() => handleReset()}>
                      <Text style={styles.buttonTextAlt}>Reset</Text>
                    </Pressable>
                  </View>
                </>
              ) : (
                <>
                  <View>
                    <Text style={styles.instruction}>
                      Try Check {appState.lastTriedIndex}...
                    </Text>
                  </View>
                  <Pressable
                    style={styles.buttonAlt}
                    onPress={() => handleCancel()}>
                    <Text style={styles.buttonTextAlt}>Cancel</Text>
                  </Pressable>
                </>
              )}
            </View>
          </View>
        </Modal>
        <View>
          <Text style={styles.instruction}>We couldn't find the URL</Text>
        </View>
        <Pressable style={styles.button} onPress={() => handleTryBindUrl()}>
          <Text style={styles.buttonText}>Try Bind URL</Text>
        </Pressable>
      </SafeAreaView>
    </SafeAreaProvider>
  );
};

const styles = StyleSheet.create({
  screen: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  modalView: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    minWidth: 400,
    maxHeight: 200,
    backgroundColor: '#fff',
    borderRadius: 10,
    borderWidth: 1,
    borderColor: '#000',
  },
  instruction: {
    marginBottom: 20,
    fontSize: 20,
    fontWeight: 'bold',
    color: '#000',
  },
  buttonContainer: {
    flexDirection: 'row',
    width: '100%',
    gap: 30,
  },
  button: {
    backgroundColor: '#000',
    padding: 10,
    borderRadius: 5,
  },
  buttonAlt: {
    backgroundColor: '#fff',
    padding: 10,
    borderRadius: 5,
    borderWidth: 1,
    borderColor: '#000',
  },
  buttonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: 'bold',
  },
  buttonTextAlt: {
    color: '#000',
    fontSize: 16,
    fontWeight: 'bold',
  },
});

export default LookupScreen;
