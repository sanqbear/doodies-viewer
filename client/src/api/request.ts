import axios from 'axios';
import {fetch} from 'react-native-ssl-pinning';

const instance = axios.create({
  adapter: async config => {
    try {
      const response = await fetch(config.url!, {
        method: (config.method || 'GET') as 'GET' | 'POST' | 'PUT' | 'DELETE',
        headers: config.headers,
        body: config.data,
        sslPinning: {
          certs: ['manatoki468.net'],
        },
      });

      return {
        data: response.data,
        status: response.status,
        statusText: response.status.toString(),
        headers: response.headers,
        config,
        request: null,
      };
    } catch (error) {
      return Promise.reject(error);
    }
  },
});

export default async function request(url: string) {
  console.info(url);
  const ip = await resolveDoH(url);
  console.info(ip);
  if (!ip) {
    return null;
  }

  try {
    const hostname = url.split('/')[2];
    const targetUrl = url.replace(hostname, ip);
    console.info(url);
    console.info(targetUrl);
    const response = await instance.get(targetUrl, {
      headers: {
        Host: hostname,
        'User-Agent':
          'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36',
      },
    });

    if (response.status === 200) {
      return response.data;
    }
  } catch (error) {
    console.error(error);
  }
  return null;
}

const resolveDoH = async (url: string) => {
  const hostname = url.split('/')[2];
  console.info(hostname);
  const response = await axios.get(
    `https://cloudflare-dns.com/dns-query?name=${hostname}&type=A`,
    {
      headers: {
        Accept: 'application/dns-json',
      },
    },
  );
  const data = await response.data;
  return data.Answer && data.Answer.length > 0 ? data.Answer[0].data : null;
};
