import React, { Fragment, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Redirect } from 'react-router-dom';
import { connect } from 'react-redux';
import { addRecipePhotos } from '../../actions/photo';
import { getPost } from '../../actions/post';
import { getUser } from '../../actions/user';

const PhotosToPost = ({ getPost, post: { post }, match, user }) => {
	useEffect(() => {
		getPost(match.params.postId);
	}, []);

	// console.log(post.postId);
	// console.log(user);
	// const idOfPost = post && post.userId;
	// const idOfLoggedInUser = auth.user && auth.user.id;

	// if (idOfPost && idOfLoggedInUser === null) {
	// 	return <Redirect to={`/add-photos/${post.postId}`} />;
	// }
	// const id = [ post && post.userId, auth.user && auth.user.id ];
	// console.log(id);
	// console.log(idOfPost);
	// console.log(idOfLoggedInUser);

	// {idOfPost !== idOfLoggedInUser && <Redirect to='/posts' />}
	return (
		<Fragment>
			<h1>somethinf</h1>
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
