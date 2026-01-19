import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { Plus, Edit2, Trash2, X, Save } from 'lucide-react';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Textarea } from '../ui/textarea';
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
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '../ui/table';
import {
  Form,
  FormItem,
  FormLabel,
  FormControl,
  FormMessage,
  FormDescription,
} from '../ui/form';
import { useAppSelector } from '../../hooks/redux';
import * as categoryApi from '../../services/api/categoryApi';
import Header from '../common/Header';

// Validation schema using Zod
const categorySchema = z.object({
  name: z
    .string()
    .min(1, 'Category name is required')
    .min(2, 'Category name must be at least 2 characters')
    .max(100, 'Category name must be less than 100 characters'),
  description: z
    .string()
    .max(255, 'Description must be less than 255 characters')
    .optional()
    .or(z.literal('')),
});

const CategoryManagement = () => {
  const navigate = useNavigate();
  const { user: currentUser, isAuthenticated } = useAppSelector((state) => state.auth);
  
  // Check if user is Super Admin
  const isSuperAdmin = currentUser?.roles?.includes('Super Admin');

  // State for data
  const [categories, setCategories] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  // State for form (create/edit)
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingCategoryId, setEditingCategoryId] = useState(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [categoryToDelete, setCategoryToDelete] = useState(null);

  // React Hook Form setup
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm({
    resolver: zodResolver(categorySchema),
    defaultValues: {
      name: '',
      description: '',
    },
  });

  // Fetch categories
  const fetchCategories = async () => {
    setIsLoading(true);
    setError(null);
    
    try {
      const data = await categoryApi.getCategories();
      setCategories(data || []);
    } catch (err) {
      setError(err.message || 'Failed to fetch categories');
      console.error('Error fetching categories:', err);
    } finally {
      setIsLoading(false);
    }
  };

  // Load categories on mount
  useEffect(() => {
    if (isAuthenticated && isSuperAdmin) {
      fetchCategories();
    }
  }, [isAuthenticated, isSuperAdmin]);

  // Redirect if not Super Admin
  useEffect(() => {
    if (isAuthenticated && !isSuperAdmin) {
      navigate('/dashboard');
    }
  }, [isAuthenticated, isSuperAdmin, navigate]);

  // Handle form open for create
  const handleCreateClick = () => {
    setEditingCategoryId(null);
    reset({
      name: '',
      description: '',
    });
    setIsFormOpen(true);
  };

  // Handle form open for edit
  const handleEditClick = (category) => {
    setEditingCategoryId(category.id);
    reset({
      name: category.name,
      description: category.description || '',
    });
    setIsFormOpen(true);
  };

  // Handle form cancel
  const handleFormCancel = () => {
    setIsFormOpen(false);
    setEditingCategoryId(null);
    reset({
      name: '',
      description: '',
    });
  };

  // Handle form submit
  const onSubmit = async (data) => {
    setIsLoading(true);
    setError(null);

    try {
      if (editingCategoryId) {
        // Update existing category
        await categoryApi.updateCategory(editingCategoryId, {
          name: data.name,
          description: data.description || null,
        });
      } else {
        // Create new category
        await categoryApi.createCategory({
          name: data.name,
          description: data.description || null,
        });
      }
      
      setIsFormOpen(false);
      setEditingCategoryId(null);
      reset({
        name: '',
        description: '',
      });
      await fetchCategories();
    } catch (err) {
      setError(err.message || `Failed to ${editingCategoryId ? 'update' : 'create'} category`);
      console.error(`Error ${editingCategoryId ? 'updating' : 'creating'} category:`, err);
    } finally {
      setIsLoading(false);
    }
  };

  // Handle delete click
  const handleDeleteClick = (categoryId) => {
    setCategoryToDelete(categoryId);
    setDeleteDialogOpen(true);
  };

  // Handle confirm delete
  const handleConfirmDelete = async () => {
    if (!categoryToDelete) return;

    setIsLoading(true);
    setError(null);

    try {
      await categoryApi.deleteCategory(categoryToDelete);
      setDeleteDialogOpen(false);
      setCategoryToDelete(null);
      await fetchCategories();
    } catch (err) {
      setError(err.message || 'Failed to delete category');
      console.error('Error deleting category:', err);
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
              <h1 className="text-3xl font-bold text-gray-900">Category Management</h1>
              <p className="mt-1 text-sm text-gray-600">Manage product categories</p>
            </div>
            <div className="flex gap-2">
              {!isFormOpen && (
                <Button onClick={handleCreateClick}>
                  <Plus className="h-4 w-4 mr-2" />
                  Add Category
                </Button>
              )}
              <Button onClick={() => navigate('/dashboard')} variant="outline">
                Back to Dashboard
              </Button>
            </div>
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Error Message */}
        {error && (
          <div className="mb-6 p-4 bg-destructive/10 border border-destructive/20 rounded-lg">
            <p className="text-sm font-medium text-destructive">{error}</p>
          </div>
        )}

        {/* Create/Edit Form */}
        {isFormOpen && (
          <Card className="mb-6">
            <CardHeader>
              <CardTitle>
                {editingCategoryId ? 'Edit Category' : 'Create New Category'}
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                <FormItem>
                  <FormLabel>Category Name *</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="Enter category name"
                      {...register('name')}
                      className={errors.name ? 'border-destructive' : ''}
                    />
                  </FormControl>
                  {!errors.name && (
                    <FormDescription>
                      Category name must be between 2 and 100 characters
                    </FormDescription>
                  )}
                  {errors.name && <FormMessage>{errors.name.message}</FormMessage>}
                </FormItem>

                <FormItem>
                  <FormLabel>Description</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Enter category description (optional)"
                      {...register('description')}
                      className={errors.description ? 'border-destructive' : ''}
                      rows={4}
                    />
                  </FormControl>
                  {!errors.description && (
                    <FormDescription>
                      Description must be less than 255 characters
                    </FormDescription>
                  )}
                  {errors.description && <FormMessage>{errors.description.message}</FormMessage>}
                </FormItem>

                <div className="flex gap-2">
                  <Button type="submit" disabled={isLoading}>
                    <Save className="h-4 w-4 mr-2" />
                    {isLoading ? 'Saving...' : editingCategoryId ? 'Update Category' : 'Create Category'}
                  </Button>
                  <Button type="button" variant="outline" onClick={handleFormCancel} disabled={isLoading}>
                    <X className="h-4 w-4 mr-2" />
                    Cancel
                  </Button>
                </div>
              </Form>
            </CardContent>
          </Card>
        )}

        {/* Categories Table */}
        <Card>
          <CardHeader>
            <CardTitle>Categories</CardTitle>
          </CardHeader>
          <CardContent>
            {isLoading && !categories.length ? (
              <div className="text-center py-8 text-muted-foreground">
                Loading categories...
              </div>
            ) : categories.length === 0 ? (
              <div className="text-center py-8 text-muted-foreground">
                No categories found. Click "Add Category" to create one.
              </div>
            ) : (
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>ID</TableHead>
                    <TableHead>Name</TableHead>
                    <TableHead>Description</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Created At</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {categories.map((category) => (
                    <TableRow key={category.id}>
                      <TableCell className="font-medium">{category.id}</TableCell>
                      <TableCell className="font-medium">{category.name}</TableCell>
                      <TableCell className="text-muted-foreground">
                        {category.description || '-'}
                      </TableCell>
                      <TableCell>
                        <span
                          className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                            category.isActive
                              ? 'bg-green-100 text-green-800'
                              : 'bg-red-100 text-red-800'
                          }`}
                        >
                          {category.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </TableCell>
                      <TableCell className="text-muted-foreground">
                        {new Date(category.createdAt).toLocaleDateString()}
                      </TableCell>
                      <TableCell>
                        <div className="flex gap-2">
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleEditClick(category)}
                            disabled={isLoading || isFormOpen}
                          >
                            <Edit2 className="h-4 w-4" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleDeleteClick(category.id)}
                            disabled={isLoading || isFormOpen}
                            className="text-destructive hover:text-destructive"
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            )}
          </CardContent>
        </Card>
      </div>

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={(open) => {
        setDeleteDialogOpen(open);
        if (!open) {
          setCategoryToDelete(null);
        }
      }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you sure?</AlertDialogTitle>
            <AlertDialogDescription>
              This action will deactivate the category. The category will no longer be available for use.
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
              {isLoading ? 'Deactivating...' : 'Deactivate Category'}
            </Button>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
};

export default CategoryManagement;
