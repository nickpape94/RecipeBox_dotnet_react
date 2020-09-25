import React, { Fragment, useEffect, useState, useMemo } from 'react';
import PropTypes from 'prop-types';
import { Redirect, Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { addRecipePhotos } from '../../actions/photo';
import { getPost } from '../../actions/post';
import Spinner from '../layout/Spinner';
import PhotoPreview from '../photo/PhotoPreview';

const PhotosToPost = ({ addRecipePhotos, getPost, post: { post }, auth: { loading, user }, history }) => {
	// const [state={notLoaded:true}, setState] = useState(null);
	useEffect(() => {
		if (post !== null) {
			getPost(post.postId);
		}
	}, []);

	const [ files, setFiles ] = useState([]);

	const userIdOfPost = post && post.userId;
	const idOfLoggedInUser = user && user.id;

	if (loading) {
		return <Spinner />;
	}
	// if (userIdOfPost !== idOfLoggedInUser || post === null) {
	// 	return <Redirect to={`/posts`} />;
	// }

	const onSubmit = async (e) => {
		e.preventDefault();

		for (let i = 0; i < files.length; i++) {
			const formData = new FormData();
			formData.append('file', files[i]);
			addRecipePhotos(post.postId, history, formData);
		}
	};

	return (
		<Fragment>
			<div className='text-center lead m-1'>
				<i className='fas fa-upload fa-2x text-primary' />{' '}
				<h3>Share Photos Of Your Recipe For Others To See</h3>
				<small>(Maximum Of 6 Posts Per Post)</small>
			</div>
			<PhotoPreview files={files} setFiles={setFiles} />
			<form className='container' onSubmit={(e) => onSubmit(e)}>
				<div className='my-1 text-center'>
					<input type='submit' className='upload_photo btn-success' value='Upload' />
				</div>
				<div className='lnk m-1 text-center a:hover'>
					<Link to='/posts'>Continue without uploading any photos</Link>
				</div>
			</form>
		</Fragment>
	);
};

PhotosToPost.propTypes = {
	getPost: PropTypes.func.isRequired,
	addRecipePhotos: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	post: state.post,
	auth: state.auth,
	photo: state.photo
});

export default connect(mapStateToProps, { getPost, addRecipePhotos })(PhotosToPost);
