import { create } from "zustand";
import { persist } from "zustand/middleware";
import { api } from "../api/AuthApi";

export const useAuthStore = create(
  persist(
    (set) => ({
      user: null,
      isLoading: false,
      error: null,

      setUser: (user) => set({ user }),

      // ✅ Login — guarda el usuario al iniciar sesión
      login: async (username, password) => {
        set({ isLoading: true, error: null });
        try {
          const res = await api.post("/auth/login", { username, password }, { withCredentials: true });
          set({ user: res.data });
          return res.data;
        } catch (err) {
          set({
            error: err.response?.data?.message || "Error de autenticación",
          });
          return null;
        } finally {
          set({ isLoading: false });
        }
      },

      // ✅ Verifica si hay sesión activa en backend (cookie HttpOnly)
      checkSession: async () => {
        try {
          const res = await api.get("/auth/usuario", { withCredentials: true });
          set({ user: res.data });
        } catch {
          set({ user: null });
        }
      },

      logout: async () => {
        await api.post("/auth/logout", {}, { withCredentials: true });
        set({ user: null });
      },
    }),
    {
      name: "auth-storage", // 🔒 Persistencia local
    }
  )
);
