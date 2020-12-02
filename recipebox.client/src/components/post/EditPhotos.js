import React, { Fragment, useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { withRouter, Redirect, Link } from 'react-router-dom';
import { addRecipePhotos, deleteRecipePhoto } from '../../actions/photo';
import { getPost } from '../../actions/post';
import Spinner from '../layout/Spinner';
import PhotoManagement from '../photo/PhotoManagement';
import PhotoPreview from '../photo/PhotoPreview';

const EditPhotos = ({
	getPost,
	addRecipePhotos,
	deleteRecipePhoto,
	post: { post },
	match,
	auth: { user, loading },
	history
}) => {
	const [ loadingPage, setLoadingPage ] = useState(false);
	const [ currentFiles, setCurrentFiles ] = useState([]);
	const [ newFiles, setNewFiles ] = useState([]);
	const [ uploading, startedUploading ] = useState(false);

	useEffect(
		() => {
			if (post === null) {
				getPost(match.params.id, setLoadingPage);
			}

			if (post !== null) {
				setCurrentFiles(loading || !post.postPhoto ? null : post.postPhoto);
			}
		},
		[ getPost, match.params.id, loading, post ]
	);

	if (loadingPage || loading || uploading) {
		return <Spinner />;
	}

	if (user && user.id !== post.userId) {
		return <Redirect to='/posts' />;
	}

	const onSubmit = async (e) => {
		e.preventDefault();

		startedUploading(true);

		for (let i = 0; i < newFiles.length; i++) {
			const formData = new FormData();
			formData.append('file', newFiles[i]);
			addRecipePhotos(post.postId, history, formData);
		}
	};

	return (
		<Fragment>
			<div className='text-center lead m-1'>
				<i className='fas fa-upload fa-2x text-primary' /> <h3>Manage photos </h3>
				{/* <small>(Please select the files in the order you would like them to be displayed)</small> */}
			</div>
			{currentFiles &&
			!loading && (
				<PhotoPreview
					files={currentFiles}
					setFiles={setCurrentFiles}
					newFiles={newFiles}
					setNewFiles={setNewFiles}
					edit={true}
					deleteRecipePhoto={deleteRecipePhoto}
				/>
			)}
			{/* {newFiles &&
			!loading && (
				<PhotoManagement
					currentFiles={currentFiles}
					setCurrentFiles={setCurrentFiles}
					newFiles={newFiles}
					setNewFiles={setNewFiles}
					deleteRecipePhoto={deleteRecipePhoto}
				/>
			)} */}
			<form className='container' onSubmit={(e) => onSubmit(e)}>
				<div className='my-1 text-center'>
					{newFiles && newFiles.length === 0 ? (
						<input type='submit' className='upload_photo btn-success' value='Upload' disabled />
					) : (
						<input type='submit' className='upload_photo btn-success' value='Upload' />
					)}
				</div>
			</form>
			<div className='lnk m-1 text-center a:hover'>
				<Link to={`/posts/${post.postId}`}>Continue without adding any new photos</Link>
			</div>
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

export default connect(mapStateToProps, { getPost, deleteRecipePhoto, addRecipePhotos })(withRouter(EditPhotos));
