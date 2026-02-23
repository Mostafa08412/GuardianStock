import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { ImportProvider } from "@/contexts/ImportContext";
import { AuthProvider } from "@/contexts/AuthContext";
import { GoogleOAuthProvider } from "@react-oauth/google";
import { RoleProvider } from "@/contexts/RoleContext";
import { ProtectedRoute } from "@/components/auth/ProtectedRoute";
import GlobalImportStatus from "@/components/import/GlobalImportStatus";
import Index from "./pages/Index";
import Auth from "./pages/Auth";
import NotFound from "./pages/NotFound";
import Dashboard from "./pages/Dashboard";
import Products from "./pages/Products";
import CategoryDetails from "./pages/CategoryDetails";
import Categories from "./pages/Categories";
import Transactions from "./pages/Transactions";
import TransactionDetails from "./pages/TransactionDetails";
import Alerts from "./pages/Alerts";
import AlertDetails from "./pages/AlertDetails";
import Inventories from "./pages/Inventories";
import InventoryDetails from "./pages/InventoryDetails";
import Users from "./pages/Users";
import Profile from "./pages/Profile";
import ProductDetails from "./pages/ProductDetails";
import SearchResults from "./pages/SearchResults";
import MainLayout from "./components/layout/MainLayout";

import { GOOGLE_CLIENT_ID } from "@/config/runtimeConfig";

const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <GoogleOAuthProvider clientId={GOOGLE_CLIENT_ID}>
        <AuthProvider>
          <RoleProvider>
            <ImportProvider>
              <Toaster />
              <Sonner />
              <GlobalImportStatus />
              <BrowserRouter>
                <Routes>
                  {/* Public Routes */}
                  <Route path="/auth" element={<Auth />} />

                  {/* Protected Routes with MainLayout */}
                  <Route
                    element={
                      <ProtectedRoute>
                        <MainLayout />
                      </ProtectedRoute>
                    }
                  >
                    <Route path="/" element={<Index />} />
                    <Route path="/dashboard" element={<Dashboard />} />
                    <Route path="/products" element={<Products />} />
                    <Route path="/products/:id" element={<ProductDetails />} />
                    <Route path="/categories" element={<Categories />} />
                    <Route path="/categories/:id" element={<CategoryDetails />} />
                    <Route path="/transactions" element={<Transactions />} />
                    <Route path="/transactions/:id" element={<TransactionDetails />} />
                    <Route path="/alerts" element={<Alerts />} />
                    <Route path="/alerts/:id" element={<AlertDetails />} />
                    <Route path="/inventories" element={<Inventories />} />
                    <Route path="/inventories/:id" element={<InventoryDetails />} />
                    <Route path="/users" element={<Users />} />
                    <Route path="/profile" element={<Profile />} />
                    <Route path="/search" element={<SearchResults />} />
                  </Route>

                  <Route path="*" element={<NotFound />} />
                </Routes>
              </BrowserRouter>
            </ImportProvider>
          </RoleProvider>
        </AuthProvider>
      </GoogleOAuthProvider>
    </TooltipProvider>
  </QueryClientProvider>
);

export default App;
