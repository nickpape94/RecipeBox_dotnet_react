import React, { Fragment, useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { withRouter, Redirect, Link } from 'react-router-dom';
import { addRecipePhotos, deleteRecipePhoto } from '../../actions/photo';
import { getPost } from '../../actions/post';
import Spinner from '../layout/Spinner';
import PhotoManagement from '../photo/PhotoManagement';

const EditPhotos = ({ getPost, deleteRecipePhoto, post: { post }, match, auth: { user, loading } }) => {
	const [ loadingPage, setLoadingPage ] = useState(false);
	const [ files, setFiles ] = useState([]);
	// const [ shit, setShit ] = useState('');

	useEffect(
		() => {
			if (post === null) {
				getPost(match.params.id, setLoadingPage);
			}

			if (post !== null) {
				setFiles(loading || !post.postPhoto ? null : post.postPhoto);
			}
		},
		[ getPost, match.params.id, loading, post ]
	);

	if (loadingPage || loading) {
		return <Spinner />;
	}

	if (user && user.id !== post.userId) {
		return <Redirect to='/posts' />;
	}

	return (
		<Fragment>
			<div className='text-center lead m-1'>
				<i className='fas fa-upload fa-2x text-primary' /> <h3>Manage photos </h3>
				{/* <small>(Please select the files in the order you would like them to be displayed)</small> */}
			</div>
			{files &&
			!loading && <PhotoManagement files={files} setFiles={setFiles} deleteRecipePhoto={deleteRecipePhoto} />}
			<form className='container'>
				<div className='my-1 text-center'>
					{files && files.length === 0 ? (
						<input type='submit' className='upload_photo btn-success' value='Upload' disabled />
					) : (
						<input type='submit' className='upload_photo btn-success' value='Upload' />
					)}
				</div>
				<div className='lnk m-1 text-center a:hover'>
					<Link to={`/posts/${post.postId}`}>Continue without uploading any photos</Link>
				</div>
			</form>
		</Fragment>
	);
};

EditPhotos.propTypes = {
	getPost: PropTypes.func.isRequired,
	deleteRecipePhoto: PropTypes.func.isRequired,
	auth: PropTypes.object.isRequired,
	post: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	auth: state.auth,
	post: state.post
});

export default connect(mapStateToProps, { getPost, deleteRecipePhoto })(withRouter(EditPhotos));
