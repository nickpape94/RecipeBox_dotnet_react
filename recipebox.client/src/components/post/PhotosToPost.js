import React, { Fragment, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Redirect, Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { addRecipePhotos } from '../../actions/photo';
import { getPost } from '../../actions/post';
import { getUser } from '../../actions/user';
import Spinner from '../layout/Spinner';

const PhotosToPost = ({ getPost, post: { post }, auth: { loading, user } }) => {
	// const [state={notLoaded:true}, setState] = useState(null);
	useEffect(() => {
		if (post !== null) {
			getPost(post.postId);
		}
	}, []);

	const userIdOfPost = post && post.userId;
	const idOfLoggedInUser = user && user.id;

	if (loading) {
		return <Spinner />;
	}
	// if (userIdOfPost !== idOfLoggedInUser || post === null) {
	// 	return <Redirect to={`/posts`} />;
	// }

	return (
		<Fragment>
			<h2 className='text-center lead m-1'>
				<i class='fas fa-upload fa-2x text-primary' /> <h3>Share Photos Of Your Recipe For Others To See</h3>
			</h2>
			<form className='container'>
				{/* <div className='display-4 text-center mb-4'>
					<h1>File Upload</h1>
					<h1>{userIdOfPost}</h1>
					<h1>{idOfLoggedInUser}</h1>
				</div> */}

				<div className='custom-file'>
					<input type='file' className='custom-file-input' id='customFile' multiple />
				</div>
				<label className='custom-file-label' for='customFile'>
					Choose file(s)
				</label>

				<div className='my-1 text-center'>
					<input type='submit' className='btn btn-success' value='Upload' />
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
	post: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	post: state.post,
	auth: state.auth
});

export default connect(mapStateToProps, { addRecipePhotos, getPost })(PhotosToPost);
