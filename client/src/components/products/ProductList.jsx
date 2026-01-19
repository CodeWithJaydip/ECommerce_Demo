import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ShoppingBag, Plus, Edit2, Trash2, Package, DollarSign, Tag } from 'lucide-react';
import { Button } from '../ui/button';
import { Card, CardContent } from '../ui/card';
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
import { useAppSelector } from '../../hooks/redux';
import * as productApi from '../../services/api/productApi';
import Header from '../common/Header';

const ProductList = () => {
  const navigate = useNavigate();
  const { user: currentUser, isAuthenticated } = useAppSelector((state) => state.auth);
  
  // Check user roles
  const isSuperAdmin = currentUser?.roles?.includes('Super Admin');
  const isSeller = currentUser?.roles?.includes('Seller');
  const canManageProducts = isSuperAdmin || isSeller;

  // State for data
  const [products, setProducts] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [productToDelete, setProductToDelete] = useState(null);

  // Fetch products
  const fetchProducts = async () => {
    setIsLoading(true);
    setError(null);
    
    try {
      const data = await productApi.getProducts();
      setProducts(data || []);
    } catch (err) {
      setError(err.message || 'Failed to fetch products');
      console.error('Error fetching products:', err);
    } finally {
      setIsLoading(false);
    }
  };

  // Load products on mount
  useEffect(() => {
    fetchProducts();
  }, []);

  // Handle delete click
  const handleDeleteClick = (productId) => {
    setProductToDelete(productId);
    setDeleteDialogOpen(true);
  };

  // Handle confirm delete
  const handleConfirmDelete = async () => {
    if (!productToDelete) return;

    setIsLoading(true);
    setError(null);

    try {
      await productApi.deleteProduct(productToDelete);
      setDeleteDialogOpen(false);
      setProductToDelete(null);
      await fetchProducts();
    } catch (err) {
      setError(err.message || 'Failed to delete product');
      console.error('Error deleting product:', err);
    } finally {
      setIsLoading(false);
    }
  };

  // Handle edit click
  const handleEditClick = (productId) => {
    navigate(`/products/edit/${productId}`);
  };

  // Handle add product click
  const handleAddProduct = () => {
    navigate('/products/new');
  };

  const getImageUrl = (imagePath) => {
    if (!imagePath) return null;
    return `${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5176'}/${imagePath.startsWith('/') ? imagePath.slice(1) : imagePath}`;
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-white">
      <Header />
      
      {/* Page Header */}
      <div className="bg-white shadow-sm border-b border-gray-200 pt-16">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <div className="flex justify-between items-center">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">Products</h1>
              <p className="mt-1 text-sm text-gray-600">Browse our collection of products</p>
            </div>
            {canManageProducts && (
              <Button onClick={handleAddProduct}>
                <Plus className="h-4 w-4 mr-2" />
                Add Product
              </Button>
            )}
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

        {/* Products Grid */}
        {isLoading && products.length === 0 ? (
          <div className="text-center py-12">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
            <p className="mt-4 text-muted-foreground">Loading products...</p>
          </div>
        ) : products.length === 0 ? (
          <div className="text-center py-12">
            <ShoppingBag className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">No products</h3>
            <p className="mt-1 text-sm text-gray-500">Get started by creating a new product.</p>
            {canManageProducts && (
              <div className="mt-6">
                <Button onClick={handleAddProduct}>
                  <Plus className="h-4 w-4 mr-2" />
                  Add Product
                </Button>
              </div>
            )}
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {products.map((product) => (
              <Card key={product.id} className="overflow-hidden hover:shadow-lg transition-shadow duration-200">
                {/* Product Image */}
                <div className="relative aspect-square bg-gray-100">
                  {getImageUrl(product.imagePath) ? (
                    <img
                      src={getImageUrl(product.imagePath)}
                      alt={product.name}
                      className="w-full h-full object-cover"
                      onError={(e) => {
                        e.target.style.display = 'none';
                        e.target.nextSibling.style.display = 'flex';
                      }}
                    />
                  ) : null}
                  <div className={`absolute inset-0 flex items-center justify-center bg-gray-100 ${getImageUrl(product.imagePath) ? 'hidden' : ''}`}>
                    <Package className="h-16 w-16 text-gray-400" />
                  </div>
                  
                  {/* Stock Badge */}
                  <div className="absolute top-2 right-2">
                    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                      product.stock > 10 
                        ? 'bg-green-100 text-green-800' 
                        : product.stock > 0 
                        ? 'bg-yellow-100 text-yellow-800' 
                        : 'bg-red-100 text-red-800'
                    }`}>
                      {product.stock > 0 ? `${product.stock} in stock` : 'Out of stock'}
                    </span>
                  </div>

                  {/* Action Buttons (for authorized users) */}
                  {canManageProducts && (
                    <div className="absolute top-2 left-2 flex gap-2">
                      <Button
                        variant="secondary"
                        size="sm"
                        onClick={() => handleEditClick(product.id)}
                        className="h-8 w-8 p-0 bg-white/90 hover:bg-white"
                      >
                        <Edit2 className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="destructive"
                        size="sm"
                        onClick={() => handleDeleteClick(product.id)}
                        className="h-8 w-8 p-0 bg-white/90 hover:bg-white"
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  )}
                </div>

                <CardContent className="p-4">
                  {/* Category Badge */}
                  <div className="mb-2">
                    <span className="inline-flex items-center px-2 py-1 rounded-md text-xs font-medium bg-primary-100 text-primary-800">
                      <Tag className="h-3 w-3 mr-1" />
                      {product.categoryName}
                    </span>
                  </div>

                  {/* Product Name */}
                  <h3 className="text-lg font-semibold text-gray-900 mb-2 line-clamp-2">
                    {product.name}
                  </h3>

                  {/* Description */}
                  {product.description && (
                    <p className="text-sm text-gray-600 mb-3 line-clamp-2">
                      {product.description}
                    </p>
                  )}

                  {/* Price and Seller */}
                  <div className="flex items-center justify-between mt-4">
                    <div>
                      <div className="flex items-center text-2xl font-bold text-gray-900">
                        <DollarSign className="h-5 w-5 mr-1" />
                        {product.price.toFixed(2)}
                      </div>
                      {product.sellerName && (
                        <p className="text-xs text-gray-500 mt-1">by {product.sellerName}</p>
                      )}
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </div>

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={(open) => {
        setDeleteDialogOpen(open);
        if (!open) {
          setProductToDelete(null);
        }
      }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you sure?</AlertDialogTitle>
            <AlertDialogDescription>
              This action will deactivate the product. The product will no longer be visible to customers.
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
              {isLoading ? 'Deleting...' : 'Delete Product'}
            </Button>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
};

export default ProductList;
