import { useAppSelector } from '../../hooks/redux';
import Header from '../common/Header';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';

const Dashboard = () => {
  const { user } = useAppSelector((state) => state.auth);

  return (
    <div className="min-h-screen bg-gradient-to-br from-primary-50 to-primary-100">
      <Header />
      
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 pt-28">
        <Card className="shadow-xl">
          <CardHeader>
            <CardTitle className="text-2xl">Dashboard</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <Card className="bg-primary-50">
                <CardHeader>
                  <CardTitle className="text-lg">User Information</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-2 text-gray-700">
                    <p><span className="font-medium">Name:</span> {user?.firstName} {user?.lastName}</p>
                    <p><span className="font-medium">Email:</span> {user?.email}</p>
                    {user?.phoneNumber && (
                      <p><span className="font-medium">Phone:</span> {user?.phoneNumber}</p>
                    )}
                    <p><span className="font-medium">Roles:</span> {user?.roles?.join(', ') || 'User'}</p>
                  </div>
                </CardContent>
              </Card>

              <Card className="bg-gray-50">
                <CardHeader>
                  <CardTitle className="text-lg">Quick Actions</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    <Button variant="outline" className="w-full justify-start">
                      View Profile
                    </Button>
                    <Button variant="outline" className="w-full justify-start">
                      Settings
                    </Button>
                    <Button variant="outline" className="w-full justify-start">
                      Orders
                    </Button>
                  </div>
                </CardContent>
              </Card>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default Dashboard;
