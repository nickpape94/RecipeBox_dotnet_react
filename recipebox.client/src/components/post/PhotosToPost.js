import React, { Fragment, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Redirect } from 'react-router-dom';
import { connect } from 'react-redux';
import { addRecipePhotos } from '../../actions/photo';
import { getPost } from '../../actions/post';
import { getUser } from '../../actions/user';
import Spinner from '../layout/Spinner';

const PhotosToPost = ({ getPost, post: { post }, match, auth: { loading, isAuthenticated, user } }) => {
	useEffect(
		() => {
			getPost(match.params.postId);
		},
		[ getPost ]
	);

	const idOfPost = post && post.userId;
	const idOfLoggedInUser = user && user.id;

	if (loading) {
		return <Spinner />;
	}
	if (idOfPost !== idOfLoggedInUser && isAuthenticated === true) {
		console.log('id of post:' + idOfPost);
		console.log('id of logged in user:' + idOfLoggedInUser);
		console.log('loading status: ' + loading);
		console.log('auth status: ' + isAuthenticated);
		return <Redirect to={`/posts`} />;
	}
	// const id = [ post && post.userId, auth.user && auth.user.id ];
	// console.log(id);
	console.log('id of post:' + idOfPost);
	console.log('id of logged in user:' + idOfLoggedInUser);
	console.log('loading status: ' + loading);
	console.log('auth status: ' + isAuthenticated);
	// {idOfPost !== idOfLoggedInUser && <Redirect to='/posts' />}
	return (
		<Fragment>
			<h1>somethinf</h1>
			<h1>{idOfPost}</h1>
			<h1>{idOfLoggedInUser}</h1>
			{/* {idOfPost !== idOfLoggedInUser && <Redirect to='/posts' />} */}
		</Fragment>
	);
};

PhotosToPost.propTypes = {
	getPost: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	post: state.post,
	auth: state.auth
});

export default connect(mapStateToProps, { addRecipePhotos, getPost })(PhotosToPost);
