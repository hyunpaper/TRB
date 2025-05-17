import { BrowserRouter, Routes, Route } from "react-router-dom";
import DashboardPage from "./adapters.presentation/pages/dashboard";
import { AuthProvider } from "./adapters.presentation/hooks/AuthContext";

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<DashboardPage />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
