import React, {useContext, useEffect, useState} from 'react';
import {fetchUser, User} from "../../Api/apiClient";
import { LoginContext } from '../LoginManager/LoginManager';
import "./UserDetails.scss";

interface UserDetailsProps {
    userId: string;
}

export function UserDetails(props: UserDetailsProps): JSX.Element {
    const [user, setUser] = useState<User | null>(null);
    const {username, password} = useContext(LoginContext);
    
    useEffect(() => {
        fetchUser(props.userId, username as string, password as string)
            .then(response => setUser(response));
    }, [props, username, password]);
    
    if (!user) {
        return <section>Loading...</section>
    }
    
    return (
        <section className="user-details">
            <img className="cover-image" src={user.coverImageUrl} alt="A cover image for the user."/>
            <div className="user-info">
                <img className="profile-image" src={user.profileImageUrl} alt=""/>
                <div className="contact-info">
                    <h1 className="title">{user.displayName}</h1>
                    <div className="username">{user.username}</div>
                    <div className="email">{user.email}</div>
                </div>
            </div>
        </section>
    );
}