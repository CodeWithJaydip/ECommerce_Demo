import { useEffect, useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { ArrowLeft, Plus, Check, MapPin, Package, AlertTriangle, CreditCard, Trash2 } from 'lucide-react';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Label } from '../ui/label';
import * as basketApi from '../../services/api/basketApi';
import * as shippingAddressApi from '../../services/api/shippingAddressApi';
import Header from '../common/Header';

const CheckoutPage = () => {
  const navigate = useNavigate();
  const [summary, setSummary] = useState(null);
  const [addresses, setAddresses] = useState([]);
  const [selectedAddressId, setSelectedAddressId] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showAddressForm, setShowAddressForm] = useState(false);
  const [savingAddress, setSavingAddress] = useState(false);
  const [addressForm, setAddressForm] = useState({
    fullName: '',
    addressLine1: '',
    addressLine2: '',
    city: '',
    state: '',
    postalCode: '',
    country: '',
    phoneNumber: '',
    isDefault: false,
  });
  const [addressErrors, setAddressErrors] = useState({});

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    if (selectedAddressId) {
      loadCheckoutSummary(selectedAddressId);
    }
  }, [selectedAddressId]);

  const loadData = async () => {
    try {
      setError(null);
      const [summaryData, addressData] = await Promise.all([
        basketApi.getCheckoutSummary(null),
        shippingAddressApi.getAddresses(),
      ]);
      setSummary(summaryData);
      setAddresses(addressData || []);

      // Auto-select default address
      const defaultAddr = (addressData || []).find(a => a.isDefault);
      if (defaultAddr) {
        setSelectedAddressId(defaultAddr.id);
      }
    } catch (err) {
      if (err.message?.includes('empty')) {
        navigate('/basket');
        return;
      }
      setError(err.message || 'Failed to load checkout data');
    } finally {
      setIsLoading(false);
    }
  };

  const loadCheckoutSummary = async (addressId) => {
    try {
      const data = await basketApi.getCheckoutSummary(addressId);
      setSummary(data);
    } catch (err) {
      setError(err.message || 'Failed to load checkout summary');
    }
  };

  const getImageUrl = (imagePath) => {
    if (!imagePath) return null;
    return `${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5176'}/${imagePath.startsWith('/') ? imagePath.slice(1) : imagePath}`;
  };

  const validateAddressForm = () => {
    const errors = {};
    if (!addressForm.fullName.trim()) errors.fullName = 'Full name is required';
    if (!addressForm.addressLine1.trim()) errors.addressLine1 = 'Address line 1 is required';
    if (!addressForm.city.trim()) errors.city = 'City is required';
    if (!addressForm.state.trim()) errors.state = 'State is required';
    if (!addressForm.postalCode.trim()) errors.postalCode = 'Postal code is required';
    if (!addressForm.country.trim()) errors.country = 'Country is required';
    setAddressErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSaveAddress = async () => {
    if (!validateAddressForm()) return;
    setSavingAddress(true);
    try {
      const newAddress = await shippingAddressApi.createAddress(addressForm);
      setAddresses(prev => [newAddress, ...prev]);
      setSelectedAddressId(newAddress.id);
      setShowAddressForm(false);
      setAddressForm({
        fullName: '', addressLine1: '', addressLine2: '', city: '',
        state: '', postalCode: '', country: '', phoneNumber: '', isDefault: false,
      });
      setAddressErrors({});
    } catch (err) {
      setError(err.message || 'Failed to save address');
    } finally {
      setSavingAddress(false);
    }
  };

  const handleDeleteAddress = async (id) => {
    try {
      await shippingAddressApi.deleteAddress(id);
      setAddresses(prev => prev.filter(a => a.id !== id));
      if (selectedAddressId === id) {
        setSelectedAddressId(null);
      }
    } catch (err) {
      setError(err.message || 'Failed to delete address');
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-white">
      <Header />

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pt-24 pb-12">
        {/* Back Button */}
        <Button variant="ghost" className="mb-6" onClick={() => navigate('/basket')}>
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back to Basket
        </Button>

        <h1 className="text-3xl font-bold text-gray-900 mb-8">Checkout</h1>

        {error && (
          <div className="mb-6 p-4 bg-destructive/10 border border-destructive/20 rounded-lg">
            <p className="text-sm font-medium text-destructive">{error}</p>
          </div>
        )}

        {isLoading ? (
          <div className="text-center py-12">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
            <p className="mt-4 text-muted-foreground">Loading checkout...</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Left Column: Shipping Address */}
            <div className="lg:col-span-2 space-y-6">
              {/* Shipping Address Section */}
              <Card>
                <CardHeader>
                  <div className="flex items-center justify-between">
                    <CardTitle className="flex items-center gap-2">
                      <MapPin className="h-5 w-5" />
                      Shipping Address
                    </CardTitle>
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => setShowAddressForm(!showAddressForm)}
                    >
                      <Plus className="h-4 w-4 mr-1" />
                      Add New
                    </Button>
                  </div>
                </CardHeader>
                <CardContent className="space-y-4">
                  {/* New Address Form */}
                  {showAddressForm && (
                    <div className="p-4 border border-dashed border-gray-300 rounded-lg space-y-4">
                      <h4 className="font-medium text-gray-900">New Shipping Address</h4>
                      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <div>
                          <Label htmlFor="fullName">Full Name *</Label>
                          <Input
                            id="fullName"
                            value={addressForm.fullName}
                            onChange={(e) => setAddressForm(prev => ({ ...prev, fullName: e.target.value }))}
                            className={addressErrors.fullName ? 'border-destructive' : ''}
                          />
                          {addressErrors.fullName && <p className="text-xs text-destructive mt-1">{addressErrors.fullName}</p>}
                        </div>
                        <div>
                          <Label htmlFor="phoneNumber">Phone Number</Label>
                          <Input
                            id="phoneNumber"
                            value={addressForm.phoneNumber}
                            onChange={(e) => setAddressForm(prev => ({ ...prev, phoneNumber: e.target.value }))}
                          />
                        </div>
                        <div className="sm:col-span-2">
                          <Label htmlFor="addressLine1">Address Line 1 *</Label>
                          <Input
                            id="addressLine1"
                            value={addressForm.addressLine1}
                            onChange={(e) => setAddressForm(prev => ({ ...prev, addressLine1: e.target.value }))}
                            className={addressErrors.addressLine1 ? 'border-destructive' : ''}
                          />
                          {addressErrors.addressLine1 && <p className="text-xs text-destructive mt-1">{addressErrors.addressLine1}</p>}
                        </div>
                        <div className="sm:col-span-2">
                          <Label htmlFor="addressLine2">Address Line 2</Label>
                          <Input
                            id="addressLine2"
                            value={addressForm.addressLine2}
                            onChange={(e) => setAddressForm(prev => ({ ...prev, addressLine2: e.target.value }))}
                          />
                        </div>
                        <div>
                          <Label htmlFor="city">City *</Label>
                          <Input
                            id="city"
                            value={addressForm.city}
                            onChange={(e) => setAddressForm(prev => ({ ...prev, city: e.target.value }))}
                            className={addressErrors.city ? 'border-destructive' : ''}
                          />
                          {addressErrors.city && <p className="text-xs text-destructive mt-1">{addressErrors.city}</p>}
                        </div>
                        <div>
                          <Label htmlFor="state">State *</Label>
                          <Input
                            id="state"
                            value={addressForm.state}
                            onChange={(e) => setAddressForm(prev => ({ ...prev, state: e.target.value }))}
                            className={addressErrors.state ? 'border-destructive' : ''}
                          />
                          {addressErrors.state && <p className="text-xs text-destructive mt-1">{addressErrors.state}</p>}
                        </div>
                        <div>
                          <Label htmlFor="postalCode">Postal Code *</Label>
                          <Input
                            id="postalCode"
                            value={addressForm.postalCode}
                            onChange={(e) => setAddressForm(prev => ({ ...prev, postalCode: e.target.value }))}
                            className={addressErrors.postalCode ? 'border-destructive' : ''}
                          />
                          {addressErrors.postalCode && <p className="text-xs text-destructive mt-1">{addressErrors.postalCode}</p>}
                        </div>
                        <div>
                          <Label htmlFor="country">Country *</Label>
                          <Input
                            id="country"
                            value={addressForm.country}
                            onChange={(e) => setAddressForm(prev => ({ ...prev, country: e.target.value }))}
                            className={addressErrors.country ? 'border-destructive' : ''}
                          />
                          {addressErrors.country && <p className="text-xs text-destructive mt-1">{addressErrors.country}</p>}
                        </div>
                      </div>
                      <div className="flex items-center gap-2">
                        <input
                          type="checkbox"
                          id="isDefault"
                          checked={addressForm.isDefault}
                          onChange={(e) => setAddressForm(prev => ({ ...prev, isDefault: e.target.checked }))}
                          className="h-4 w-4 rounded border-gray-300"
                        />
                        <Label htmlFor="isDefault" className="text-sm font-normal">Set as default address</Label>
                      </div>
                      <div className="flex gap-3">
                        <Button onClick={handleSaveAddress} disabled={savingAddress}>
                          {savingAddress ? 'Saving...' : 'Save Address'}
                        </Button>
                        <Button variant="outline" onClick={() => { setShowAddressForm(false); setAddressErrors({}); }}>
                          Cancel
                        </Button>
                      </div>
                    </div>
                  )}

                  {/* Saved Addresses */}
                  {addresses.length === 0 && !showAddressForm ? (
                    <div className="text-center py-8 text-gray-500">
                      <MapPin className="mx-auto h-8 w-8 text-gray-300 mb-2" />
                      <p className="text-sm">No saved addresses. Add one to continue.</p>
                    </div>
                  ) : (
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                      {addresses.map((address) => (
                        <div
                          key={address.id}
                          onClick={() => setSelectedAddressId(address.id)}
                          className={`relative p-4 border-2 rounded-lg cursor-pointer transition-colors ${
                            selectedAddressId === address.id
                              ? 'border-primary-600 bg-primary-50'
                              : 'border-gray-200 hover:border-gray-300'
                          }`}
                        >
                          {selectedAddressId === address.id && (
                            <div className="absolute top-2 right-2 h-5 w-5 rounded-full bg-primary-600 flex items-center justify-center">
                              <Check className="h-3 w-3 text-white" />
                            </div>
                          )}
                          <p className="font-medium text-sm">{address.fullName}</p>
                          <p className="text-xs text-gray-600 mt-1">{address.addressLine1}</p>
                          {address.addressLine2 && <p className="text-xs text-gray-600">{address.addressLine2}</p>}
                          <p className="text-xs text-gray-600">{address.city}, {address.state} {address.postalCode}</p>
                          <p className="text-xs text-gray-600">{address.country}</p>
                          {address.phoneNumber && <p className="text-xs text-gray-500 mt-1">{address.phoneNumber}</p>}
                          <div className="flex items-center gap-2 mt-2">
                            {address.isDefault && (
                              <span className="text-xs px-2 py-0.5 bg-primary-100 text-primary-700 rounded-full">Default</span>
                            )}
                            <button
                              onClick={(e) => { e.stopPropagation(); handleDeleteAddress(address.id); }}
                              className="text-xs text-gray-400 hover:text-destructive flex items-center gap-1"
                            >
                              <Trash2 className="h-3 w-3" />
                              Remove
                            </button>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </CardContent>
              </Card>

              {/* Order Items */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Package className="h-5 w-5" />
                    Order Items ({summary?.basket?.totalItems || 0})
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="divide-y">
                    {summary?.basket?.items?.map((item) => (
                      <div key={item.id} className="flex items-center gap-4 py-3">
                        <div className="w-14 h-14 flex-shrink-0 rounded-lg overflow-hidden bg-gray-100">
                          {getImageUrl(item.productImagePath) ? (
                            <img src={getImageUrl(item.productImagePath)} alt={item.productName} className="w-full h-full object-cover" />
                          ) : (
                            <div className="w-full h-full flex items-center justify-center">
                              <Package className="h-6 w-6 text-gray-400" />
                            </div>
                          )}
                        </div>
                        <div className="flex-1 min-w-0">
                          <p className="text-sm font-medium text-gray-900 truncate">{item.productName}</p>
                          <p className="text-xs text-gray-500">Qty: {item.quantity} x ${item.currentPrice.toFixed(2)}</p>
                          {!item.isAvailable && (
                            <p className="text-xs text-destructive flex items-center gap-1 mt-0.5">
                              <AlertTriangle className="h-3 w-3" /> Unavailable
                            </p>
                          )}
                        </div>
                        <p className="text-sm font-semibold">${item.subTotal.toFixed(2)}</p>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Right Column: Order Summary */}
            <div className="lg:col-span-1">
              <Card className="sticky top-24">
                <CardHeader>
                  <CardTitle>Order Summary</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Subtotal</span>
                    <span className="font-medium">${(summary?.subTotal || 0).toFixed(2)}</span>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Shipping</span>
                    <span className="font-medium text-green-600">
                      {summary?.shippingCost === 0 ? 'Free' : `$${(summary?.shippingCost || 0).toFixed(2)}`}
                    </span>
                  </div>
                  <hr />
                  <div className="flex justify-between text-lg font-bold">
                    <span>Total</span>
                    <span>${(summary?.totalAmount || 0).toFixed(2)}</span>
                  </div>

                  {summary?.hasStockIssues && (
                    <div className="p-3 bg-amber-50 border border-amber-200 rounded-lg">
                      <div className="flex items-center gap-2 text-amber-800 text-sm">
                        <AlertTriangle className="h-4 w-4 flex-shrink-0" />
                        <span>Some items have stock issues. Please go back and review your basket.</span>
                      </div>
                    </div>
                  )}

                  {!selectedAddressId && (
                    <div className="p-3 bg-blue-50 border border-blue-200 rounded-lg">
                      <p className="text-sm text-blue-800">Please select or add a shipping address to continue.</p>
                    </div>
                  )}

                  <Button
                    className="w-full"
                    size="lg"
                    disabled={!selectedAddressId || summary?.hasStockIssues}
                  >
                    <CreditCard className="h-4 w-4 mr-2" />
                    Place Order (Coming Soon)
                  </Button>

                  <p className="text-xs text-center text-gray-500">
                    Payment integration will be available soon.
                  </p>

                  <Link to="/basket" className="block">
                    <Button variant="outline" className="w-full">
                      <ArrowLeft className="h-4 w-4 mr-2" />
                      Back to Basket
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

export default CheckoutPage;
