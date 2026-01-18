import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Search, ChevronLeft, ChevronRight, Edit2, Save, X, ArrowUpDown, ArrowUp, ArrowDown } from 'lucide-react';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { useAppSelector } from '../../hooks/redux';
import * as userApi from '../../services/api/userApi';

const UserManagement = () => {
  const navigate = useNavigate();
  const { user: currentUser, isAuthenticated } = useAppSelector((state) => state.auth);
  
  // Check if user is Super Admin
  const isSuperAdmin = currentUser?.roles?.includes('Super Admin');

  // State for pagination
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  // State for filtering
  const [filters, setFilters] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    isActive: null,
    roleName: '',
    isLocked: null,
  });

  // State for sorting
  const [sortBy, setSortBy] = useState(null);
  const [sortDescending, setSortDescending] = useState(false);

  // State for data
  const [users, setUsers] = useState([]);
  const [metadata, setMetadata] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  // Fetch users
  const fetchUsers = async () => {
    setIsLoading(true);
    setError(null);
    
    try {
      const params = {
        pageNumber,
        pageSize,
        ...filters,
        sortBy,
        sortDescending,
      };

      // Remove null/empty values
      Object.keys(params).forEach(key => {
        if (params[key] === null || params[key] === '' || params[key] === undefined) {
          delete params[key];
        }
      });

      const response = await userApi.getUsers(params);
      setUsers(response.items || []);
      setMetadata(response.metadata || {});
    } catch (err) {
      setError(err.message || 'Failed to load users');
      console.error('Error fetching users:', err);
    } finally {
      setIsLoading(false);
    }
  };

  // Check Super Admin after auth is loaded
  useEffect(() => {
    if (isAuthenticated && !isSuperAdmin) {
      navigate('/dashboard');
    }
  }, [isAuthenticated, isSuperAdmin, navigate]);

  // Fetch users when component mounts or when pagination/sorting changes (only if Super Admin)
  useEffect(() => {
    if (isAuthenticated && isSuperAdmin) {
      fetchUsers();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isAuthenticated, isSuperAdmin, pageNumber, pageSize, sortBy, sortDescending]);

  // Fetch users when filters change (with debounce for typing)
  useEffect(() => {
    if (!isAuthenticated || !isSuperAdmin) return;
    
    const timer = setTimeout(() => {
      fetchUsers();
    }, 500); // Small delay for typing

    return () => clearTimeout(timer);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filters, isAuthenticated, isSuperAdmin]);

  // Handle filter change
  const handleFilterChange = (key, value) => {
    setFilters(prev => ({ ...prev, [key]: value }));
    setPageNumber(1); // Reset to first page on filter change
  };

  // Handle sort
  const handleSort = (field) => {
    if (sortBy === field) {
      setSortDescending(!sortDescending);
    } else {
      setSortBy(field);
      setSortDescending(false);
    }
    setPageNumber(1);
  };

  // Get sort icon for a column
  const getSortIcon = (field) => {
    if (sortBy !== field) {
      return <ArrowUpDown className="h-3 w-3 text-gray-400" />;
    }
    return sortDescending 
      ? <ArrowDown className="h-3 w-3 text-primary-600" />
      : <ArrowUp className="h-3 w-3 text-primary-600" />;
  };

  // Check if column is currently sorted
  const isSorted = (field) => sortBy === field;

  // Clear filters
  const clearFilters = () => {
    setFilters({
      firstName: '',
      lastName: '',
      email: '',
      phoneNumber: '',
      isActive: null,
      roleName: '',
      isLocked: null,
    });
    setSortBy(null);
    setSortDescending(false);
    setPageNumber(1);
  };

  if (!isAuthenticated) {
    return <div className="min-h-screen flex items-center justify-center">Loading...</div>;
  }

  if (!isSuperAdmin) {
    return null; // Will redirect in useEffect
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-white">
      {/* Header */}
      <div className="bg-white shadow-sm border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <div className="flex justify-between items-center">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">User Management</h1>
              <p className="mt-1 text-sm text-gray-600">Manage users, roles, and permissions</p>
            </div>
            <Button onClick={() => navigate('/dashboard')} variant="outline">
              Back to Dashboard
            </Button>
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Filters */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-6">
          <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-4">
            <Input
              placeholder="First Name"
              value={filters.firstName}
              onChange={(e) => handleFilterChange('firstName', e.target.value)}
              className="w-full"
            />
            <Input
              placeholder="Last Name"
              value={filters.lastName}
              onChange={(e) => handleFilterChange('lastName', e.target.value)}
              className="w-full"
            />
            <Input
              placeholder="Email"
              value={filters.email}
              onChange={(e) => handleFilterChange('email', e.target.value)}
              className="w-full"
            />
            <Input
              placeholder="Phone Number"
              value={filters.phoneNumber}
              onChange={(e) => handleFilterChange('phoneNumber', e.target.value)}
              className="w-full"
            />
            <select
              value={filters.isActive === null ? '' : filters.isActive.toString()}
              onChange={(e) => handleFilterChange('isActive', e.target.value === '' ? null : e.target.value === 'true')}
              className="h-10 rounded-md border border-input bg-background px-3 py-2 text-sm"
            >
              <option value="">All Status</option>
              <option value="true">Active</option>
              <option value="false">Inactive</option>
            </select>
            <Input
              placeholder="Role Name"
              value={filters.roleName}
              onChange={(e) => handleFilterChange('roleName', e.target.value)}
              className="w-full"
            />
            <select
              value={filters.isLocked === null ? '' : filters.isLocked.toString()}
              onChange={(e) => handleFilterChange('isLocked', e.target.value === '' ? null : e.target.value === 'true')}
              className="h-10 rounded-md border border-input bg-background px-3 py-2 text-sm"
            >
              <option value="">All Lock Status</option>
              <option value="true">Locked</option>
              <option value="false">Unlocked</option>
            </select>
            <div className="flex gap-2">
              <Button onClick={fetchUsers} variant="outline">
                <Search className="h-4 w-4 mr-2" />
                Search
              </Button>
              <Button onClick={clearFilters} variant="ghost">
                Clear
              </Button>
            </div>
          </div>
        </div>

        {/* Error Message */}
        {error && (
          <div className="mb-6 p-4 bg-destructive/10 border border-destructive/20 rounded-lg">
            <p className="text-sm font-medium text-destructive">{error}</p>
          </div>
        )}

        {/* Users Table */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th
                    className={`px-6 py-3 text-left text-xs font-medium uppercase tracking-wider cursor-pointer hover:bg-gray-100 transition-colors ${
                      isSorted('FirstName') 
                        ? 'bg-primary-50 text-primary-700' 
                        : 'text-gray-500'
                    }`}
                    onClick={() => handleSort('FirstName')}
                  >
                    <div className="flex items-center space-x-1">
                      <span>First Name</span>
                      {getSortIcon('FirstName')}
                    </div>
                  </th>
                  <th
                    className={`px-6 py-3 text-left text-xs font-medium uppercase tracking-wider cursor-pointer hover:bg-gray-100 transition-colors ${
                      isSorted('LastName') 
                        ? 'bg-primary-50 text-primary-700' 
                        : 'text-gray-500'
                    }`}
                    onClick={() => handleSort('LastName')}
                  >
                    <div className="flex items-center space-x-1">
                      <span>Last Name</span>
                      {getSortIcon('LastName')}
                    </div>
                  </th>
                  <th
                    className={`px-6 py-3 text-left text-xs font-medium uppercase tracking-wider cursor-pointer hover:bg-gray-100 transition-colors ${
                      isSorted('Email') 
                        ? 'bg-primary-50 text-primary-700' 
                        : 'text-gray-500'
                    }`}
                    onClick={() => handleSort('Email')}
                  >
                    <div className="flex items-center space-x-1">
                      <span>Email</span>
                      {getSortIcon('Email')}
                    </div>
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Phone
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Roles
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Status
                  </th>
                  <th
                    className={`px-6 py-3 text-left text-xs font-medium uppercase tracking-wider cursor-pointer hover:bg-gray-100 transition-colors ${
                      isSorted('CreatedAt') 
                        ? 'bg-primary-50 text-primary-700' 
                        : 'text-gray-500'
                    }`}
                    onClick={() => handleSort('CreatedAt')}
                  >
                    <div className="flex items-center space-x-1">
                      <span>Created</span>
                      {getSortIcon('CreatedAt')}
                    </div>
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {isLoading ? (
                  <tr>
                    <td colSpan="8" className="px-6 py-8 text-center text-gray-500">
                      Loading users...
                    </td>
                  </tr>
                ) : users.length === 0 ? (
                  <tr>
                    <td colSpan="8" className="px-6 py-8 text-center text-gray-500">
                      No users found
                    </td>
                  </tr>
                ) : (
                  users.map((user) => (
                    <tr key={user.id} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                        {user.firstName}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                        {user.lastName}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                        {user.email}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                        {user.phoneNumber || '-'}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                        <div className="flex flex-wrap gap-1">
                          {user.roles?.map((role) => (
                            <span
                              key={role}
                              className="px-2 py-1 text-xs font-medium rounded-full bg-primary-100 text-primary-700"
                            >
                              {role}
                            </span>
                          ))}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm">
                        <span
                          className={`px-2 py-1 text-xs font-medium rounded-full ${
                            user.isActive
                              ? 'bg-green-100 text-green-700'
                              : 'bg-red-100 text-red-700'
                          }`}
                        >
                          {user.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                        {new Date(user.createdAt).toLocaleDateString()}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm">
                        <Button variant="ghost" size="sm">
                          <Edit2 className="h-4 w-4" />
                        </Button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          {metadata && (
            <div className="bg-gray-50 px-6 py-4 border-t border-gray-200 flex items-center justify-between">
              <div className="text-sm text-gray-700">
                Showing {((pageNumber - 1) * pageSize) + 1} to {Math.min(pageNumber * pageSize, metadata.totalCount)} of{' '}
                {metadata.totalCount} users
              </div>
              <div className="flex items-center space-x-2">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setPageNumber(pageNumber - 1)}
                  disabled={!metadata.hasPrevious || isLoading}
                >
                  <ChevronLeft className="h-4 w-4 mr-1" />
                  Previous
                </Button>
                <div className="text-sm text-gray-700">
                  Page {metadata.pageNumber} of {metadata.totalPages}
                </div>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setPageNumber(pageNumber + 1)}
                  disabled={!metadata.hasNext || isLoading}
                >
                  Next
                  <ChevronRight className="h-4 w-4 ml-1" />
                </Button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default UserManagement;
