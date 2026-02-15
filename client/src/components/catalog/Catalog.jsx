import { useEffect, useState, useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Search, X, Package, DollarSign, Tag, ChevronLeft, ChevronRight, SlidersHorizontal, ShoppingBag, ShoppingCart, Check } from 'lucide-react';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Card, CardContent } from '../ui/card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../ui/select';
import * as productApi from '../../services/api/productApi';
import * as categoryApi from '../../services/api/categoryApi';
import * as basketApi from '../../services/api/basketApi';
import { useAppDispatch, useAppSelector } from '../../hooks/redux';
import { fetchBasketCount } from '../../store/slices/basketSlice';
import Header from '../common/Header';

const Catalog = () => {
  const dispatch = useAppDispatch();
  const { isAuthenticated } = useAppSelector((state) => state.auth);
  const [searchParams, setSearchParams] = useSearchParams();
  const [addingToBasket, setAddingToBasket] = useState({});
  const [addedToBasket, setAddedToBasket] = useState({});

  // Initialize state from URL search params
  const [search, setSearch] = useState(searchParams.get('search') || '');
  const [categoryId, setCategoryId] = useState(searchParams.get('categoryId') || '');
  const [minPrice, setMinPrice] = useState(searchParams.get('minPrice') || '');
  const [maxPrice, setMaxPrice] = useState(searchParams.get('maxPrice') || '');
  const [inStock, setInStock] = useState(searchParams.get('inStock') || '');
  const [sortBy, setSortBy] = useState(searchParams.get('sortBy') || 'createdAt');
  const [sortDescending, setSortDescending] = useState(searchParams.get('sortDescending') !== 'false');
  const [pageNumber, setPageNumber] = useState(parseInt(searchParams.get('pageNumber')) || 1);
  const pageSize = 12;

  // Data state
  const [products, setProducts] = useState([]);
  const [metadata, setMetadata] = useState(null);
  const [categories, setCategories] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const [filtersOpen, setFiltersOpen] = useState(false);

  // Debounced search state
  const [debouncedSearch, setDebouncedSearch] = useState(search);

  // Debounce search input
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearch(search);
      setPageNumber(1);
    }, 500);
    return () => clearTimeout(timer);
  }, [search]);

  // Fetch categories on mount
  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const data = await categoryApi.getCategories();
        setCategories(data || []);
      } catch (err) {
        console.error('Error fetching categories:', err);
      }
    };
    fetchCategories();
  }, []);

  // Build params and sync with URL
  const buildParams = useCallback(() => {
    const params = {
      pageNumber,
      pageSize,
      sortBy,
      sortDescending,
    };
    if (debouncedSearch) params.search = debouncedSearch;
    if (categoryId) params.categoryId = parseInt(categoryId);
    if (minPrice !== '') params.minPrice = parseFloat(minPrice);
    if (maxPrice !== '') params.maxPrice = parseFloat(maxPrice);
    if (inStock === 'true') params.inStock = true;
    return params;
  }, [debouncedSearch, categoryId, minPrice, maxPrice, inStock, sortBy, sortDescending, pageNumber, pageSize]);

  // Sync state to URL
  useEffect(() => {
    const params = new URLSearchParams();
    if (debouncedSearch) params.set('search', debouncedSearch);
    if (categoryId) params.set('categoryId', categoryId);
    if (minPrice !== '') params.set('minPrice', minPrice);
    if (maxPrice !== '') params.set('maxPrice', maxPrice);
    if (inStock) params.set('inStock', inStock);
    if (sortBy && sortBy !== 'createdAt') params.set('sortBy', sortBy);
    if (!sortDescending) params.set('sortDescending', 'false');
    if (pageNumber > 1) params.set('pageNumber', pageNumber.toString());
    setSearchParams(params, { replace: true });
  }, [debouncedSearch, categoryId, minPrice, maxPrice, inStock, sortBy, sortDescending, pageNumber, setSearchParams]);

  // Fetch products
  useEffect(() => {
    const fetchProducts = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const params = buildParams();
        const data = await productApi.getCatalog(params);
        setProducts(data?.items || []);
        setMetadata(data?.metadata || null);
      } catch (err) {
        setError(err.message || 'Failed to fetch products');
        console.error('Error fetching catalog:', err);
      } finally {
        setIsLoading(false);
      }
    };
    fetchProducts();
  }, [buildParams]);

  // Sort option change handler
  const handleSortChange = (value) => {
    switch (value) {
      case 'newest':
        setSortBy('createdAt');
        setSortDescending(true);
        break;
      case 'oldest':
        setSortBy('createdAt');
        setSortDescending(false);
        break;
      case 'price-low':
        setSortBy('price');
        setSortDescending(false);
        break;
      case 'price-high':
        setSortBy('price');
        setSortDescending(true);
        break;
      case 'name-az':
        setSortBy('name');
        setSortDescending(false);
        break;
      case 'name-za':
        setSortBy('name');
        setSortDescending(true);
        break;
      default:
        setSortBy('createdAt');
        setSortDescending(true);
    }
    setPageNumber(1);
  };

  // Get current sort value for the select
  const getCurrentSortValue = () => {
    if (sortBy === 'price' && !sortDescending) return 'price-low';
    if (sortBy === 'price' && sortDescending) return 'price-high';
    if (sortBy === 'name' && !sortDescending) return 'name-az';
    if (sortBy === 'name' && sortDescending) return 'name-za';
    if (sortBy === 'createdAt' && !sortDescending) return 'oldest';
    return 'newest';
  };

  // Clear all filters
  const clearFilters = () => {
    setSearch('');
    setDebouncedSearch('');
    setCategoryId('');
    setMinPrice('');
    setMaxPrice('');
    setInStock('');
    setSortBy('createdAt');
    setSortDescending(true);
    setPageNumber(1);
  };

  // Check if any filters are active
  const hasActiveFilters = debouncedSearch || categoryId || minPrice !== '' || maxPrice !== '' || inStock === 'true';

  // Get active filter count
  const activeFilterCount = [debouncedSearch, categoryId, minPrice !== '' ? minPrice : '', maxPrice !== '' ? maxPrice : '', inStock === 'true' ? 'true' : ''].filter(Boolean).length;

  const handleAddToBasket = async (productId) => {
    if (!isAuthenticated) return;
    setAddingToBasket(prev => ({ ...prev, [productId]: true }));
    try {
      await basketApi.addItem({ productId, quantity: 1 });
      dispatch(fetchBasketCount());
      setAddedToBasket(prev => ({ ...prev, [productId]: true }));
      setTimeout(() => {
        setAddedToBasket(prev => ({ ...prev, [productId]: false }));
      }, 2000);
    } catch (err) {
      setError(err.message || 'Failed to add item to basket');
    } finally {
      setAddingToBasket(prev => ({ ...prev, [productId]: false }));
    }
  };

  const getImageUrl = (imagePath) => {
    if (!imagePath) return null;
    return `${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5176'}/${imagePath.startsWith('/') ? imagePath.slice(1) : imagePath}`;
  };

  const getCategoryName = (id) => {
    const cat = categories.find(c => c.id === parseInt(id));
    return cat?.name || '';
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-white">
      <Header />

      {/* Page Header */}
      <div className="bg-white shadow-sm border-b border-gray-200 pt-16">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">Catalog</h1>
              <p className="mt-1 text-sm text-gray-600">
                Browse and discover products
                {metadata && ` - ${metadata.totalCount} product${metadata.totalCount !== 1 ? 's' : ''} found`}
              </p>
            </div>

            {/* Search Bar */}
            <div className="relative w-full sm:w-80">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Search products..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="pl-10 pr-10"
              />
              {search && (
                <button
                  onClick={() => setSearch('')}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                >
                  <X className="h-4 w-4" />
                </button>
              )}
            </div>
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Filter & Sort Bar */}
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
          <div className="flex items-center gap-3">
            <Button
              variant="outline"
              onClick={() => setFiltersOpen(!filtersOpen)}
              className="relative"
            >
              <SlidersHorizontal className="h-4 w-4 mr-2" />
              Filters
              {activeFilterCount > 0 && (
                <span className="ml-2 inline-flex items-center justify-center h-5 w-5 rounded-full bg-primary-600 text-white text-xs">
                  {activeFilterCount}
                </span>
              )}
            </Button>
            {hasActiveFilters && (
              <Button variant="ghost" size="sm" onClick={clearFilters}>
                <X className="h-4 w-4 mr-1" />
                Clear all
              </Button>
            )}
          </div>

          <Select value={getCurrentSortValue()} onValueChange={handleSortChange}>
            <SelectTrigger className="w-48">
              <SelectValue placeholder="Sort by" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="newest">Newest First</SelectItem>
              <SelectItem value="oldest">Oldest First</SelectItem>
              <SelectItem value="price-low">Price: Low to High</SelectItem>
              <SelectItem value="price-high">Price: High to Low</SelectItem>
              <SelectItem value="name-az">Name: A to Z</SelectItem>
              <SelectItem value="name-za">Name: Z to A</SelectItem>
            </SelectContent>
          </Select>
        </div>

        {/* Active Filter Badges */}
        {hasActiveFilters && (
          <div className="flex flex-wrap gap-2 mb-6">
            {debouncedSearch && (
              <span className="inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm bg-primary-100 text-primary-800">
                Search: "{debouncedSearch}"
                <button onClick={() => { setSearch(''); setDebouncedSearch(''); }} className="hover:text-primary-600">
                  <X className="h-3 w-3" />
                </button>
              </span>
            )}
            {categoryId && (
              <span className="inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm bg-primary-100 text-primary-800">
                Category: {getCategoryName(categoryId)}
                <button onClick={() => { setCategoryId(''); setPageNumber(1); }} className="hover:text-primary-600">
                  <X className="h-3 w-3" />
                </button>
              </span>
            )}
            {minPrice !== '' && (
              <span className="inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm bg-primary-100 text-primary-800">
                Min: ${minPrice}
                <button onClick={() => { setMinPrice(''); setPageNumber(1); }} className="hover:text-primary-600">
                  <X className="h-3 w-3" />
                </button>
              </span>
            )}
            {maxPrice !== '' && (
              <span className="inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm bg-primary-100 text-primary-800">
                Max: ${maxPrice}
                <button onClick={() => { setMaxPrice(''); setPageNumber(1); }} className="hover:text-primary-600">
                  <X className="h-3 w-3" />
                </button>
              </span>
            )}
            {inStock === 'true' && (
              <span className="inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm bg-green-100 text-green-800">
                In Stock Only
                <button onClick={() => { setInStock(''); setPageNumber(1); }} className="hover:text-green-600">
                  <X className="h-3 w-3" />
                </button>
              </span>
            )}
          </div>
        )}

        {/* Expandable Filters Panel */}
        {filtersOpen && (
          <Card className="mb-6">
            <CardContent className="pt-6">
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                {/* Category Filter */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Category</label>
                  <Select
                    value={categoryId || 'all'}
                    onValueChange={(value) => { setCategoryId(value === 'all' ? '' : value); setPageNumber(1); }}
                  >
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder="All Categories" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="all">All Categories</SelectItem>
                      {categories.map((cat) => (
                        <SelectItem key={cat.id} value={cat.id.toString()}>
                          {cat.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                {/* Min Price */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Min Price</label>
                  <Input
                    type="number"
                    min="0"
                    step="0.01"
                    placeholder="0.00"
                    value={minPrice}
                    onChange={(e) => { setMinPrice(e.target.value); setPageNumber(1); }}
                  />
                </div>

                {/* Max Price */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Max Price</label>
                  <Input
                    type="number"
                    min="0"
                    step="0.01"
                    placeholder="0.00"
                    value={maxPrice}
                    onChange={(e) => { setMaxPrice(e.target.value); setPageNumber(1); }}
                  />
                </div>

                {/* In Stock Toggle */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Availability</label>
                  <Select
                    value={inStock || 'all'}
                    onValueChange={(value) => { setInStock(value === 'all' ? '' : value); setPageNumber(1); }}
                  >
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder="All Products" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="all">All Products</SelectItem>
                      <SelectItem value="true">In Stock Only</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>
            </CardContent>
          </Card>
        )}

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
            <h3 className="mt-2 text-sm font-medium text-gray-900">No products found</h3>
            <p className="mt-1 text-sm text-gray-500">
              {hasActiveFilters
                ? 'Try adjusting your filters or search term.'
                : 'No products are available at the moment.'}
            </p>
            {hasActiveFilters && (
              <div className="mt-4">
                <Button variant="outline" onClick={clearFilters}>
                  Clear all filters
                </Button>
              </div>
            )}
          </div>
        ) : (
          <>
            <div className={`grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 ${isLoading ? 'opacity-50 pointer-events-none' : ''}`}>
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
                      {isAuthenticated && product.stock > 0 && (
                        <Button
                          size="sm"
                          variant={addedToBasket[product.id] ? 'outline' : 'default'}
                          className={addedToBasket[product.id] ? 'bg-green-50 text-green-700 border-green-300' : ''}
                          onClick={() => handleAddToBasket(product.id)}
                          disabled={addingToBasket[product.id]}
                        >
                          {addedToBasket[product.id] ? (
                            <>
                              <Check className="h-4 w-4 mr-1" />
                              Added
                            </>
                          ) : addingToBasket[product.id] ? (
                            <span className="inline-block animate-spin rounded-full h-4 w-4 border-b-2 border-white"></span>
                          ) : (
                            <>
                              <ShoppingCart className="h-4 w-4 mr-1" />
                              Add
                            </>
                          )}
                        </Button>
                      )}
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>

            {/* Pagination */}
            {metadata && metadata.totalPages > 1 && (
              <div className="mt-8 flex flex-col sm:flex-row items-center justify-between gap-4">
                <div className="text-sm text-gray-700">
                  Showing {((pageNumber - 1) * pageSize) + 1} to {Math.min(pageNumber * pageSize, metadata.totalCount)} of{' '}
                  {metadata.totalCount} products
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

                  {/* Page Numbers */}
                  <div className="hidden sm:flex items-center space-x-1">
                    {Array.from({ length: metadata.totalPages }, (_, i) => i + 1)
                      .filter(page => {
                        // Show first, last, and pages around current
                        return page === 1 || page === metadata.totalPages || Math.abs(page - pageNumber) <= 1;
                      })
                      .reduce((acc, page, idx, arr) => {
                        // Add ellipsis markers between non-consecutive pages
                        if (idx > 0 && page - arr[idx - 1] > 1) {
                          acc.push({ type: 'ellipsis', key: `ellipsis-${page}` });
                        }
                        acc.push({ type: 'page', page, key: `page-${page}` });
                        return acc;
                      }, [])
                      .map((item) => {
                        if (item.type === 'ellipsis') {
                          return (
                            <span key={item.key} className="px-2 text-gray-400">
                              ...
                            </span>
                          );
                        }
                        return (
                          <Button
                            key={item.key}
                            variant={item.page === pageNumber ? 'default' : 'outline'}
                            size="sm"
                            className="h-8 w-8 p-0"
                            onClick={() => setPageNumber(item.page)}
                            disabled={isLoading}
                          >
                            {item.page}
                          </Button>
                        );
                      })}
                  </div>

                  <div className="sm:hidden text-sm text-gray-700">
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
          </>
        )}
      </div>
    </div>
  );
};

export default Catalog;
