import { useEffect, useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { ShoppingCart, Trash2, Minus, Plus, Package, AlertTriangle, ArrowRight, ShoppingBag } from 'lucide-react';
import { Button } from '../ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from '../ui/alert-dialog';
import * as basketApi from '../../services/api/basketApi';
import { useAppDispatch } from '../../hooks/redux';
import { fetchBasketCount } from '../../store/slices/basketSlice';
import Header from '../common/Header';

const BasketPage = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [basket, setBasket] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [updatingItems, setUpdatingItems] = useState({});

  const fetchBasket = async () => {
    try {
      setError(null);
      const data = await basketApi.getBasket();
      setBasket(data);
    } catch (err) {
      setError(err.message || 'Failed to load basket');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchBasket();
  }, []);

  const getImageUrl = (imagePath) => {
    if (!imagePath) return null;
    return `${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5176'}/${imagePath.startsWith('/') ? imagePath.slice(1) : imagePath}`;
  };

  const handleUpdateQuantity = async (productId, newQuantity) => {
    if (newQuantity < 1 || newQuantity > 99) return;
    setUpdatingItems(prev => ({ ...prev, [productId]: true }));
    try {
      const data = await basketApi.updateItem(productId, { quantity: newQuantity });
      setBasket(data);
      dispatch(fetchBasketCount());
    } catch (err) {
      setError(err.message || 'Failed to update quantity');
    } finally {
      setUpdatingItems(prev => ({ ...prev, [productId]: false }));
    }
  };

  const handleRemoveItem = async (productId) => {
    setUpdatingItems(prev => ({ ...prev, [productId]: true }));
    try {
      await basketApi.removeItem(productId);
      await fetchBasket();
      dispatch(fetchBasketCount());
    } catch (err) {
      setError(err.message || 'Failed to remove item');
    } finally {
      setUpdatingItems(prev => ({ ...prev, [productId]: false }));
    }
  };

  const handleClearBasket = async () => {
    try {
      await basketApi.clearBasket();
      setBasket(null);
      dispatch(fetchBasketCount());
    } catch (err) {
      setError(err.message || 'Failed to clear basket');
    }
  };

  const hasStockIssues = basket?.items?.some(item => !item.isAvailable || item.quantity > item.stock);

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-white">
      <Header />

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pt-24 pb-12">
        <div className="flex items-center justify-between mb-8">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Shopping Basket</h1>
            <p className="mt-1 text-sm text-gray-600">
              {basket?.totalItems ? `${basket.totalItems} item${basket.totalItems !== 1 ? 's' : ''} in your basket` : 'Your basket is empty'}
            </p>
          </div>
          {basket?.items?.length > 0 && (
            <AlertDialog>
              <AlertDialogTrigger asChild>
                <Button variant="outline" className="text-destructive border-destructive/30 hover:bg-destructive/10">
                  <Trash2 className="h-4 w-4 mr-2" />
                  Clear Basket
                </Button>
              </AlertDialogTrigger>
              <AlertDialogContent>
                <AlertDialogHeader>
                  <AlertDialogTitle>Clear Basket?</AlertDialogTitle>
                  <AlertDialogDescription>
                    This will remove all items from your basket. This action cannot be undone.
                  </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                  <AlertDialogCancel>Cancel</AlertDialogCancel>
                  <AlertDialogAction onClick={handleClearBasket} className="bg-destructive text-destructive-foreground hover:bg-destructive/90">
                    Clear All
                  </AlertDialogAction>
                </AlertDialogFooter>
              </AlertDialogContent>
            </AlertDialog>
          )}
        </div>

        {error && (
          <div className="mb-6 p-4 bg-destructive/10 border border-destructive/20 rounded-lg">
            <p className="text-sm font-medium text-destructive">{error}</p>
          </div>
        )}

        {isLoading ? (
          <div className="text-center py-12">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
            <p className="mt-4 text-muted-foreground">Loading basket...</p>
          </div>
        ) : !basket?.items?.length ? (
          <div className="text-center py-16">
            <ShoppingCart className="mx-auto h-16 w-16 text-gray-300" />
            <h3 className="mt-4 text-lg font-medium text-gray-900">Your basket is empty</h3>
            <p className="mt-2 text-sm text-gray-500">Start shopping and add items to your basket.</p>
            <div className="mt-6">
              <Link to="/catalog">
                <Button>
                  <ShoppingBag className="h-4 w-4 mr-2" />
                  Browse Catalog
                </Button>
              </Link>
            </div>
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Basket Items */}
            <div className="lg:col-span-2 space-y-4">
              {basket.items.map((item) => (
                <Card key={item.id} className={`overflow-hidden ${!item.isAvailable ? 'border-destructive/30 bg-destructive/5' : ''}`}>
                  <CardContent className="p-4">
                    <div className="flex gap-4">
                      {/* Product Image */}
                      <div className="w-24 h-24 flex-shrink-0 rounded-lg overflow-hidden bg-gray-100">
                        {getImageUrl(item.productImagePath) ? (
                          <img
                            src={getImageUrl(item.productImagePath)}
                            alt={item.productName}
                            className="w-full h-full object-cover"
                            onError={(e) => {
                              e.target.style.display = 'none';
                              e.target.nextSibling.style.display = 'flex';
                            }}
                          />
                        ) : null}
                        <div className={`w-full h-full flex items-center justify-center ${getImageUrl(item.productImagePath) ? 'hidden' : ''}`}>
                          <Package className="h-8 w-8 text-gray-400" />
                        </div>
                      </div>

                      {/* Product Details */}
                      <div className="flex-1 min-w-0">
                        <div className="flex justify-between items-start">
                          <div>
                            <h3 className="text-base font-semibold text-gray-900 truncate">{item.productName}</h3>
                            <p className="text-sm text-gray-500 mt-0.5">${item.currentPrice.toFixed(2)} each</p>
                            {item.unitPrice !== item.currentPrice && (
                              <p className="text-xs text-amber-600 mt-0.5">
                                Price changed since added (was ${item.unitPrice.toFixed(2)})
                              </p>
                            )}
                          </div>
                          <p className="text-lg font-bold text-gray-900">${item.subTotal.toFixed(2)}</p>
                        </div>

                        {/* Stock Warning */}
                        {!item.isAvailable && (
                          <div className="flex items-center gap-1 mt-2 text-destructive text-sm">
                            <AlertTriangle className="h-4 w-4" />
                            <span>This product is no longer available</span>
                          </div>
                        )}
                        {item.isAvailable && item.quantity > item.stock && (
                          <div className="flex items-center gap-1 mt-2 text-amber-600 text-sm">
                            <AlertTriangle className="h-4 w-4" />
                            <span>Only {item.stock} available in stock</span>
                          </div>
                        )}

                        {/* Quantity Controls */}
                        <div className="flex items-center justify-between mt-3">
                          <div className="flex items-center gap-2">
                            <Button
                              variant="outline"
                              size="icon"
                              className="h-8 w-8"
                              onClick={() => handleUpdateQuantity(item.productId, item.quantity - 1)}
                              disabled={item.quantity <= 1 || updatingItems[item.productId]}
                            >
                              <Minus className="h-3 w-3" />
                            </Button>
                            <span className="w-10 text-center text-sm font-medium">{item.quantity}</span>
                            <Button
                              variant="outline"
                              size="icon"
                              className="h-8 w-8"
                              onClick={() => handleUpdateQuantity(item.productId, item.quantity + 1)}
                              disabled={item.quantity >= 99 || item.quantity >= item.stock || updatingItems[item.productId]}
                            >
                              <Plus className="h-3 w-3" />
                            </Button>
                          </div>
                          <Button
                            variant="ghost"
                            size="sm"
                            className="text-destructive hover:text-destructive hover:bg-destructive/10"
                            onClick={() => handleRemoveItem(item.productId)}
                            disabled={updatingItems[item.productId]}
                          >
                            <Trash2 className="h-4 w-4 mr-1" />
                            Remove
                          </Button>
                        </div>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>

            {/* Order Summary Sidebar */}
            <div className="lg:col-span-1">
              <Card className="sticky top-24">
                <CardHeader>
                  <CardTitle>Order Summary</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Subtotal ({basket.totalItems} items)</span>
                    <span className="font-medium">${basket.totalAmount.toFixed(2)}</span>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Shipping</span>
                    <span className="font-medium text-green-600">Free</span>
                  </div>
                  <hr />
                  <div className="flex justify-between text-lg font-bold">
                    <span>Total</span>
                    <span>${basket.totalAmount.toFixed(2)}</span>
                  </div>

                  {hasStockIssues && (
                    <div className="p-3 bg-amber-50 border border-amber-200 rounded-lg">
                      <div className="flex items-center gap-2 text-amber-800 text-sm">
                        <AlertTriangle className="h-4 w-4 flex-shrink-0" />
                        <span>Some items have stock issues. Please review before checkout.</span>
                      </div>
                    </div>
                  )}

                  <Button
                    className="w-full"
                    size="lg"
                    onClick={() => navigate('/checkout')}
                    disabled={hasStockIssues}
                  >
                    Proceed to Checkout
                    <ArrowRight className="h-4 w-4 ml-2" />
                  </Button>

                  <Link to="/catalog" className="block">
                    <Button variant="outline" className="w-full">
                      Continue Shopping
                    </Button>
                  </Link>
                </CardContent>
              </Card>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default BasketPage;
