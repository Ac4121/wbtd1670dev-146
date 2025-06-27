import React, { createContext, useState, useEffect, useContext } from 'react';

const AuthContext = createContext();

export const useAuth = () => {
    return useContext(AuthContext);
};

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);

    const [movie, setMovie] = useState({
        "id": 3,
        "title": "Planet Earth 3",
        "description": " Explore the world's animals and natural wonders with David Attenborough.",
        "genres": "Nature Documentary",
        "ticketsBoughtTotal": 0,
        "esrbRating": "PG",
        "runtime": "2 hours",
        "movieImageFilename": "planet_earth_3.jpg"

    });
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        
        const checkAuthStatus = async () => {
            try {
               

                if (user) {
                }
                else {
                    setUser(null);  // Set user to null if not authenticated
                }
            } catch (error) {
                setUser(null);  // If the request fails, assume not authenticated
            } finally {
                setLoading(false);
            }
        };

        checkAuthStatus();
    }, []);

    const login = (userData) => {
        setUser(userData);
    };

    const logout = () => {
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, setUser, login, logout, loading, setLoading, movie, setMovie }}>
            {children}
        </AuthContext.Provider>
    );
};
