import { useEffect } from "react";
import { useAuthStore } from "../store/useAuthStore";

export function useAuthInit() {
  const { checkSession } = useAuthStore();

  useEffect(() => {
    checkSession();
  }, [checkSession]);
}
