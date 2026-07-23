import axios from "axios";


const api = axios.create({
  baseURL:
    import.meta.env.VITE_API_URL ||
    import.meta.env.VITE_API_BASE_URL ||
    "https://talentai-hch6gphugdetdpga.eastasia-01.azurewebsites.net/api",
});


api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token") || localStorage.getItem("talentai_token");

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },

  (error) => {
    return Promise.reject(error);
  }
);


export default api;
