
import './App.css';

import { Home } from './Pages/Home.jsx'
import { Movies } from './Pages/Movies.jsx'
import { Bookings } from './Pages/Bookings.jsx'

import { ChartStat } from './Pages/ChartStat.jsx'
import { About } from './Pages/About.jsx'
import { Gmail } from './Pages/Email.jsx'

import { useAuth,AuthProvider } from './AuthContext.jsx'
import { LoginPage }  from './Pages/LoginPage.jsx'

import ProtectedRoute from './ProtectedRoute.jsx'




// import React router
import {
    BrowserRouter as Router,
    Routes,
    Route,
    NavLink
} from 'react-router-dom';


function NavItem({ label, destination }) {
    return (
        <NavLink
            to={destination}
            className={({ isActive }) =>
                isActive ? "active" : ""
            }
        >
            {label}
        </NavLink>)
}

function App() {


    return (
        <AuthProvider>
        <Router>
            

            {/* Navigation bar with clickable links */ }
            <div>
                <NavBar></NavBar>
                
                { /* We define our routes here and pages*/ }
                
                    <main>
                    <Routes>
                        
                        <Route path="/" element={<Home />} />
                     
                            <Route path="/gmail" element={<ProtectedRoute roles={["Admin", "Staff"]} />} >
                                <Route path="/gmail" element={<Gmail />} />
                            </Route>
                            <Route path="/about" element={<About />} />
                          <Route path="/movies" element={<Movies />} />
                          <Route path="/bookings" element={<Bookings />} />
                        <Route path="/login" element={<LoginPage />} />
                        
                        
                        <Route path="/chart" element={<ProtectedRoute roles={["Admin", "Staff"]} />} >
                                <Route path="/chart" element={<ChartStat />} />
                        </Route>
                        
                    </Routes>
                    </main>
                       
                
                
                
                </div>
                
            </Router>
        </AuthProvider>
    );
}




function NavBar() {
    const { user } = useAuth();

    var getRoles = [];

    // collect roles from logged in user
    if (user) {
        getRoles = user.roles

    }

    //  roles: Array of role names
    function hasRequiredRole(roles, getRoles) {
        if (!roles) return true; // If no roles are required, grant access by default
        return roles.some(role => getRoles.includes(role)); // Check if user has at least one of the required roles
    };

    return(
    <nav className="navbar">
        <ul>

            <li>
                <NavItem label='Home' destination="/" />
            </li>

            <li>
                <NavItem label='About' destination="/about" />
            </li>

            <li>
                <NavItem label='Movies' destination="/movies" />
            </li>

            {hasRequiredRole(["Admin"], getRoles) ?
                    <a href="/staff/role/Index" style={{ textDecoration: 'none' }}>
                        ManageRoles
                    </a>
                    : null
            }

            {hasRequiredRole(["Admin"], getRoles) ?
                    <a href="/staff/Sessiontimes/Index" style={{ textDecoration: 'none' }}>
                        ManageSessions
                    </a> : null
            }

            {hasRequiredRole(["Admin"], getRoles) ?
            <li>
                <NavItem label='Chart' destination="/chart" />
            </li> : null
                }

            {hasRequiredRole(["Admin"], getRoles) ?
            <li>
                <NavItem label='Gmail' destination="/gmail" />
            </li> : null
            }



            <li>
                <NavItem label='Login/Logout' destination="/login" />
            </li>


        </ul>
        </nav>
    )
}

export { App
    };


