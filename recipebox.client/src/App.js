import React, { Fragment, useEffect } from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Navbar from './components/layout/Navbar';
import Landing from './components/layout/Landing';
import Register from './components/auth/Register';
import PasswordReset from './components/auth/PasswordReset';
import Login from './components/auth/Login';
import ForgotPassword from './components/email/ForgotPassword';
import EmailConfirmed from './components/email/EmailConfirmed';
import Post from './components/post/Post';
import Posts from './components/posts/Posts';
import Cuisines from './components/posts/Cuisines';
import PostForm from './components/post/PostForm';
import PhotosToPost from './components/post/PhotosToPost';
import Alert from './components/layout/Alert';
import UserProfile from './components/user/UserProfile';
import UserFavourites from './components/user/UserFavourites';
import UserPosts from './components/user/UserPosts';
import EditPost from './components/post/EditPost';
import EditPhotos from './components/post/EditPhotos';
import PrivateRoute from './components/routing/PrivateRoute';

//Redux
import { Provider } from 'react-redux';
import store from './store';
import { loadUser } from './actions/auth';
import setAuthToken from './utils/setAuthToken';

import './App.css';
// import 'bootstrap/dist/css/bootstrap.min.css';
// import Container from 'react-bootstrap/Container';

if (localStorage.token) {
	setAuthToken(localStorage.token);
}

const App = () => {
	useEffect(() => {
		store.dispatch(loadUser());
	}, []);

	return (
		<Provider store={store}>
			<Router>
				<Fragment>
					<Navbar />
					<Route exact path='/' component={Landing} />
					<section className='container'>
						<Alert />
						<Switch>
							<Route exact path='/register' component={Register} />
							<Route exact path='/login' component={Login} />
							<Route exact path='/posts' component={Posts} />
							<Route exact path='/cuisines' component={Cuisines} />
							<Route exact path='/posts/:id' component={Post} />
							<Route exact path='/password-reset' component={ForgotPassword} />
							<Route exact path='/email-confirmed' component={EmailConfirmed} />
							<Route exact path='/reset-password' component={PasswordReset} />
							<Route exact path='/submit-post' component={PostForm} />
							<PrivateRoute exact path='/posts/:id/edit' component={EditPost} />
							<PrivateRoute exact path='/posts/:id/edit/photos' component={EditPhotos} />
							<Route exact path='/post/add-photos' component={PhotosToPost} />
							<Route exact path='/users/:id' component={UserProfile} />
							<Route exact path='/users/:id/posts' component={UserPosts} />
							<Route exact path='/users/:id/favourites' component={UserFavourites} />
							{/* <Route
								exact
								path='reset-password?email={email}&token={validToken}'
								component={PasswordReset}
							/> */}
						</Switch>
					</section>
				</Fragment>
			</Router>
		</Provider>
	);
};

export default App;
