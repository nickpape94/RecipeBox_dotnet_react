import React, { Fragment, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { Redirect, Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { addRecipePhotos } from '../../actions/photo';
import { getPost } from '../../actions/post';
import { getUser } from '../../actions/user';
import Spinner from '../layout/Spinner';

const PhotosToPost = ({ addRecipePhotos, getPost, post: { post }, auth: { loading, user }, history }) => {
	// const [state={notLoaded:true}, setState] = useState(null);
	useEffect(() => {
		if (post !== null) {
			getPost(post.postId);
		}
	}, []);

	const [ file, setFile ] = useState('');
	const [ filename, setFilename ] = useState('Choose File(s)');
	const [ uploadedFile, setUploadedFile ] = useState({});

	const { postPhotos } = file;

	const onChange = (e) => {
		setFile(e.target.files[0]);
		setFilename(e.target.files[0]);
	};

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
		const formData = new FormData();
		formData.append('file', file);
		const response = await addRecipePhotos(post.postId, history, formData);
		console.log(response);
		console.log('url ' + response.url);
	};

	return (
		<Fragment>
			<div className='text-center lead m-1'>
				<i className='fas fa-upload fa-2x text-primary' />{' '}
				<h3>Share Photos Of Your Recipe For Others To See</h3>
			</div>
			<form className='container' onSubmit={(e) => onSubmit(e)}>
				<div className='my-2 text-center'>
					<div className='row'>
						<div className='col-md-6'>
							<input
								type='file'
								className='form-control'
								// id='images'
								// name='images[]'
								// name='files'
								multiple
								value={postPhotos}
								onChange={(e) => onChange(e)}
							/>
						</div>
					</div>
					<div className='row' id='image_preview' />
				</div>

				<div className='my-1 text-center'>
					<input type='submit' className='upload_photo btn-success' value='Upload' />
				</div>
				<div className='lnk m-1 text-center a:hover'>
					<Link to='/posts'>Continue without uploading any photos</Link>
				</div>
			</form>
			{uploadedFile ? (
				<div className='row mt-5'>
					<div className='col-md-6 m-auto'>
						<h3 className='text-center'>{uploadedFile.fileName}</h3>
						<img style={{ width: '100%' }} src={uploadedFile.filePath} alt='' />
					</div>
				</div>
			) : null}
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
