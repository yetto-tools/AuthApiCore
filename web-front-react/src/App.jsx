import { RouterProvider } from "react-router-dom";
import { router } from "./routes/router";
import { useAuthInit } from "./hooks/useAuthInit";

export default function App() {
  useAuthInit();
  return <RouterProvider router={router} />;
}
