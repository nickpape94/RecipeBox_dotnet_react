import React, { useEffect, useState, useMemo, Fragment } from 'react';
import PropTypes from 'prop-types';
import { useDropzone } from 'react-dropzone';
import {
	thumbsContainer,
	thumb,
	thumbInner,
	img,
	baseStyle,
	activeStyle,
	acceptStyle,
	rejectStyle
} from '../layout/PhotoUploadStyles';

const PhotoPreview = ({ files, setFiles, newFiles = [], setNewFiles, edit = false, deleteRecipePhoto }) => {
	const removeFile = (file) => () => {
		const fileRemoved = [ ...files ];
		fileRemoved.splice(fileRemoved.indexOf(file), 1);
		setFiles(fileRemoved);
		if (edit) {
			deleteRecipePhoto(file.postId, file.postPhotoId);
		}
	};

	const removeNewFile = (file) => () => {
		const newFileRemoved = [ ...newFiles ];
		newFileRemoved.splice(newFileRemoved.indexOf(file), 1);
		setFiles(newFileRemoved);
	};

	const removeAllFiles = () => {
		setFiles([]);
	};

	const removeAllNewFiles = () => {
		setNewFiles([]);
	};

	const { getRootProps, getInputProps, isDragActive, isDragAccept, isDragReject, acceptedFiles, open } = useDropzone({
		accept: 'image/*',
		onDrop: (acceptedFiles) => {
			!edit
				? setFiles(
						acceptedFiles.map((file) =>
							Object.assign(file, {
								preview: URL.createObjectURL(file)
							})
						)
					)
				: setNewFiles(
						acceptedFiles.map((file) =>
							Object.assign(file, {
								preview: URL.createObjectURL(file)
							})
						)
					);
		}
	});

	files.length = Math.min(files.length, 6);
	newFiles.length = 6 - files.length;
	console.log(files.length, newFiles.length);

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
		<div className='thumb-style' style={thumb} key={file.name}>
			<div style={thumbInner}>
				{files[0].postPhotoId !== undefined && files[0].postPhotoId !== null ? (
					<img src={file.url} style={img} />
				) : (
					<img src={file.preview} style={img} />
				)}

				<button onClick={removeFile(file)}>
					<i className='fas fa-trash-alt fa-2x' />
				</button>
			</div>
		</div>
	));

	const newThumbs = newFiles.map((file) => (
		<div className='thumb-style' style={thumb} key={file.name}>
			<div style={thumbInner}>
				{files[0].postPhotoId !== undefined && files[0].postPhotoId !== null ? (
					<img src={file.url} style={img} />
				) : (
					<img src={file.preview} style={img} />
				)}

				<button onClick={removeNewFile(file)}>
					<i className='fas fa-trash-alt fa-2x' />
				</button>
			</div>
		</div>
	));

	useEffect(
		() => () => {
			// Make sure to revoke the data uris to avoid memory leaks
			files.forEach((file) => URL.revokeObjectURL(file.preview));
			newFiles.forEach((file) => URL.revokeObjectURL(file.preview));
		},
		[ files, newFiles ]
	);

	return (
		<div className='my-2 text-center'>
			<section className='container'>
				{!edit && (
					<div {...getRootProps({ style })}>
						<input {...getInputProps()} />
						<p>Drag 'n' drop some files here</p>
						<p>(Maximum of 6 photos)</p>
						<button className='button my-1 btn btn-primary'>Open Files</button>
					</div>
				)}

				{files.length === 0 && edit && <h3>No photos currently uploaded</h3>}
				{files.length > 0 &&
				edit && (
					<Fragment>
						<h3>Current photos</h3>
						<aside style={thumbsContainer}>{thumbs}</aside>
					</Fragment>
				)}
				{!edit && (
					<button className='btn btn-danger' onClick={removeAllFiles}>
						Remove All
					</button>
				)}
				{newFiles.length > 0 &&
				newFiles[0] != null &&
				edit && (
					<button className='btn btn-danger' onClick={removeAllNewFiles}>
						Remove All
					</button>
				)}

				{edit && (
					<div {...getRootProps({ style })}>
						<input {...getInputProps()} />
						<p>Drag 'n' drop some files here</p>
						<p>(Maximum of 6 photos)</p>
						<button className='button my-1 btn btn-primary'>Open Files</button>
					</div>
				)}
				{edit && newFiles.length === 0 && <aside style={thumbsContainer}>{newThumbs}</aside>}
			</section>
		</div>
	);
};

export default PhotoPreview;
