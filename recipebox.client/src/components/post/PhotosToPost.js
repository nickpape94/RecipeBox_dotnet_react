import React, { Fragment, useEffect, useState, useMemo } from 'react';
import PropTypes from 'prop-types';
import { Redirect, Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { addRecipePhotos } from '../../actions/photo';
import { getPost } from '../../actions/post';
import { getUser } from '../../actions/user';
import Spinner from '../layout/Spinner';
import { useDropzone } from 'react-dropzone';

const thumbsContainer = {
	display: 'flex',
	flexDirection: 'row',
	flexWrap: 'wrap',
	marginTop: 16,
	justifyContent: 'center'
};

const thumb = {
	display: 'inline-flex',
	borderRadius: 2,
	border: '1px solid #eaeaea',
	marginBottom: 8,
	marginRight: 8,
	width: 300,
	height: 300,
	padding: 4,
	boxSizing: 'border-box'
};

const thumbInner = {
	display: 'flex',
	minWidth: 0,
	overflow: 'hidden'
};

const img = {
	display: 'block',
	width: 'auto',
	height: '100%'
};

const baseStyle = {
	flex: 1,
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'center',
	padding: '20px',
	borderWidth: 2,
	borderRadius: 2,
	borderColor: '#eeeeee',
	borderStyle: 'dashed',
	backgroundColor: '#fafafa',
	color: '#bdbdbd',
	outline: 'none',
	transition: 'border .24s ease-in-out'
};

const activeStyle = {
	borderColor: '#2196f3'
};

const acceptStyle = {
	borderColor: '#00e676'
};

const rejectStyle = {
	borderColor: '#ff1744'
};

const PhotosToPost = ({ addRecipePhotos, getPost, post: { post }, auth: { loading, user }, history }) => {
	// const [state={notLoaded:true}, setState] = useState(null);
	useEffect(() => {
		if (post !== null) {
			getPost(post.postId);
		}
	}, []);

	const [ files, setFiles ] = useState([]);
	const [ fileUrl, setFileUrl ] = useState('');
	const { getRootProps, getInputProps, isDragActive, isDragAccept, isDragReject, acceptedFiles, open } = useDropzone({
		accept: 'image/*',
		onDrop: (acceptedFiles) => {
			setFiles(
				acceptedFiles.map((file) =>
					Object.assign(file, {
						preview: URL.createObjectURL(file)
					})
				)
			);
		}
	});

	const style = useMemo(
		() => ({
			...baseStyle,
			...(isDragActive ? activeStyle : {}),
			...(isDragAccept ? acceptStyle : {}),
			...(isDragReject ? rejectStyle : {})
		}),
		[ isDragActive, isDragReject ]
	);

	const thumbs = files.map((file) => (
		<div style={thumb} key={file.name}>
			<div style={thumbInner}>
				<img src={file.preview} style={img} />
			</div>
		</div>
	));

	useEffect(
		() => () => {
			// Make sure to revoke the data uris to avoid memory leaks
			files.forEach((file) => URL.revokeObjectURL(file.preview));
		},
		[ files ]
	);

	// const filepath = acceptedFiles.map((file) => (
	// 	<li key={file.path}>
	// 		{file.path} - {file.size} bytes
	// 	</li>
	// ));

	const { postPhotos } = files;

	const onChange = (e) => {
		setFiles(e.target.files[0]);
	};

	const userIdOfPost = post && post.userId;
	const idOfLoggedInUser = user && user.id;

	if (loading) {
		return <Spinner />;
	}
	// if (userIdOfPost !== idOfLoggedInUser || post === null) {
	// 	return <Redirect to={`/posts`} />;
	// }

	console.log(fileUrl);

	const onSubmit = async (e) => {
		e.preventDefault();
		const formData = new FormData();
		formData.append('file', files);
		const response = await addRecipePhotos(post.postId, history, formData);
		setFileUrl(response.url);
		// console.log(response);
		// console.log('url ' + response.url);
	};

	return (
		<Fragment>
			<div className='text-center lead m-1'>
				<i className='fas fa-upload fa-2x text-primary' />{' '}
				<h3>Share Photos Of Your Recipe For Others To See</h3>
			</div>
			<div className='my-2 text-center'>
				<section className='container'>
					<div {...getRootProps({ style })}>
						<input {...getInputProps()} />
						<p>Drag 'n' drop some files heres</p>
						<button className='button my-1 btn btn-primary'>Open Files</button>
					</div>
					<aside style={thumbsContainer}>{thumbs}</aside>
				</section>
			</div>
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
