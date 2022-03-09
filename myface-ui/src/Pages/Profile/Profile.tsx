import React, {useContext} from "react";
import { useParams } from 'react-router-dom'; 
import {Page} from "../Page/Page";
import {UserDetails} from "../../Components/UserDetails/UserDetails";
import {PostList} from "../../Components/PostList/PostList";
import {fetchPostsDislikedBy, fetchPostsForUser, fetchPostsLikedBy} from "../../Api/apiClient";
import "./Profile.scss";
import {Users} from "../Users/Users";
import { LoginContext } from "../../Components/LoginManager/LoginManager";

export function Profile(): JSX.Element {
    const {id} = useParams();
    const { username, password } = useContext(LoginContext);
    
    if (id === undefined) {
        // Shouldn't ever happen - but if the ID is somehow undefined, show the base users page.
        return <Users/>
    }
    
    return (
        <Page containerClassName="profile">
            <UserDetails userId={id!}/>
            <div className="activity">
                <PostList title="Posts" fetchPosts={() => fetchPostsForUser(1, 12, id, username as string, password as string)}/>

                <PostList title="Likes" fetchPosts={() => fetchPostsLikedBy(1, 12, id, username as string, password as string )}/>
                <PostList title="Dislikes" fetchPosts={() => fetchPostsDislikedBy(1, 12, id, username as string, password as string)}/>
            </div>
        </Page>
    );
}