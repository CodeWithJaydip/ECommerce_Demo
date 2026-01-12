import { useAppDispatch, useAppSelector } from '../../hooks/redux';
import { logout } from '../../store/slices/authSlice';
import { useNavigate } from 'react-router-dom';

const Dashboard = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { user } = useAppSelector((state) => state.auth);

  const handleLogout = () => {
    dispatch(logout());
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-primary-50 to-primary-100">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center">
              <h1 className="text-xl font-bold text-gray-900">ECommerce</h1>
            </div>
            <div className="flex items-center space-x-4">
              <span className="text-gray-700">
                Welcome, {user?.firstName} {user?.lastName}
              </span>
              <button
                onClick={handleLogout}
                className="px-4 py-2 text-sm font-medium text-white bg-primary-600 rounded-lg hover:bg-primary-700 transition-colors"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="bg-white rounded-2xl shadow-xl p-8">
          <h2 className="text-2xl font-bold text-gray-900 mb-6">Dashboard</h2>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="bg-primary-50 rounded-lg p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-2">User Information</h3>
              <div className="space-y-2 text-gray-700">
                <p><span className="font-medium">Name:</span> {user?.firstName} {user?.lastName}</p>
                <p><span className="font-medium">Email:</span> {user?.email}</p>
                {user?.phoneNumber && (
                  <p><span className="font-medium">Phone:</span> {user?.phoneNumber}</p>
                )}
                <p><span className="font-medium">Roles:</span> {user?.roles?.join(', ') || 'User'}</p>
              </div>
            </div>

            <div className="bg-gray-50 rounded-lg p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-2">Quick Actions</h3>
              <div className="space-y-3">
                <button className="w-full text-left px-4 py-2 bg-white rounded-lg hover:bg-gray-100 transition-colors">
                  View Profile
                </button>
                <button className="w-full text-left px-4 py-2 bg-white rounded-lg hover:bg-gray-100 transition-colors">
                  Settings
                </button>
                <button className="w-full text-left px-4 py-2 bg-white rounded-lg hover:bg-gray-100 transition-colors">
                  Orders
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
