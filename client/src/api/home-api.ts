import request from './request';

export const getHomePage = async (url: string) => {
  const data = await request(url);
  console.info(data);
};
