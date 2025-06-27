import { useAuth } from '../AuthContext.jsx';

function LoginPage() {

    const { user } = useAuth();

    const contents = user == null ?
    <div>
        <h2>Welcome</h2>
        <p>User not logged in</p>
        <br></br>
    </div>
        
        :
        <div>
            <h2>Welcome, {user.username?.trim()}!</h2>
            <br/>
        </div>

    return (
        <div>
        
            {contents}

            <button onClick={() => window.location.href = "Identity/Account/Login"} value="">Sign in</button>
            <br />
            <br />
            <button onClick={() => window.location.href = "Identity/Account/Logout"} value="">Sign out</button>

         
        </div>
    );
};

export { LoginPage };
