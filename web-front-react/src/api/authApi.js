import axios from "axios";

// Cambia por la URL de tu backend
export const api = axios.create({
  baseURL: "https://localhost:44360/api/",
  withCredentials: true, // ⚡ permite enviar cookies HttpOnly
  headers: {
    "Content-Type": "application/json",
  },
});

// 🔹 Instancia separada solo para refresh
const refreshApi = axios.create({
  baseURL: "https://localhost:44360/api",
  withCredentials: true,
});

let isRefreshing = false;
let refreshSubscribers = [];

const onRefreshed = (token) => {
  refreshSubscribers.forEach((callback) => callback(token));
  refreshSubscribers = [];
};

const addSubscriber = (callback) => {
  refreshSubscribers.push(callback);
};

// 🔹 Interceptor principal
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve) => {
          addSubscriber((token) => {
            originalRequest.headers.Authorization = `Bearer ${token}`;
            resolve(api(originalRequest));
          });
        });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        const { data } = await refreshApi.post("/auth/refresh");
        const newToken = data.accessToken;

        // Guarda y aplica el nuevo token
        localStorage.setItem("access_token", newToken);
        api.defaults.headers.Authorization = `Bearer ${newToken}`;
        onRefreshed(newToken);

        return api(originalRequest);
      } catch (refreshError) {
        console.error("Error al refrescar token:", refreshError);
        window.location.href = "/login";
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);
