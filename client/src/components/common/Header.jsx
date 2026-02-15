import { useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { ShoppingBag, ShoppingCart, User, Settings, LogOut, FolderTree, Package, Search } from 'lucide-react';
import { Button } from '../ui/button';
import { useAppDispatch, useAppSelector } from '../../hooks/redux';
import { logout } from '../../store/slices/authSlice';
import { fetchBasketCount, resetBasket } from '../../store/slices/basketSlice';

const Header = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { user, isAuthenticated } = useAppSelector((state) => state.auth);
  const { itemCount } = useAppSelector((state) => state.basket);
  const isSuperAdmin = user?.roles?.includes('Super Admin');

  useEffect(() => {
    if (isAuthenticated) {
      dispatch(fetchBasketCount());
    }
  }, [isAuthenticated, dispatch]);

  const handleLogout = () => {
    dispatch(logout());
    dispatch(resetBasket());
    navigate('/');
  };

  return (
    <nav className="fixed top-0 w-full bg-white/80 backdrop-blur-md border-b border-gray-200 z-50 shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          <Link to="/" className="flex items-center space-x-2">
            <ShoppingBag className="h-8 w-8 text-primary-600" />
            <span className="text-2xl font-bold text-gray-900">ECommerce</span>
          </Link>
          <div className="flex items-center space-x-4">
            {/* Catalog Link (visible to all users) */}
            <Link to="/catalog">
              <Button variant="ghost" className="hidden sm:inline-flex">
                <Search className="h-4 w-4 mr-2" />
                Catalog
              </Button>
            </Link>

            {isAuthenticated && user ? (
              <>
                {/* User Management Link (Super Admin only) */}
                {isSuperAdmin && (
                  <Link to="/admin/users">
                    <Button variant="ghost" className="hidden sm:inline-flex">
                      <Settings className="h-4 w-4 mr-2" />
                      User Management
                    </Button>
                  </Link>
                )}

                {/* Category Management Link (Super Admin only) */}
                {isSuperAdmin && (
                  <Link to="/admin/categories">
                    <Button variant="ghost" className="hidden sm:inline-flex">
                      <FolderTree className="h-4 w-4 mr-2" />
                      Categories
                    </Button>
                  </Link>
                )}

                {/* Products Link */}
                <Link to="/products">
                  <Button variant="ghost" className="hidden sm:inline-flex">
                    <Package className="h-4 w-4 mr-2" />
                    Products
                  </Button>
                </Link>

                {/* Basket Link */}
                <Link to="/basket" className="relative">
                  <Button variant="ghost" size="icon" className="relative">
                    <ShoppingCart className="h-5 w-5" />
                    {itemCount > 0 && (
                      <span className="absolute -top-1 -right-1 inline-flex items-center justify-center h-5 w-5 rounded-full bg-primary-600 text-white text-xs font-bold">
                        {itemCount > 99 ? '99+' : itemCount}
                      </span>
                    )}
                  </Button>
                </Link>

                {/* Dashboard Link */}
                <Link to="/dashboard">
                  <Button variant="ghost" className="hidden sm:inline-flex">
                    <User className="h-4 w-4 mr-2" />
                    Dashboard
                  </Button>
                </Link>

                {/* User Info & Logout */}
                <div className="flex items-center space-x-3">
                  <div className="hidden sm:flex flex-col text-right">
                    <span className="text-sm font-medium text-gray-900">
                      {user.firstName} {user.lastName}
                    </span>
                    <span className="text-xs text-gray-500">{user.email}</span>
                  </div>
                  <Button
                    variant="outline"
                    onClick={handleLogout}
                    className="hidden sm:inline-flex"
                  >
                    <LogOut className="h-4 w-4 mr-2" />
                    Logout
                  </Button>
                </div>
              </>
            ) : (
              <>
                <Link to="/login">
                  <Button variant="ghost" className="hidden sm:inline-flex">
                    Sign In
                  </Button>
                </Link>
                <Link to="/register">
                  <Button className="bg-primary-600 hover:bg-primary-700">
                    Get Started
                  </Button>
                </Link>
              </>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Header;
