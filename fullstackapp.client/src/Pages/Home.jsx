
import React, { createContext, useState, useEffect, useContext } from 'react';
import axios from 'axios';
import { useAuth } from '../AuthContext.jsx';

function Home() {
    
    const { user, setUser, loading, setLoading, login } = useAuth();

    
    useEffect(() => {
        const verifyLogin = async () => {
            if (user == null) {

                const resp = await handleLogin(setUser,setLoading)
                if (resp) {
                // pass
                }
                else { // no user found
                    console.log('login2')
                    window.location.href = '/login';
                    
                }
                
            }
        }
        verifyLogin();
    }, [user]);

    
    if (loading) {
        return <div>Loading</div>; 
    }


    const contents = user == null ? <div>No user found</div>
        : 
    <div>
            <h2>Welcome, {user.username?.trim()}!</h2>
            <p>Roles: {user && user.roles?.join(', ')}</p> {/* Display roles */}
            
        </div>
         

    return (
        <div>
            <h2>Home</h2>
            {contents}
        </div>
    );

    

}

async function handleLogin(setUser,setLoading) {

    try {
        // Send login request

        const response = await axios.get('api/LoginDetails/GetUserDetails', { withCredentials: true });
        console.log('Response: ', JSON.stringify(response))


        /*
        const response = {
            'data': {
                "username": "john_doe",
                "email": "john.doe@example.com",
                "roles": ["Admin", "Staff"]
            }
        };
        
        */

        if (response.data) {
            // Store user data
           
            setUser(response.data);
            setLoading(false);
            return true
        }

        else {
            //alert('Login failed');
            console.log("no response from user")
            setLoading(false);
          
            return false
            
        }
    } catch (error) {
        console.log('error')
        console.log(error)
        return false;
        //alert('Error during login');
    }
}


export { Home }