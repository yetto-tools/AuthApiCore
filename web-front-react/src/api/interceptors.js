import axios from "axios";
import { api } from "./AuthApi";

// Endpoint de refresh
const refreshApi = axios.create({
  baseURL: "https://localhost:44360/api",
  withCredentials: true,
});

let isRefreshing = false;
let refreshSubscribers = [];

// Reintenta todas las peticiones en espera cuando se obtiene un nuevo token
const onRefreshed = (newToken) => {
  refreshSubscribers.forEach((callback) => callback(newToken));
  refreshSubscribers = [];
};

// Suscribir peticiones en cola
const addSubscriber = (callback) => {
  refreshSubscribers.push(callback);
};

// Interceptor de respuesta
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Solo actúa en 401
    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        // Espera al refresh si ya está en curso
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
        // Llama al endpoint de refresh
        const { data } = await refreshApi.post("/auth/refresh");
        const newToken = data.accessToken;

        // Guarda el token nuevo (si lo manejas manualmente)
        localStorage.setItem("access_token", newToken);

        // Actualiza headers globales
        api.defaults.headers.Authorization = `Bearer ${newToken}`;
        onRefreshed(newToken);

        // Reintenta la petición original
        return api(originalRequest);
      } catch (refreshError) {
        console.error("Error al refrescar token:", refreshError);
        window.location.href = "/login"; // o logout
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);

// Interceptor de solicitud
// api.interceptors.request.use(
//   (config) => {
//     const token = localStorage.getItem("access_token");
//     if (token) {
//       config.headers.Authorization = `Bearer ${token}`;
//     }
//     return config;
//   },
//   (error) => Promise.reject(error)
// );