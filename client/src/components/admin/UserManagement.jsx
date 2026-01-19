import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Search, ChevronLeft, ChevronRight, Edit2, Save, X, ArrowUpDown, ArrowUp, ArrowDown, Trash2 } from 'lucide-react';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '../ui/alert-dialog';
import { Card, CardContent } from '../ui/card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../ui/select';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '../ui/table';
import { useAppSelector } from '../../hooks/redux';
import * as userApi from '../../services/api/userApi';
import Header from '../common/Header';

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

  // State for editing
  const [editingUserId, setEditingUserId] = useState(null);
  const [editFormData, setEditFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    isActive: true,
    roleIds: [], // Array of role IDs
  });

  // State for delete dialog
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [userToDelete, setUserToDelete] = useState(null);

  // Available roles mapping (based on seed data)
  const availableRoles = [
    { id: 2, name: 'Super Admin' },
    { id: 3, name: 'Seller' },
    { id: 4, name: 'Buyer' },
  ];

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

  // Handle edit click
  const handleEditClick = (user) => {
    setEditingUserId(user.id);
    // Map role names to role IDs
    const userRoleIds = user.roles
      ? user.roles
          .map(roleName => availableRoles.find(r => r.name === roleName)?.id)
          .filter(id => id !== undefined)
      : [];
    
    setEditFormData({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      phoneNumber: user.phoneNumber || '',
      isActive: user.isActive,
      roleIds: userRoleIds,
    });
  };

  // Handle save edit
  const handleSaveEdit = async () => {
    if (!editingUserId) return;

    setIsLoading(true);
    setError(null);

    try {
      // Update user info (exclude roleIds from this call)
      const { roleIds, ...userData } = editFormData;
      await userApi.updateUser(editingUserId, userData);
      
      // Update user roles separately (always call, even if empty array to remove all roles)
      if (roleIds !== undefined) {
        await userApi.updateUserRoles(editingUserId, { roleIds: roleIds || [] });
      }
      
      setEditingUserId(null);
      // Refresh users list
      await fetchUsers();
    } catch (err) {
      setError(err.message || 'Failed to update user');
      console.error('Error updating user:', err);
    } finally {
      setIsLoading(false);
    }
  };

  // Handle cancel edit
  const handleCancelEdit = () => {
    setEditingUserId(null);
    setEditFormData({
      firstName: '',
      lastName: '',
      email: '',
      phoneNumber: '',
      isActive: true,
      roleIds: [],
    });
  };

  // Handle delete click - open dialog
  const handleDeleteClick = (userId) => {
    setUserToDelete(userId);
    setDeleteDialogOpen(true);
  };

  // Handle confirm delete (soft delete - set IsActive to false)
  const handleConfirmDelete = async () => {
    if (!userToDelete) return;

    setIsLoading(true);
    setError(null);

    try {
      await userApi.deleteUser(userToDelete);
      setDeleteDialogOpen(false);
      setUserToDelete(null);
      // Refresh users list
      await fetchUsers();
    } catch (err) {
      setError(err.message || 'Failed to delete user');
      console.error('Error deleting user:', err);
    } finally {
      setIsLoading(false);
    }
  };

  if (!isAuthenticated) {
    return <div className="min-h-screen flex items-center justify-center">Loading...</div>;
  }

  if (!isSuperAdmin) {
    return null; // Will redirect in useEffect
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-white">
      <Header />
      
      {/* Page Header */}
      <div className="bg-white shadow-sm border-b border-gray-200 pt-16">
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
        <Card className="mb-6">
          <CardContent className="pt-6">
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
              <Select
                value={filters.isActive === null ? 'all' : filters.isActive.toString()}
                onValueChange={(value) => handleFilterChange('isActive', value === 'all' ? null : value === 'true')}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="All Status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Status</SelectItem>
                  <SelectItem value="true">Active</SelectItem>
                  <SelectItem value="false">Inactive</SelectItem>
                </SelectContent>
              </Select>
              <Input
                placeholder="Role Name"
                value={filters.roleName}
                onChange={(e) => handleFilterChange('roleName', e.target.value)}
                className="w-full"
              />
              <Select
                value={filters.isLocked === null ? 'all' : filters.isLocked.toString()}
                onValueChange={(value) => handleFilterChange('isLocked', value === 'all' ? null : value === 'true')}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="All Lock Status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Lock Status</SelectItem>
                  <SelectItem value="true">Locked</SelectItem>
                  <SelectItem value="false">Unlocked</SelectItem>
                </SelectContent>
              </Select>
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
          </CardContent>
        </Card>

        {/* Error Message */}
        {error && (
          <div className="mb-6 p-4 bg-destructive/10 border border-destructive/20 rounded-lg">
            <p className="text-sm font-medium text-destructive">{error}</p>
          </div>
        )}

        {/* Users Table */}
        <Card className="overflow-hidden">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead
                  className={`cursor-pointer hover:bg-muted transition-colors ${
                    isSorted('FirstName') 
                      ? 'bg-primary-50 text-primary-700' 
                      : ''
                  }`}
                  onClick={() => handleSort('FirstName')}
                >
                  <div className="flex items-center space-x-1">
                    <span>First Name</span>
                    {getSortIcon('FirstName')}
                  </div>
                </TableHead>
                <TableHead
                  className={`cursor-pointer hover:bg-muted transition-colors ${
                    isSorted('LastName') 
                      ? 'bg-primary-50 text-primary-700' 
                      : ''
                  }`}
                  onClick={() => handleSort('LastName')}
                >
                  <div className="flex items-center space-x-1">
                    <span>Last Name</span>
                    {getSortIcon('LastName')}
                  </div>
                </TableHead>
                <TableHead
                  className={`cursor-pointer hover:bg-muted transition-colors ${
                    isSorted('Email') 
                      ? 'bg-primary-50 text-primary-700' 
                      : ''
                  }`}
                  onClick={() => handleSort('Email')}
                >
                  <div className="flex items-center space-x-1">
                    <span>Email</span>
                    {getSortIcon('Email')}
                  </div>
                </TableHead>
                <TableHead>Phone</TableHead>
                <TableHead>Roles</TableHead>
                <TableHead>Status</TableHead>
                <TableHead
                  className={`cursor-pointer hover:bg-muted transition-colors ${
                    isSorted('CreatedAt') 
                      ? 'bg-primary-50 text-primary-700' 
                      : ''
                  }`}
                  onClick={() => handleSort('CreatedAt')}
                >
                  <div className="flex items-center space-x-1">
                    <span>Created</span>
                    {getSortIcon('CreatedAt')}
                  </div>
                </TableHead>
                <TableHead>Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
                {isLoading ? (
                  <TableRow>
                    <TableCell colSpan="8" className="text-center text-muted-foreground">
                      Loading users...
                    </TableCell>
                  </TableRow>
                ) : users.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan="8" className="text-center text-muted-foreground">
                      No users found
                    </TableCell>
                  </TableRow>
                ) : (
                  users.map((user) => (
                    <TableRow key={user.id}>
                      <TableCell className="font-medium">
                        {editingUserId === user.id ? (
                          <Input
                            value={editFormData.firstName}
                            onChange={(e) => setEditFormData({ ...editFormData, firstName: e.target.value })}
                            className="w-32"
                          />
                        ) : (
                          user.firstName
                        )}
                      </TableCell>
                      <TableCell>
                        {editingUserId === user.id ? (
                          <Input
                            value={editFormData.lastName}
                            onChange={(e) => setEditFormData({ ...editFormData, lastName: e.target.value })}
                            className="w-32"
                          />
                        ) : (
                          user.lastName
                        )}
                      </TableCell>
                      <TableCell>
                        {editingUserId === user.id ? (
                          <Input
                            type="email"
                            value={editFormData.email}
                            onChange={(e) => setEditFormData({ ...editFormData, email: e.target.value })}
                            className="w-48"
                          />
                        ) : (
                          user.email
                        )}
                      </TableCell>
                      <TableCell>
                        {editingUserId === user.id ? (
                          <Input
                            value={editFormData.phoneNumber}
                            onChange={(e) => setEditFormData({ ...editFormData, phoneNumber: e.target.value })}
                            className="w-32"
                          />
                        ) : (
                          user.phoneNumber || '-'
                        )}
                      </TableCell>
                      <TableCell>
                        {editingUserId === user.id ? (
                          <div className="flex flex-col gap-2 w-48">
                            {availableRoles.map((role) => (
                              <label key={role.id} className="flex items-center space-x-2 cursor-pointer">
                                <input
                                  type="checkbox"
                                  checked={editFormData.roleIds.includes(role.id)}
                                  onChange={(e) => {
                                    if (e.target.checked) {
                                      setEditFormData({
                                        ...editFormData,
                                        roleIds: [...editFormData.roleIds, role.id],
                                      });
                                    } else {
                                      setEditFormData({
                                        ...editFormData,
                                        roleIds: editFormData.roleIds.filter(id => id !== role.id),
                                      });
                                    }
                                  }}
                                  className="h-4 w-4 text-primary-600 rounded border-gray-300"
                                />
                                <span className="text-sm text-gray-700">{role.name}</span>
                              </label>
                            ))}
                          </div>
                        ) : (
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
                        )}
                      </TableCell>
                      <TableCell>
                        {editingUserId === user.id ? (
                          <Select
                            value={editFormData.isActive ? 'true' : 'false'}
                            onValueChange={(value) => setEditFormData({ ...editFormData, isActive: value === 'true' })}
                          >
                            <SelectTrigger className="w-32 h-8">
                              <SelectValue />
                            </SelectTrigger>
                            <SelectContent>
                              <SelectItem value="true">Active</SelectItem>
                              <SelectItem value="false">Inactive</SelectItem>
                            </SelectContent>
                          </Select>
                        ) : (
                          <span
                            className={`px-2 py-1 text-xs font-medium rounded-full ${
                              user.isActive
                                ? 'bg-green-100 text-green-700'
                                : 'bg-red-100 text-red-700'
                            }`}
                          >
                            {user.isActive ? 'Active' : 'Inactive'}
                          </span>
                        )}
                      </TableCell>
                      <TableCell>
                        {new Date(user.createdAt).toLocaleDateString()}
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center space-x-2">
                          {editingUserId === user.id ? (
                            <>
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={handleSaveEdit}
                                disabled={isLoading}
                                className="text-green-600 hover:text-green-700"
                              >
                                <Save className="h-4 w-4" />
                              </Button>
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={handleCancelEdit}
                                disabled={isLoading}
                                className="text-gray-600 hover:text-gray-700"
                              >
                                <X className="h-4 w-4" />
                              </Button>
                            </>
                          ) : (
                            <>
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => handleEditClick(user)}
                                disabled={isLoading}
                                className="text-blue-600 hover:text-blue-700"
                              >
                                <Edit2 className="h-4 w-4" />
                              </Button>
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => handleDeleteClick(user.id)}
                                disabled={isLoading}
                                className="text-red-600 hover:text-red-700"
                              >
                                <Trash2 className="h-4 w-4" />
                              </Button>
                            </>
                          )}
                        </div>
                      </TableCell>
                    </TableRow>
                  ))
                )}
            </TableBody>
          </Table>

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
        </Card>
      </div>

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={(open) => {
        setDeleteDialogOpen(open);
        if (!open) {
          setUserToDelete(null);
        }
      }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you sure?</AlertDialogTitle>
            <AlertDialogDescription>
              This action will deactivate the user. The user will no longer be able to access the system.
              This action cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isLoading}>
              Cancel
            </AlertDialogCancel>
            <Button
              variant="destructive"
              onClick={handleConfirmDelete}
              disabled={isLoading}
            >
              {isLoading ? 'Deactivating...' : 'Deactivate User'}
            </Button>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
};

export default UserManagement;
