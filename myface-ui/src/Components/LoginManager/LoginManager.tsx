import React, {createContext, ReactNode, useState} from "react";

export const LoginContext = createContext<
{
    isLoggedIn: boolean,
    isAdmin: boolean,
    logIn: (username: string, password: string) => void,
    logOut: () => void,
    username: string | undefined,
    password: string | undefined,
}
>({
    isLoggedIn: false,
    isAdmin: false,
    logIn: () => {},
    logOut: () => {},
    username: undefined,
    password: undefined,
});

interface LoginManagerProps {
    children: ReactNode
}

export function LoginManager(props: LoginManagerProps): JSX.Element {
    const [loggedIn, setLoggedIn] = useState(false);
    const [username, setUsername] = useState<string>();
    const [password, setPassword] = useState<string>();
    
    function logIn(username: string, password: string) {
        setLoggedIn(true);
        setUsername(username);
        setPassword(password);
    }
    
    function logOut() {
        setLoggedIn(false);
    }
    
    const context = {
        isLoggedIn: loggedIn,
        isAdmin: loggedIn,
        logIn: logIn,
        logOut: logOut,
        username: username,
        password: password,
    };
    
    return (
        <LoginContext.Provider value={context}>
            {props.children}
        </LoginContext.Provider>
    );
}

/*
'{  isLoggedIn: boolean;
    isAdmin: boolean; 
    logIn: (username: string, password: string) => void; 
    logOut: () => void;
    username: string | undefined; 
    password: string | undefined; }'

 is not assignable to type '
 {  isLoggedIn: false; 
    isAdmin: false; 
    logIn: (username: string, password: string) => void; 
    logOut: () => void; 
    username: string | undefined; 
    password: string | undefined; }'.
*/