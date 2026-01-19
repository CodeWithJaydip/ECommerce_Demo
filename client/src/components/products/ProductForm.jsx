import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { Save, X, ArrowLeft } from 'lucide-react';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Textarea } from '../ui/textarea';
import {
  Form,
  FormItem,
  FormLabel,
  FormControl,
  FormMessage,
  FormDescription,
} from '../ui/form';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../ui/select';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { useAppSelector } from '../../hooks/redux';
import * as productApi from '../../services/api/productApi';
import * as categoryApi from '../../services/api/categoryApi';
import * as userApi from '../../services/api/userApi';
import Header from '../common/Header';

// Validation schema using Zod
const productSchema = z.object({
  name: z
    .string()
    .min(1, 'Product name is required')
    .min(2, 'Product name must be at least 2 characters')
    .max(200, 'Product name must be less than 200 characters'),
  description: z
    .string()
    .max(1000, 'Description must be less than 1000 characters')
    .optional()
    .or(z.literal('')),
  price: z
    .number()
    .min(0.01, 'Price must be greater than 0'),
  stock: z
    .number()
    .min(0, 'Stock must be greater than or equal to 0')
    .int('Stock must be a whole number'),
  categoryId: z
    .number()
    .min(1, 'Category is required'),
  sellerId: z
    .number()
    .optional(),
  sku: z
    .string()
    .max(100, 'SKU must be less than 100 characters')
    .optional()
    .or(z.literal('')),
});

const ProductForm = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEditMode = !!id;
  const { user: currentUser, isAuthenticated } = useAppSelector((state) => state.auth);
  
  // Check user roles
  const isSuperAdmin = currentUser?.roles?.includes('Super Admin');
  const isSeller = currentUser?.roles?.includes('Seller');
  const canManageProducts = isSuperAdmin || isSeller;

  // State
  const [categories, setCategories] = useState([]);
  const [sellers, setSellers] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const [selectedImage, setSelectedImage] = useState(null);
  const [imagePreview, setImagePreview] = useState(null);
  const [removeImage, setRemoveImage] = useState(false);

  // React Hook Form setup
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    watch,
  } = useForm({
    resolver: zodResolver(productSchema),
    defaultValues: {
      name: '',
      description: '',
      price: 0,
      stock: 0,
      categoryId: undefined,
      sellerId: undefined,
      sku: '',
    },
  });

  const categoryId = watch('categoryId');
  const sellerId = watch('sellerId');

  // Fetch categories
  const fetchCategories = async () => {
    try {
      const data = await categoryApi.getCategories();
      setCategories(data || []);
    } catch (err) {
      console.error('Error fetching categories:', err);
    }
  };

  // Fetch sellers (only for Super Admin)
  const fetchSellers = async () => {
    if (!isSuperAdmin) return;
    
    try {
      // Get all users and filter for sellers
      const params = {
        pageNumber: 1,
        pageSize: 1000,
        roleName: 'Seller',
        isActive: true,
      };
      const result = await userApi.getUsers(params);
      setSellers(result.items || []);
    } catch (err) {
      console.error('Error fetching sellers:', err);
    }
  };

  // Fetch product for edit
  const fetchProduct = async () => {
    if (!isEditMode) return;

    setIsLoading(true);
    try {
      const product = await productApi.getProductById(parseInt(id));
      reset({
        name: product.name,
        description: product.description || '',
        price: product.price,
        stock: product.stock,
        categoryId: product.categoryId,
        sellerId: isSuperAdmin ? product.sellerId : undefined,
        sku: product.sku || '',
      });
      if (product.imagePath) {
        setImagePreview(`${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5176'}/${product.imagePath.startsWith('/') ? product.imagePath.slice(1) : product.imagePath}`);
      }
    } catch (err) {
      setError(err.message || 'Failed to fetch product');
      console.error('Error fetching product:', err);
    } finally {
      setIsLoading(false);
    }
  };

  // Load data on mount
  useEffect(() => {
    if (!isAuthenticated || !canManageProducts) {
      navigate('/products');
      return;
    }

    fetchCategories();
    if (isSuperAdmin) {
      fetchSellers();
    }
    if (isEditMode) {
      fetchProduct();
    }
  }, [id, isAuthenticated, canManageProducts, isSuperAdmin, isEditMode, navigate]);

  // Handle image file selection
  const handleImageChange = (e) => {
    const file = e.target.files?.[0];
    if (file) {
      setSelectedImage(file);
      setRemoveImage(false);
      // Create preview
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result);
      };
      reader.readAsDataURL(file);
    }
  };

  // Handle remove image
  const handleRemoveImage = () => {
    setSelectedImage(null);
    setImagePreview(null);
    setRemoveImage(true);
  };

  // Handle form submit
  const onSubmit = async (data) => {
    setIsLoading(true);
    setError(null);

    try {
      const productData = {
        name: data.name,
        description: data.description || null,
        price: data.price,
        stock: data.stock,
        categoryId: data.categoryId,
        sku: data.sku || null,
        image: selectedImage,
        ...(isSuperAdmin && data.sellerId && { sellerId: data.sellerId }),
        ...(isEditMode && removeImage && { removeImage: true }),
      };

      if (isEditMode) {
        await productApi.updateProduct(parseInt(id), productData);
      } else {
        await productApi.createProduct(productData);
      }
      
      navigate('/products');
    } catch (err) {
      setError(err.message || `Failed to ${isEditMode ? 'update' : 'create'} product`);
      console.error(`Error ${isEditMode ? 'updating' : 'creating'} product:`, err);
    } finally {
      setIsLoading(false);
    }
  };

  if (!isAuthenticated || !canManageProducts) {
    return null;
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-white">
      <Header />
      
      {/* Page Header */}
      <div className="bg-white shadow-sm border-b border-gray-200 pt-16">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">
                {isEditMode ? 'Edit Product' : 'Add New Product'}
              </h1>
              <p className="mt-1 text-sm text-gray-600">
                {isEditMode ? 'Update product information' : 'Create a new product listing'}
              </p>
            </div>
            <Button variant="outline" onClick={() => navigate('/products')}>
              <ArrowLeft className="h-4 w-4 mr-2" />
              Back to Products
            </Button>
          </div>
        </div>
      </div>

      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Error Message */}
        {error && (
          <div className="mb-6 p-4 bg-destructive/10 border border-destructive/20 rounded-lg">
            <p className="text-sm font-medium text-destructive">{error}</p>
          </div>
        )}

        {/* Product Form */}
        <Card>
          <CardHeader>
            <CardTitle>Product Information</CardTitle>
          </CardHeader>
          <CardContent>
            <Form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
              {/* Product Name */}
              <FormItem>
                <FormLabel>Product Name *</FormLabel>
                <FormControl>
                  <Input
                    placeholder="Enter product name"
                    {...register('name')}
                    className={errors.name ? 'border-destructive' : ''}
                  />
                </FormControl>
                {!errors.name && (
                  <FormDescription>
                    Product name must be between 2 and 200 characters
                  </FormDescription>
                )}
                {errors.name && <FormMessage>{errors.name.message}</FormMessage>}
              </FormItem>

              {/* Description */}
              <FormItem>
                <FormLabel>Description</FormLabel>
                <FormControl>
                  <Textarea
                    placeholder="Enter product description (optional)"
                    {...register('description')}
                    className={errors.description ? 'border-destructive' : ''}
                    rows={4}
                  />
                </FormControl>
                {!errors.description && (
                  <FormDescription>
                    Description must be less than 1000 characters
                  </FormDescription>
                )}
                {errors.description && <FormMessage>{errors.description.message}</FormMessage>}
              </FormItem>

              {/* Price and Stock */}
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <FormItem>
                  <FormLabel>Price *</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      step="0.01"
                      min="0.01"
                      placeholder="0.00"
                      {...register('price', { valueAsNumber: true })}
                      className={errors.price ? 'border-destructive' : ''}
                    />
                  </FormControl>
                  {errors.price && <FormMessage>{errors.price.message}</FormMessage>}
                </FormItem>

                <FormItem>
                  <FormLabel>Stock *</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      min="0"
                      step="1"
                      placeholder="0"
                      {...register('stock', { valueAsNumber: true })}
                      className={errors.stock ? 'border-destructive' : ''}
                    />
                  </FormControl>
                  {errors.stock && <FormMessage>{errors.stock.message}</FormMessage>}
                </FormItem>
              </div>

              {/* Category */}
              <FormItem>
                <FormLabel>Category *</FormLabel>
                <Select
                  value={categoryId ? categoryId.toString() : undefined}
                  onValueChange={(value) => setValue('categoryId', parseInt(value), { shouldValidate: true })}
                >
                  <FormControl>
                    <SelectTrigger className={errors.categoryId ? 'border-destructive' : ''}>
                      <SelectValue placeholder="Select a category" />
                    </SelectTrigger>
                  </FormControl>
                  <SelectContent>
                    {categories.map((category) => (
                      <SelectItem key={category.id} value={category.id.toString()}>
                        {category.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {errors.categoryId && <FormMessage>{errors.categoryId.message}</FormMessage>}
              </FormItem>

              {/* Seller (Super Admin only) */}
              {isSuperAdmin && (
                <FormItem>
                  <FormLabel>Seller</FormLabel>
                  <Select
                    value={sellerId ? sellerId.toString() : 'all'}
                    onValueChange={(value) => {
                      if (value === 'all') {
                        setValue('sellerId', undefined);
                      } else {
                        setValue('sellerId', parseInt(value));
                      }
                    }}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Select a seller (optional)" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      <SelectItem value="all">Current User</SelectItem>
                      {sellers.map((seller) => (
                        <SelectItem key={seller.id} value={seller.id.toString()}>
                          {seller.firstName} {seller.lastName} ({seller.email})
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormDescription>
                    Select a seller for this product. If not selected, it will be assigned to you.
                  </FormDescription>
                </FormItem>
              )}

              {/* SKU */}
              <FormItem>
                <FormLabel>SKU (Stock Keeping Unit)</FormLabel>
                <FormControl>
                  <Input
                    placeholder="Enter SKU (optional)"
                    {...register('sku')}
                    className={errors.sku ? 'border-destructive' : ''}
                  />
                </FormControl>
                {!errors.sku && (
                  <FormDescription>
                    Unique identifier for inventory management
                  </FormDescription>
                )}
                {errors.sku && <FormMessage>{errors.sku.message}</FormMessage>}
              </FormItem>

              {/* Product Image */}
              <FormItem>
                <FormLabel>Product Image</FormLabel>
                <FormControl>
                  <div className="space-y-4">
                    {imagePreview && (
                      <div className="relative inline-block">
                        <img
                          src={imagePreview}
                          alt="Product preview"
                          className="h-32 w-32 object-cover rounded-lg border border-gray-200"
                        />
                        {isEditMode && !selectedImage && (
                          <Button
                            type="button"
                            variant="destructive"
                            size="sm"
                            className="absolute top-0 right-0 -mt-2 -mr-2"
                            onClick={handleRemoveImage}
                          >
                            <X className="h-3 w-3" />
                          </Button>
                        )}
                      </div>
                    )}
                    <div className="flex gap-2">
                      <Input
                        type="file"
                        accept="image/*"
                        onChange={handleImageChange}
                        className="cursor-pointer"
                      />
                      {imagePreview && selectedImage && (
                        <Button
                          type="button"
                          variant="outline"
                          size="sm"
                          onClick={handleRemoveImage}
                        >
                          <X className="h-4 w-4 mr-2" />
                          Remove
                        </Button>
                      )}
                    </div>
                  </div>
                </FormControl>
                <FormDescription>
                  Upload an image for the product (JPG, PNG, GIF, WEBP, max 5MB)
                </FormDescription>
              </FormItem>

              {/* Form Actions */}
              <div className="flex gap-2 pt-4">
                <Button type="submit" disabled={isLoading}>
                  <Save className="h-4 w-4 mr-2" />
                  {isLoading ? 'Saving...' : isEditMode ? 'Update Product' : 'Create Product'}
                </Button>
                <Button type="button" variant="outline" onClick={() => navigate('/products')} disabled={isLoading}>
                  <X className="h-4 w-4 mr-2" />
                  Cancel
                </Button>
              </div>
            </Form>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default ProductForm;
