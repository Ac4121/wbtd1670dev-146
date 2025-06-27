import React from 'react';
import { Navigate, Outlet, Route } from 'react-router-dom';
import { useAuth } from './AuthContext';
import { LoginPage } from './Pages/LoginPage';
import { Unauthorized } from './Unauthorized';
import { Home } from './Pages/Home';

/*
Protects the routes with user authorisation and role-based access

element: Component function
roles: The roles allowed to access the element

*/
const ProtectedRoute = ({ element, roles, ...rest }) => {
    const { user } = useAuth();

    if (!user || user === null) {
        return <Navigate to='/' element={<Home />} />
    }

    { console.log("User: ", user) }

    if (user) {
        var getRoles = user.roles


        const hasRequiredRole = (roles, getRoles) => {
            if (!roles) return true; // If no roles are required, grant access by default
            return roles.some(role => getRoles.includes(role)); // Check if user has at least one of the required roles
        };

        // Optionally check roles if your app uses role-based access
        if (roles && roles.length > 0 && !hasRequiredRole) {
            return <Navigate to="/unauthorized" element={<Unauthorized />} />;
        }


        return (<Outlet /> )
    }
    
}


export default ProtectedRoute;
