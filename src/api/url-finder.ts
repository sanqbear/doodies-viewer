import axios from 'axios';

async function findUrl(url: string) {
  const response = await fetchUrlWithTimeout(url, 10000);
  if (response.status === 302) {
    let loc = response.headers.location;
    if (loc?.includes('manatoki')) {
      const response2 = await axios.get(loc);
      if (response2.status === 200) {
        return loc;
      }
    }
  } else if (response.status === 200) {
    if (response.headers.server === 'cloudflare') {
      return url;
    }
  }
  return null;
}

const fetchUrlWithTimeout = async (url: string, timeout: number) => {
  const response = await axios.get(url, {
    timeout: timeout,
    maxRedirects: 0,
  });
  return response;
};

export default findUrl;
